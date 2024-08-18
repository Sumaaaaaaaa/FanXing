using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using AVG_System.Scripts;
using AVG_System.Scripts.Interface;
using UnityEngine;

namespace AVG_System.SaveAndLoadSystem
{
    [System.Serializable]
    public class SaveDataset
    {
        public SaveData[] saveDataset;
        public SaveDataset(int length)
        {
            this.saveDataset = new SaveData[length];
        }
        public override string ToString()
        {
            var returnString = $"[[{nameof(SaveDataset)}]] - [[Length = {saveDataset.Length}]]\n[hasData]-[scriptIndex]-[dialogIndex]-[title]-[timeStamp]";
            foreach (var saveData in saveDataset)
            {
                returnString += $"\n[{saveData.hasData}]-[{saveData.scriptIndex}]-[{saveData.dialogIndex}]-[{saveData.title}]-[{saveData.timeStamp}]";
            }
            return returnString;
        }
        public SaveData this[int index]
        {
            set => saveDataset[index] = value;
            get => saveDataset[index];
        }
    }
    [System.Serializable]
    public class SaveData
    {
        public bool hasData;
        public int scriptIndex;
        public int dialogIndex;
        public string title;
        public string timeStamp;
        public SaveData(int scriptIndex,int dialogIndex,string title, string timeStamp)
        {
            this.hasData = true;
            this.scriptIndex = scriptIndex;
            this.dialogIndex = dialogIndex;
            this.title = title;
            this.timeStamp = timeStamp;
        }
        public DateTime Time => DateTime.ParseExact(timeStamp, "o", null);
    }
    
    /// <summary>
    /// 存储模型层
    /// </summary>
    public static class SaveAndLoadSystemModel
    {
        // ---静态数据集---
        private const int Length = 4; // 保存档案数量
        private static readonly string JsonFilePath = Path.Combine(
            Application.persistentDataPath, "Saving.json"); // Json文件保存路径
        public static readonly string ScreenShotPath = Path.Combine(
            Application.persistentDataPath, "ScreenShots"); // 截图文件保路径
        
        // ---变量---
        private static SaveDataset _savingDataset; // 保存数据集
        private static Sprite[] _screenshots; // 截图数据集
        
        /// <summary>
        /// 游戏启动初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            // 检测是否存在Json文件，选择是否创建
            if (File.Exists(JsonFilePath)) 
            {
                // 读取Json文件转为变量
                var file = File.ReadAllText(JsonFilePath);
                _savingDataset = JsonUtility.FromJson<SaveDataset>(file);
            }
            else
            {
                // 以全空保存数据集到变量
                _savingDataset = new SaveDataset(Length);
                // 创建Json存储文件
                File.WriteAllText(JsonFilePath,JsonUtility.ToJson(_savingDataset));
            }
            
