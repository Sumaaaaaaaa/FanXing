
// TODO:【优化】现在的EndAction是一个替换原生End的解决方法，这样非常不优雅，可以尝试以增加Action的方式进行注册，并另外增加一个选择在结束时进行加载的功能。

//TODO: 场景动画功能
//TODO: Auto键
//TODO: _instance.Clicker 是不是可以不需要任何的执行操作         

using System;
using System.Collections;
using System.Collections.Generic;
using AVG_System.Scripts.OutSide;
using Unity.VisualScripting;
using UnityEngine;

namespace AVG_System.Scripts
{
// 字体 https://github.com/lxgw/LxgwWenKai
// 默认CG贴图 https://80.lv/articles/free-uv-checker-maps/
// 默认替换背景贴图 https://noranekogames.itch.io/yumebackground
    public class Frase
    {
        public string name;
        public string dialog;
        public string[] characterAndEmotion; // 0:Character, 1:Emotion.....
        public string backGround;
        public string audio;
        public string CG;
        /// <summary>
        /// 构造方法
        /// </summary>
        public Frase(string name, string dialog, string backGround, string[] characterAndEmotion=null, string audio=null, string CG=null)
        {
            this.name = name;
            this.dialog = dialog;
            this.backGround = backGround;
            this.characterAndEmotion = characterAndEmotion;
            this.audio = audio;
            this.CG = CG;
        }
    }
    class InFieldCharacters
    {
        private List<KeyAndValue<string, GameObject>> list = new List<KeyAndValue<string, GameObject>>(3);
        public void Remove(string key) 
        { 
            for (int i = 0; i < list.Count; i++)
            {
                if (key == list[i].Key)
                {
                    list.RemoveAt(i);
                }
            }
        }
        public void Clear()
        {
            list.Clear();
        }
        public bool TryGetValue(string key, out GameObject gameObject) 
        { 
            foreach(var i in list)
            {
                if (i.Key == key)
                {
                    gameObject = i.Value;
                    return true;
                }
            }
            gameObject = null;
            return false;
        }
        public bool Contains(string key) 
        {
            foreach (var i in list)
            {
                if (i.Key == key)
                {
                    return true;
                }
            }
            return false;
        }
        public void Add(string key, GameObject gameObject)
        {
            list.Add(new KeyAndValue<string, GameObject>(key, gameObject));
        }
        public int Count()
        {
            return list.Count;
        }
        public string[] GetAllNames()
        {
            string[] names = new string[list.Count];
            for(int i = 0; i< list.Count;i++)
            {
                names[i] = list[i].Key;
            }
            return names;
        }
        public GameObject[] GetAllObjects()
        {
            GameObject[] gameObjects = new GameObject[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                gameObjects[i] = list[i].Value;
            }
            return gameObjects;
        }
        public GameObject this[int index]
        {
            get
            {
                return list[index].Value;
            }
            set
            {
                list[index].Value = value;
            }
        }
    }

    static class ScriptsManager
    {
        private static int _now = 0;
        public static int Now
        {
            get => _now;
        }
        
        private static string[] _titles;
        public static string[] Titles
        {
            set => _titles = value;
            get => _titles;
        }

        public static string NowTitle => _titles[_now];

        /// <summary>
        /// 进行保存
        /// </summary>
        /// <param name="dialogIndex"></param>
        public static void Save(int dialogIndex)
        {
            Debug.LogError("Save -- 废弃！");
            PlayerPrefs.SetInt("scriptIndex", _now);
            PlayerPrefs.SetInt("dialogIndex", dialogIndex);
            PlayerPrefs.Save();
        }

        public static string Load(int scriptIndex)
        {
            _now = scriptIndex;
            return _titles[_now];
        }
        public static bool TryGetNext(out string value)
        {
            if (_now + 1 >= _titles.Length)
            {
                value = null;
                return false;
            }
            _now++;
            value = _titles[_now];
            return true;
        }
        public static string GetFirst()
        {
            _now = 0;
            return _titles[0];
        }
    }
    
// JSON用的数据结构定义
    [Serializable] class Json_Dialog
    {
        public string name;
        public string dialog;
        public string[] characterAndEmotion;
        public string background;
        public string audio;
        public string CGImage;
    }
    [Serializable] class Json_root
    {
        public Json_Dialog[] Dialogs;
    }

    public class AVG_System : MonoBehaviour
    {
        // 单例
        private static AVG_System _instance; // 单例实体
        
        // 自定义属性参数
        [Header("属性参数")]
        [SerializeField] private float fastForwardInterval = 0.5f;
        [SerializeField] private bool isLogPreloadEnabled = false;