            // 检测是否存在ScreenShot文件夹，选择是否创建
            if (Directory.Exists(ScreenShotPath))
            { 
                // 赋值空的对象
                _screenshots = new Sprite[Length]; 
                // 遍历文件夹中的图片为Sprite
                var files = Directory.GetFileSystemEntries(ScreenShotPath);
                foreach (var file in files)
                {   
                    // TODO: 注意可能出现的错误
                    // 读取文件的二进制数据
                    var fileData = File.ReadAllBytes(file); 
                    // 创建一个Texture2D对象，分辨率设置为2x2（后面会覆盖）
                    var texture = new Texture2D(2, 2); 
                    // 将二进制数据加载到Texture2D对象中
                    texture.LoadImage(fileData); 
                    // 使用Texture2D对象创建一个Sprite
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    // 给变量赋值
                    if (int.TryParse(Path.GetFileNameWithoutExtension(file), out int result))
                    {
                        _screenshots[result] = sprite;
                    }
                }
            }
            else
            { 
                Directory.CreateDirectory(ScreenShotPath); // 创建文件夹
                _screenshots = new Sprite[Length]; // 赋值空的对象
            }
            // 测试输出
            PrintOut(nameof(Init));
        }
        /// <summary>
        /// 清除存储数据
        /// </summary>
        public static void ClearStorage()
        {
            File.Delete(JsonFilePath);
            Directory.Delete(ScreenShotPath,true);
            // TODO:FIXME: 并没有生成新的存档和改变现有变量的问题
        }
        /// <summary>
        /// 读取返回所有数据。
        /// </summary>
        /// <param name="saveDataset">保存数据集</param>
        /// <param name="screenshots">截图数据集</param>
        public static void Load_all(out SaveDataset saveDataset,out Sprite[] screenshots)
        {
            saveDataset = _savingDataset;
            screenshots = _screenshots;
        }
        /// <summary>
        /// 快速读取
        /// </summary>
        /// <returns>SavaData数据</returns>
        public static void Load(int index,out SaveData saveData, out Sprite sprite)
        {
            if (index >= Length | index < 0)
            {
                Debug.LogError($"<{nameof(SaveAndLoadSystemModel)}>...<{nameof(SaveData)}>...错误的Index输入");
                saveData = null;
                sprite = null;
                return;
            }
            saveData = _savingDataset[index];
            sprite = _screenshots[index];
            return;
        }
        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="scriptIndex">ScriptIndex</param>
        /// <param name="dialogIndex">DialogIndex</param>
        /// <param name="title">脚本标题</param>
        /// <param name="position">描述存储的存档的序列号</param>
        /// <param name="timeStamp">标准Json时钟TimeStamp字符串</param>
        public static void Save(int scriptIndex, int dialogIndex, string title,int position, string timeStamp, Texture2D screenshot)
        {
            // 修改变量
            _savingDataset[position] = new SaveData(scriptIndex, dialogIndex, title, timeStamp);
            // 覆盖保存Json文件
            File.WriteAllText(JsonFilePath,JsonUtility.ToJson(_savingDataset));
            
            // 转为Sprite对象
            var rect = new Rect(0, 0, screenshot.width, screenshot.height);
            var screenshotSprite = Sprite.Create(screenshot, rect, new Vector2(0.5f, 0.5f));
            // 修改变量
            _screenshots[position] = screenshotSprite;
            // 寻找截图文件夹中是否存在目标文件
            var filePath = Path.Combine(ScreenShotPath, $"{position.ToString()}.png");
            if (Directory.Exists(filePath)) File.Delete(filePath); // 删除原始文件
            // 保存图片
            byte[] bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(filePath,bytes);
            // 测试输出
            PrintOut(nameof(Save));
        }
        public static IEnumerator SaveIE(int scriptIndex, int dialogIndex, string title, int position, DateTime time,
            Texture2D screenshot)
        {
            // 修改变量
            _savingDataset[position] = new SaveData(scriptIndex, dialogIndex, title, time.ToString("o"));
            // 覆盖保存Json文件
            File.WriteAllText(JsonFilePath,JsonUtility.ToJson(_savingDataset));
            
            // 转为Sprite对象
            var rect = new Rect(0, 0, screenshot.width, screenshot.height);
            var screenshotSprite = Sprite.Create(screenshot, rect, new Vector2(0.5f, 0.5f));
            // 修改变量
            _screenshots[position] = screenshotSprite;
            // 寻找截图文件夹中是否存在目标文件
            var filePath = Path.Combine(ScreenShotPath, $"{position.ToString()}.png");
            if (Directory.Exists(filePath)) File.Delete(filePath); // 删除原始文件
            // 保存图片
            byte[] bytes = screenshot.EncodeToPNG();
            yield return SaveFileAsync(filePath, bytes);
            // 测试输出
            PrintOut(nameof(Save));
            yield break;
        }
        private static async Task SaveFileAsync(string path, byte[] bytes)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
        /// <summary>
        /// 内部测试用数据打印功能
        /// </summary>
        /// <param name="calloutSource">nameof(方法名)，以告知是哪个方法调用的测试打印</param>
        private static void PrintOut(string calloutSource)
        {
            var debugString = "";
            debugString += $"<{nameof(SaveAndLoadSystemModel)}>...<{calloutSource}>...\nSaving_Path...{Application.persistentDataPath}\n\n";
            debugString += _savingDataset;
            debugString += $"\n\n[[{nameof(_screenshots)}]] - [[Length = {_screenshots.Length}]]\n";
            foreach (var i in _screenshots)
            {
                debugString += $"[{i}] ";
            }
            Debug.Log(debugString);
        }
    }
    public class SaveAndLoadSystem : CavansGroupBehaviour
    {
        [SerializeField] private float fadeTime;
        [SerializeField] private SaveAndLoadSystem_Block autoSaveBlock;
        private static float FadeTime
        {
            get
            {
                return _instance.fadeTime;
            }
        }
        private static SaveAndLoadSystem _instance;
        private static SaveAndLoadSystem Instance
        {
            get
            {
                if (_instance is null) Debug.LogError($"<{nameof(SaveAndLoadSystem)}>...单例模式错误");
                return _instance;
            }
            set
            {
                if (_instance is not null) Debug.LogError($"<{nameof(SaveAndLoadSystem)}>...单例模式错误");
                _instance = value;
            }
        }

        public static Texture2D Screenshot { get; private set; }

        // 变量
        public static bool IsSaving { get; set; } = false;

        private void Awake()
        {
            Instance = this;
            Out(); // 初始不可见
        }
        /// <summary>
        /// 开启
        /// </summary>
        /// <param name="isSaving"></param>
        public static void Open(bool isSaving)
        {
            Instance.StartCoroutine(Instance.Open_IE(isSaving));
        }

        private IEnumerator Open_IE(bool isSaving)
        {
            if (isSaving)
            {
                yield return new WaitForEndOfFrame();
                Screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            }
            _instance.FadeIn(FadeTime);
            IsSaving = isSaving;
        }
        public static void Open(string key)
        {
            var isSaving = false;
            if (key.ToLower() == "save")
            {
                Screenshot = ScreenCapture.CaptureScreenshotAsTexture();
                isSaving = true;
            }
            else if(key.ToLower() == "load")
            {
                isSaving = false;
            }
            else
            {
                Debug.LogError($"<{nameof(SaveAndLoadSystem)}>...<{nameof(Open)}>...key格式错误");
                return;
            }
            _instance.FadeIn(FadeTime);
            IsSaving = isSaving;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public static void Close()
        {
            _instance.FadeOut(FadeTime);
        }
        /// <summary>
        /// 快速保存
        /// </summary>
        public static void QuickSave()
        {
            Instance.StartCoroutine(QuickSave_IE());
        }

        public static void QuickLoad()
        {
            SaveAndLoadSystemModel.Load(0,out var result,out var sprite);
            if (!result.hasData)
            {
                MessageWindowManager.Message("无保存数据");
                return;
            }
            ConfirmWindow.Confirm(() => {SaveAndLoadSystem.Close();
                AVG_System.Scripts.AVG_System.BeginAuto(true,result.scriptIndex,result.dialogIndex,Title.In);
            },"确认要读取吗。");
        }
        private static IEnumerator QuickSave_IE()
        {
            yield return new WaitForEndOfFrame();
            var screenShot = ScreenCapture.CaptureScreenshotAsTexture();
            // 模型层
            var timeNow = DateTime.UtcNow;
            yield return SaveAndLoadSystemModel.SaveIE(
                scriptIndex:ScriptsManager.Now,
                dialogIndex:Scripts.AVG_System.FraseNow,
                title:ScriptsManager.NowTitle,
                position:0,
                time: timeNow,
                screenshot: screenShot
            );
            //视觉层
            var sprite = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), 
                new Vector2(0.5f, 0.5f));
            Instance.autoSaveBlock.Renew(
                title:ScriptsManager.NowTitle,
                time:timeNow,
                sprite:sprite
                );
            //返回信息
            MessageWindowManager.Message("已经快速保存。");
        }
    }
}