        [SerializeField] private string[] scripts;
        // 调试模式
        [Header("<debug_IgnoreFileMissing>模式")]
        [SerializeField] private bool debug_IgnoreFileMissing = false;
        [Header("<debug_IgnoreFileMissing>模式...默认数据")]
        [SerializeField] private Sprite debug_DefaultBackGround = null;
        [SerializeField] private Sprite debug_DefaultCharacter = null;
        [SerializeField] private AudioClip debug_DefaultAudio = null;
        [SerializeField] private Sprite debug_DefaultCG = null;
        
        // 子游戏元件
        [Header("子组件")]
        [SerializeField] private GameObject Clicker;
        [SerializeField] private Dialog_Name_Script dialogName;
        [SerializeField] private Dialog_Script cdialog;
        [SerializeField] private BackGround_Script cBackGround;
        [SerializeField] private DialogBackGround_Script cDialogBackground;
        [SerializeField] private Log cLog;
        [SerializeField] private BackGround_Script cCG;
    
        // 组件
        private static AudioSource _audioSource;
        private static CanvasGroup _canvasGroup;

        // 角色Sprite部分
        [SerializeField] private GameObject _characterSprites;
        [SerializeField] private GameObject _characterSpritePrefab;
    
        // 资源对象
        static private Dictionary<string, Sprite> _resource_CG = new Dictionary<string, Sprite>();
        static private Dictionary<string, Sprite> _resource_backGrounds = new Dictionary<string, Sprite>();
        static private Dictionary<string, Dictionary<string, Sprite>> _resource_characterAndEmotion = new Dictionary<string, Dictionary<string, Sprite>>();
        static private Dictionary<string, AudioClip> _resource_audios = new Dictionary<string, AudioClip>();

        // 资源卸载方法
        static private void Unload()
        {
            print($"<{nameof(AVG_System)}>...<{nameof(Unload)}>......旧资源已被卸载.");
            _resource_CG.Clear();
            _resource_backGrounds.Clear();
            _resource_characterAndEmotion.Clear();
            _resource_audios.Clear();
        }

        // 静态参数
        static public float speed_character = 0.05f; // 文字逐个显示速度(每speed_character秒显示一个字符)
        static public float fade_time = 1f;   // 渐变时间（秒）

        // 系统变量
        static private bool _isActive = false; // AVG系统是否在运行
        static private Coroutine _animation_c = null; // 动画协程
        static private Frase[] _dialog = null; // 对话数组
        static private int _fraseNow = -1; // 当前显示句子Index
        static public int FraseNow => _fraseNow;
        static private int _fadeState = 0; // 过渡标识符
        static private Coroutine _fadeCoroutine = null;
        private static Coroutine _fastForwardCoroutine = null; // 快进协程
        private static Action _endAction = null;
        private static Action _allEndAction = null;
        private InFieldCharacters _inFieldCharacters= new InFieldCharacters();

        private void Awake()
        {
            // 单例
#if DEBUG
            if (_instance != null)
                Debug.LogWarning($"AVG_System...单例模式错误...出现了多个{this.GetType()}的实例");
#endif
            _instance = this;
            _canvasGroup = this.GetComponent<CanvasGroup>();
            // 初始Alpha设为0
            _canvasGroup.alpha = 0.0f;
            // 初始不可交互
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            // 给脚本管理器赋值
            ScriptsManager.Titles = scripts;
        }

        // 公开方法
        
        public static void BeginAuto(bool isLoad,int scriptIndex, int dialogIndex,Action allEndAction)
        {
            _allEndAction = allEndAction;
            if (isLoad)
            {  
                LoadAndBegin(ScriptsManager.Load(scriptIndex),dialogIndex,GoNextOrEnd);
            }
            // 开始新游戏
            else
            {
                LoadAndBegin(ScriptsManager.GetFirst(),0,GoNextOrEnd);
                return;
            }
        }
        public static void LoadAndBegin(string path, int index = 0,Action action = null)
        {
            _instance.StartCoroutine(LoadAndBegin_IE(path, index, action));
        }
        private static IEnumerator LoadAndBegin_IE(string path, int index = 0,Action endAction = null)
        {
            path = $"Dialogs/{path}";
            if (_isActive)
            {
                End();
                yield return new WaitForSeconds(fade_time);
                yield return new WaitForEndOfFrame();
                yield return _instance.IEload(path);
                Begin(index,endAction);
            }
            else
            {
                yield return _instance.IEload(path);
                Begin(index,endAction);
            }
        }
        /// <summary>
        /// 通过输入一段对话数组开始一段对话
        /// </summary>
        public static void Begin(int startIndex = 0,Action endAction = null)
        {
            if(_dialog == null)
            {
                Debug.LogError("没有已经载入的对话，确认是否已经通过Load()载入了对话");
                return;
            }
            
            _isActive = true;

            // 显示
            _instance.dialogName.Clear();
            _instance.cdialog.Clear();
            FadeIn();

            // 启用点击面板
            _instance.Clicker.SetActive(true);
            // 使UI可交互
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            // 变量存入
            _fraseNow = startIndex;
            _endAction = endAction ?? End;

            // 在各个组件显示结束后，显示第一段对话。
            _animation_c = _instance.StartCoroutine(ShowFirstLine(startIndex));
        }
        /// <summary>
        /// 继续对话，显示全部文字或进入下一句话
        /// </summary>
        public static void Next()
        {
            // DEBUG部分
            if (!_isActive)
            {
                Debug.LogError("AVG系统未开始对话，但是使用了Next()方法");
                return;
            }
            // ------------------------跳过部分------------------------
            // 若在动画中，跳过后续操作
            if (_animation_c is not null)
            {
                return;
            }
            bool isSkiped = false;
            // 检查文本是否在逐字显示 
            if (_instance.cdialog.IsAnimation)
            {
                //TODO: [代码优化]-这些是不是可以通过接口优化实现
                _instance.cdialog.Skip();
                isSkiped = true;
            }
            if(_instance.dialogName.isAnimation)
            {
                _instance.dialogName.Skip(); 
                isSkiped = true;
            }
            if(_instance.cDialogBackground.isAnimation)
            {
                _instance.cDialogBackground.Skip(); 
                isSkiped = true;
            }
            if(_instance.cBackGround.IsAnimation)
            {
                _instance.cBackGround.Skip(); 
                isSkiped = true;
            }

            if (_instance.cCG.IsAnimation)
            {
                _instance.cCG.Skip();
                isSkiped = true;
            }
            for (int i=0;i<_instance._characterSprites.GetComponent<RectTransform>().childCount;i++)
            {
                Transform transform = _instance._characterSprites.GetComponent<RectTransform>().GetChild(i);
                if (transform.GetComponent<CharacterSprite_Script>()._isAnimation)
                {
                    transform.GetComponent<CharacterSprite_Script>().Skip();
                    isSkiped = true;
                }
            }
            if (isSkiped)
            {
                return;
            }
            // ------------------------------------------------
            // ------------------------进行下一句话------------------------
            else
            {
                // 是结束对话？
                _fraseNow++;
                if (_fraseNow >= _dialog.Length)
                {
                    _endAction();
                    //End(); 
                    return;
                }
            
                // 下一句
                _instance.dialogName.Text = _dialog[_fraseNow].name;
                _instance.cdialog.Text = _dialog[_fraseNow].dialog;
                if (_dialog[_fraseNow].characterAndEmotion is not null) 
                    _instance.Sprite_GroupFunction(_dialog[_fraseNow].characterAndEmotion);
                if(_dialog[_fraseNow].backGround != "") BackGround_Goto(_dialog[_fraseNow].backGround);
                if (_dialog[_fraseNow].audio != "") Audio_Play(_dialog[_fraseNow].audio);
                
                if (_dialog[_fraseNow].CG.ToLower() == "out")
                {
                    CG_End();
                }else if (_dialog[_fraseNow].CG != "")
                {
                    CG_Goto(_dialog[_fraseNow].CG);
                }
            
                // LOG推入
                _instance.cLog.Add(_dialog[_fraseNow].name, 
                    _dialog[_fraseNow].dialog,
                    Audio_Play,
                    _dialog[_fraseNow].audio
                );
            }
        
        }
        public static void OpenLog()
        {
            if (!_isActive)
            {
                Debug.LogError("未开始对话，不能打开Log页面");
                return;
            }
            _instance.cLog.FadeIn();
            // 禁用点击面板
            _instance.Clicker.SetActive(false);
        }
        public static void CloseLog()
        {
            if (!_isActive)
            {
                Debug.LogError("未开始对话，不能进行Log页面的操作");
                return;
            }
            _instance.cLog.FadeOut();
            // 启用点击面板
            _instance.Clicker.SetActive(true);
            // 停止声音播放
            Audio_Stop();
        }
        /// <summary>
        /// 快进方法
        /// </summary>
        public static void StartFastForward()
        {
            // 如果不在运行，则判定为非法行为且不进行
            if (!_isActive)
            {
                Debug.LogError($"<{nameof(StartFastForward)}>......未开始对话，不能进行快进行为");
                return;
            }

            _fastForwardCoroutine = _instance.StartCoroutine(_instance.FastForward_IE());
        }
        public static void EndFastForward()
        {
            if (!_isActive)
            {
                //Debug.LogError($"<{nameof(EndFastForward)}>......未开始对话，不能进行快进行为");
                return;
            }
            if (_fastForwardCoroutine is null)
            {
                Debug.LogError($"<{nameof(EndFastForward)}>......未开始快进模式");
            }
            if(_fastForwardCoroutine is not null) _instance.StopCoroutine(_fastForwardCoroutine);
            _fastForwardCoroutine = null;
        }

        public static void Saving(Action<int> action)
        {
            action(_fraseNow);
        }
    
        // 内部方法
        private static void GoNextOrEnd()
        {
            if (ScriptsManager.TryGetNext(out string value))
            {
                // 如果有下一幕
                LoadAndBegin(value, 0, GoNextOrEnd);
                return;
            }
            // 没有下一幕
            // 执行全部结束Action
            if (_allEndAction is not null) _allEndAction();
            // 默认终止
            End();
        }
        private IEnumerator IEload(string path) //基于Resources！
        {
            //TODO: 是不是需要有一个锁定，以防止正在对话时改变_dialog？
            // 创建一个用于debug_IgnoreFileMissing判断的bool变量，当出现有替换素材的情况出现时将它翻转为True
            var debug_IgnoreFileMissing_isActived = false;
        
            // 进行异步读取
            ResourceRequest request = Resources.LoadAsync(path,typeof(TextAsset));
            request.allowSceneActivation = true;

            // 异步等待该请求
            yield return request;
            Debug.Log($"<{nameof(AVG_System)}>...<{nameof(IEload)}>...成功完成场景对话Json文件的读取");

            // 文件没找到、格式不对的异常提示
            if (request.asset == null)
            {
                Debug.LogError("AVGSystem...IEload...未能成功找到目标文件或格式有误，将不执行后续加载");
                yield break;
            }
            TextAsset result = request.asset as TextAsset;

            // 读取Json文件转为对象
            Json_root json_root = JsonUtility.FromJson<Json_root>(result.text);

            // 遍历以获得所有需要的资源列表
            List<string> backgrounds = new List<string>();
            List<string> characters = new List<string>();
            List<string> emotions = new List<string>();
            List<string> audios = new List<string>();
            List<string> CGs = new List<string>();
            foreach (Json_Dialog json_Dialog in json_root.Dialogs)
            {
                if (json_Dialog.background != "")
                {
                    backgrounds.Add(json_Dialog.background);
                }
                if (json_Dialog.characterAndEmotion.Length != 0)
                {
                    for (int i=0; i<json_Dialog.characterAndEmotion.Length;i+=2)
                    {
                        if (json_Dialog.characterAndEmotion[i+1] != "OUT")
                        {
                            characters.Add(json_Dialog.characterAndEmotion[i]);
                            emotions.Add(json_Dialog.characterAndEmotion[i+1]);
                        }
                    }
                }
                if (json_Dialog.audio != "")
                {
                    audios.Add(json_Dialog.audio);
                }

                if (json_Dialog.CGImage != "")
                {
                    CGs.Add(json_Dialog.CGImage);
                }
            }
        
            // 测试
            List<string>characterAndEmotion = new List<string>();
            for (int i=0;i<characters.Count;i++)
            {
                characterAndEmotion.Add($"{characters[i]}_{emotions[i]}");
            }
            string backGroundString = "";
            foreach (string i in backgrounds)
            {
                backGroundString += i;
                backGroundString += " / ";
            }
            string characterAndEmotionsString = "";
            foreach (string i in characterAndEmotion)
            {
                characterAndEmotionsString += i;
                characterAndEmotionsString += " / ";
            }
            string audiosString = "";
            foreach (string i in audios)
            {
                audiosString += i;
                audiosString += " / ";
            }
            var cgString = "";
            foreach (var i in CGs)
            {
                cgString += i;
                cgString += " / ";
            }
            Debug.Log($"遍历检测到相关需求资源……\n背景……{backGroundString}\n角色和其对应Sprite……{characterAndEmotionsString}\n音频……{audiosString}\nCG图像{cgString}");

            // 创建资源加载请求
            // 加载背景
            List<ResourceRequest> resourceRequests_background = new List<ResourceRequest>();
            foreach(string background in backgrounds)
            {
                resourceRequests_background.Add(Resources.LoadAsync<Sprite>($"BackGrounds/{background}"));
            }
            // 加载角色图像
            List<ResourceRequest> resourceRequests_characterAndEmotion = new List<ResourceRequest>();
            for (int i=0;i< characters.Count;i++)
            {
                resourceRequests_characterAndEmotion.Add(Resources.LoadAsync<Sprite>($"CharacterWithEmotion/{characters[i]}_{emotions[i]}"));
            }
            // 加载音频
            List <ResourceRequest> resourceRequests_audio = new List<ResourceRequest>();
            foreach (string audio in audios)
            {
                resourceRequests_audio.Add(Resources.LoadAsync<AudioClip>($"Audios/{audio}"));
            }
            // 加载CG
            var resourceRequests_CGs = new List<ResourceRequest>();
            foreach (string CG in CGs)
            {
                resourceRequests_CGs.Add(Resources.LoadAsync<Sprite>($"CGs/{CG}"));
            }
        
            // 等待加载完成
            foreach (ResourceRequest i in resourceRequests_background)
            {
                yield return i;
            }
            foreach (ResourceRequest i in resourceRequests_characterAndEmotion)
            {
                yield return i;
            }
            foreach (ResourceRequest i in resourceRequests_audio)
            {
                yield return i;
            }
            foreach (ResourceRequest i in resourceRequests_CGs)
            {
                yield return i;
            }
        
            // 引用对象
            // 背景
            for(int i=0;i< backgrounds.Count;i++)
            {
                // 若请求到的数据为空，说明没有这个数据
                if (resourceRequests_background[i].asset == null)
                {
                    if (_instance.debug_IgnoreFileMissing)
                    {
                        // -- 忽略文件缺失模式 --
                        // 将资源关联到默认的背景上
                        _resource_backGrounds.TryAdd(backgrounds[i], debug_DefaultBackGround);
                        debug_IgnoreFileMissing_isActived = true;
                        continue;
                    }
                    // -- 正常模式 --
                    Debug.LogError($"AVGSystem...IEload...在加载背景图\"{backgrounds[i]}\"时出现错误，确认Resources中该背景图是否存在。加载程序将终止。");
                    yield break;
                }
                _resource_backGrounds.TryAdd(backgrounds[i], resourceRequests_background[i].asset as Sprite); 
            }
        
            // 角色
            for (int i=0;i< characters.Count;i++)
            {
                if (resourceRequests_characterAndEmotion[i].asset == null)
                {
                    if (!debug_IgnoreFileMissing)
                    {
                        //-- 非 忽略文件缺失模式 --
                        // 则直接报错
                        Debug.LogError($"AVGSystem...IEload...在加载角色图\"{characters[i]}_{emotions[i]}\"时出现错误，确认Resources中该背景图是否存在。加载程序将终止。");
                        yield break;
                    }
                }
                // 判断整体组里有没有这个角色组
                if(_resource_characterAndEmotion.TryGetValue(characters[i],out Dictionary<string,Sprite> target))
                {
                    // 有这个角色组的情况下，直接加入表情单独的Sprite。
                    // 如果处于--略文件缺失模式 --，且获得到的sprite对象是空的，说明需要用默认角色Sprite进行赋值
                    if (debug_IgnoreFileMissing & resourceRequests_characterAndEmotion[i].asset == null)
                    {
                        target.TryAdd(emotions[i], debug_DefaultCharacter);
                        debug_IgnoreFileMissing_isActived = true;
                    }
                    else
                    {
                        target.TryAdd(emotions[i], resourceRequests_characterAndEmotion[i].asset as Sprite); 
                    }
                    //TODO:【优化】这样的情况下，一个Sprite对象可能会被多次获取，并在TryAdd时，不再赋值，这是一个浪费资源的做法
                }
                else
                {
                    // 建立角色组
                    _resource_characterAndEmotion.Add(characters[i], new Dictionary<string, Sprite>());
                    // 如果处于--略文件缺失模式 --，且获得到的sprite对象是空的，说明需要用默认角色Sprite进行赋值
                    if (debug_IgnoreFileMissing & resourceRequests_characterAndEmotion[i].asset == null)
                    {
                        _resource_characterAndEmotion[characters[i]].Add(emotions[i], debug_DefaultCharacter);
                        debug_IgnoreFileMissing_isActived = true;
                    }
                    else
                    {
                        // 否则是直接进行正常赋值就行
                        _resource_characterAndEmotion[characters[i]].Add(emotions[i], resourceRequests_characterAndEmotion[i].asset as Sprite);
                    }
                }
            }
            // 音频
            for (int i=0; i<audios.Count;i++)
            {
                if (resourceRequests_audio[i].asset == null)
                {
                    if (_instance.debug_IgnoreFileMissing)
                    {
                        // -- 忽略文件缺失模式 --
                        // 将资源关联到默认的音频上
                        _resource_audios.TryAdd(audios[i], _instance.debug_DefaultAudio);
                        debug_IgnoreFileMissing_isActived = true;
                        continue;
                    }
                    Debug.LogError($"AVGSystem...IEload...在加载音频\"{audios[i]}\"时出现错误，确认Audios中该音频是否存在。加载程序将终止。");
                    yield break;
                }
                _resource_audios.TryAdd(audios[i], resourceRequests_audio[i].asset as AudioClip);
            }
            // CG图像
            for (int i = 0; i < CGs.Count; i++)
            {
                if (resourceRequests_CGs[i].asset == null)
                {
                    if (_instance.debug_IgnoreFileMissing)
                    {
                        // -- 忽略文件缺失模式 --
                        // 将资源关联到默认的音频上
                        _resource_CG.TryAdd(CGs[i], _instance.debug_DefaultCG);
                        debug_IgnoreFileMissing_isActived = true;
                        continue;
                    }
                    Debug.LogError($"AVGSystem...IEload...在加载CG\"{CGs[i]}\"时出现错误，确认CGs中该音频是否存在。加载程序将终止。");
                    yield break;
                }
                _resource_CG.TryAdd(CGs[i], resourceRequests_CGs[i].asset as Sprite);
            }
            // 创建Frase[]对象
            Frase[] frases = new Frase[json_root.Dialogs.Length];
            for (int i=0; i< json_root.Dialogs.Length; i++)
            {
                frases[i] = new Frase(json_root.Dialogs[i].name,
                    json_root.Dialogs[i].dialog,
                    json_root.Dialogs[i].background,
                    json_root.Dialogs[i].characterAndEmotion,
                    json_root.Dialogs[i].audio,
                    json_root.Dialogs[i].CGImage);
            }
        
            //debug_IgnoreFileMissing的提示
            if (debug_IgnoreFileMissing_isActived)
            {
                Debug.LogWarning($"<{nameof(debug_IgnoreFileMissing)}>......在资源加载时，出现了文件缺失，因为打开了该模式，故将会通过默认素材替换。");
            }
        
            // *赋值*
            _dialog = frases;
            Debug.Log($"<{nameof(AVG_System)}>...<{nameof(IEload)}>......已经在背景完成了对\"{path}\"脚本的加载");
            yield break;
        }
        public static void End()
        {
            _isActive = false;
            FadeOut();
            // LOG系统关闭（因为结束对话的时候必然是隐藏的，所以直接清除就可以）
            _instance.cLog.Clear();
        
            BackGround_End();
            CG_End();
        
            _instance.Sprite_Clear();
            Audio_Stop();
            // 终止快进
            if (_fastForwardCoroutine is not null)
            {
                _instance.StopCoroutine(_fastForwardCoroutine);
                _fastForwardCoroutine = null;
            }
            // 禁用点击面板
            _instance.Clicker.SetActive(false);
            // 禁用UI可交互
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            // 卸载资源
            Unload();

            _dialog = null;
        }
        public static void ForceAllEnd()
        {
            // 执行全部结束Action
            if (_allEndAction is not null) _allEndAction();
            End();
        }
        private static IEnumerator ShowFirstLine(int startIndex)
        {
            var frase = _dialog[startIndex];
            // 显示 名字
            _instance.dialogName.Text = frase.name;

            // 显示 人物
            var names = new List<string>();
            var result = new List<string>();
            for (var i = startIndex; i >= 0; i--)
            {
                foreach (var j in _dialog[i].characterAndEmotion)
                {
                    for (var x = 0; x < _dialog[i].characterAndEmotion.Length; x+=2)
                    {
                        if (names.Contains(_dialog[i].characterAndEmotion[x])) continue;
                        names.Add(_dialog[i].characterAndEmotion[x]);
                        if (_dialog[i].characterAndEmotion[x + 1].ToLower() == "out") continue;
                        result.Add(_dialog[i].characterAndEmotion[x]);
                        result.Add(_dialog[i].characterAndEmotion[x+1]);
                    }
                }
            }
            _instance.Sprite_GroupFunction(result.ToArray());

            // 显示 背景
            for (var i = startIndex; i >= 0; i--)
            {
                if (_dialog[i].backGround.ToLower() == "out")
                {
                    break;
                }
                if (_dialog[i].backGround != "")
                {
                    BackGround_Goto(_dialog[i].backGround);
                    break;
                }
            }
        
            // 显示 CG
            for (int i = startIndex; i >= 0; i--)
            {
                if (_dialog[i].CG.ToLower() == "out")
                {
                    break;
                }
                if (_dialog[i].CG != "")
                {
                    CG_Goto(_dialog[i].CG);
                    break;
                }
            }
        
            // 等待动画时间
            yield return new WaitForSecondsRealtime(fade_time);

            // LOG推入
            if (_instance.isLogPreloadEnabled)
            {
                for (int i=0; i<=startIndex; i++)
                {
                    _instance.cLog.Add(_dialog[i].name,_dialog[i].dialog,Audio_Play,_dialog[i].audio);
                }
            } 
            else
            {
                _instance.cLog.Add(frase.name, frase.dialog, Audio_Play, frase.audio);
            }

            // 滚动文字
            _instance.cdialog.Text = frase.dialog;

            // 开始音频
            if (frase.audio != "")
            {
                Audio_Play(frase.audio);
            }

            // 清除动画锁
            _animation_c = null;
        }
    
        // 过渡效果
        private IEnumerator FastForward_IE()
        {
            Next();
            while (true)
            {
                yield return new WaitForSeconds(fastForwardInterval);
                Next();
            }
        }
        private static void FadeIn()
        {
            if (_fadeState == 0)
            {
                _fadeCoroutine = _instance.StartCoroutine(_instance.Fade_IE(false));
                _fadeState = 1;
            }
            return;
        }
        private static void FadeOut()
        {
            if (_fadeState == 2)
            {
                _fadeCoroutine = _instance.StartCoroutine(_instance.Fade_IE(true));
                _fadeState = -1;
            }
            else
            {
                return;
            }
            return;
        }
        private IEnumerator Fade_IE(bool isFadeOut)
        {
            var startTime = Time.time;

            while (Time.time - startTime < AVG_System.fade_time)
            {

                float alpha;
                if (isFadeOut)
                {
                    alpha = 1 - (Time.time - startTime) / AVG_System.fade_time;
                }
                else
                {
                    alpha = (Time.time - startTime) / AVG_System.fade_time;
                }
                _canvasGroup.alpha = alpha;
                yield return null;
            }

            if (isFadeOut)
            {
                _canvasGroup.alpha = 0.0f;
                _fadeState = 0;
            }
            else
            {
                _canvasGroup.alpha = 1.0f;
                _fadeState = 2;
            }
            _fadeCoroutine = null;
            yield break;
        }
        // 角色Sprite部分
        private void Sprite_GroupFunction(string[] value)
        {
            for (int i=0; i<value.Length; i+=2 )
            {
                Sprite_Function(value[i], value[i+1]);
            }
            ChangePostion();
        }
        private void Sprite_Function(string name, string key)
        {
            // 有基本三种处理方式；进入In，情绪变换Emotion、退出Out
            if (key.ToLower()=="out")
            {
                // 退出逻辑
                if (_inFieldCharacters.TryGetValue(name, out GameObject target))
                {
                    target.GetComponent<CharacterSprite_Script>().Out();
                    _inFieldCharacters.Remove(name);
                    return;
                }
                else
                {
                    Debug.LogError($"并没有找到名为{name}的角色对象，确认该角色是否已经通过In()进场");
                    return;
                }
            }
            else
            {
                // 根据场内是否具有角色判定是 进入 还是 情绪变化
                if (_inFieldCharacters.TryGetValue(name, out GameObject gameObject))
                {
                    // 若场内已经存在角色，则 改变情绪图片
                    gameObject.GetComponent<CharacterSprite_Script>().Emotion(_resource_characterAndEmotion[name][key]);
                    // 若处于debug_IgnoreFileMissing模式，并且确实使用了默认图像，则进行提示
                    if (debug_IgnoreFileMissing & _resource_characterAndEmotion[name][key] == debug_DefaultCharacter)
                    {
                        Debug.LogWarning($"<{nameof(debug_IgnoreFileMissing)}>...<{nameof(Sprite_Function)}>......缺失了`{name}_{key}`的素材，将通过默认资源代替");
                    }
                    return;
                }
                else
                {
                    // 若场内不存在角色，则 进入 该角色
                    if (_resource_characterAndEmotion.TryGetValue(name, out Dictionary<string,Sprite> target))
                    {
                        if (target.TryGetValue(key,out Sprite sprite))
                        {
                            GameObject instance = Instantiate(_characterSpritePrefab, _characterSprites.transform);
                            _inFieldCharacters.Add(name, instance);
                            instance.transform.SetAsFirstSibling();
                            instance.GetComponent<CharacterSprite_Script>().Emotion(_resource_characterAndEmotion[name][key]);
                            if (debug_IgnoreFileMissing & _resource_characterAndEmotion[name][key] == debug_DefaultCharacter)
                            {
                                Debug.LogWarning($"<{nameof(debug_IgnoreFileMissing)}>...<{nameof(Sprite_Function)}>......缺失了`{name}_{key}`的素材，系统使用默认角色贴图进行了替代。");
                            }
                            return;
                        }
                        else
                        {
                            Debug.LogError($"没有找到{name}角色的{key}情绪的Sprite图像");
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError($"没有找到名为{name}的角色Sprite图像");
                        return;
                    }
                }
            }

        }
    
        private void Sprite_Clear()
        {
            foreach (string i in _inFieldCharacters.GetAllNames())
            {
                Sprite_Function(i, "Out");
            }
        }
        private void ChangePostion()
        {
            if (_inFieldCharacters.Count() == 0)
            {
                return;
            }
            else if (_inFieldCharacters.Count() == 1)
            {
                // 一个人则为x=0
                _inFieldCharacters[0].GetComponent<CharacterSprite_Script>().MoveTo(0.0f,fade_time);
                return;
            }
            else if (_inFieldCharacters.Count() == 2)
            {
                _inFieldCharacters[0].GetComponent<CharacterSprite_Script>().MoveTo(-160f,fade_time);
                _inFieldCharacters[1].GetComponent<CharacterSprite_Script>().MoveTo(160f,fade_time);
                return;
            }
            else if (_inFieldCharacters.Count() == 3)
            {

                _inFieldCharacters[0].GetComponent<CharacterSprite_Script>().MoveTo(-250f,fade_time);
                _inFieldCharacters[1].GetComponent<CharacterSprite_Script>().MoveTo(0f,fade_time);
                _inFieldCharacters[2].GetComponent<CharacterSprite_Script>().MoveTo(250f,fade_time);

            }
            else
            {
                Debug.LogError("暂时没有设计大于3个人的排版，顾什么都不会发生");
                return;
            }
            // 两个人的情况下，为-160 +160 的x 修改

        }
        // 背景图部分
        private static void BackGround_Goto(string key)
        {
            if (_resource_backGrounds.TryGetValue(key, out Sprite table))
            {
                if (table == _instance.debug_DefaultBackGround & _instance.debug_IgnoreFileMissing)
                {
                    // -- 忽略文件缺失模式  --
                    Debug.LogWarning($"<{nameof(debug_IgnoreFileMissing)}>...<{nameof(BackGround_Goto)}>......缺失了`{key}`的素材，系统使用默认背景贴图进行了替代。");
                }
                _instance.cBackGround.Goto(table);
            }
            else
            {
                Debug.LogError($"没有在{_resource_backGrounds}中找到名为\"{key}\"的图像");
            }
        }
        private static void BackGround_End()
        {
            _instance.cBackGround.Goto(null);
        }
        // CG图部分
        private static void CG_Goto(string key)
        {
            if (_resource_CG.TryGetValue(key, out Sprite table))
            {
                if (table == _instance.debug_DefaultCG & _instance.debug_IgnoreFileMissing)
                {
                    // -- 忽略文件缺失模式  --
                    Debug.LogWarning($"<{nameof(debug_IgnoreFileMissing)}>...<{nameof(CG_Goto)}>......缺失了`{key}`的素材，系统使用默认CG贴图进行了替代。");
                }
                _instance.cCG.Goto(table);
            }
            else
            {
                Debug.LogError($"没有在{_resource_CG}中找到名为\"{key}\"的图像");
            }
        }
        private static void CG_End()
        {
            _instance.cCG.Goto(null);
        }
        // 音频部分
        private static void Audio_Play(string path)
        {
            AudioSystem.StopVoice();
            AudioSystem.PlayVoice(_resource_audios[path]);
            if (_instance.debug_IgnoreFileMissing & _resource_audios[path] == _instance.debug_DefaultAudio)
            {
                Debug.LogWarning($"<{nameof(debug_IgnoreFileMissing)}>...<{nameof(Audio_Play)}>......缺失了`{path}`的素材，将不会执行音频播放");
            }
        }
        private static void Audio_Stop()
        {
            AudioSystem.StopVoice();
        }
    }
}