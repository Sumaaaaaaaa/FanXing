using System.Collections;
using TMPro;
using UnityEngine;

namespace AVG_System.Scripts
{
    [RequireComponent(typeof(TMP_Text))]
    public class Dialog_Script : MonoBehaviour
    {
        private TMP_Text _tmpText; // 获取文本组件
        private int _totalVisibleCharacters = 0; // 目前可见的字符数
        private Coroutine _loadText_ie = null; // 文本逐个出现协程
        private Coroutine _animation = null; // Fade动画
        private int _fadeState = 2;
        // 0-隐藏, 1-正在fadeTo显示, 2-显示, -1:正在fadeTo隐藏，用于让协程动画不会因为重复添加而叠加错乱

        /// <summary>
        /// 返回是否正在逐个显示字体
        /// </summary>
        public bool IsAnimation
        {
            get { return (_loadText_ie is not null) || (_animation is not null); }
        }

        // 初始化
        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            // 初始状态清空
            Clear();
        }

        /// <summary>
        /// 窗口缓动进入
        /// </summary>
        public void FadeIn() 
        {
            if(_fadeState == 0 )
            {
                _animation = StartCoroutine(Fade_IE(false));
                _fadeState = 1;
            }
            return;
        }

        /// <summary>
        /// 窗口缓动隐藏
        /// </summary>
        public void FadeOut() 
        {
            if (_fadeState == 2 )
            {
                _animation = StartCoroutine(Fade_IE(true));
                _fadeState = -1;
            }
            return;
        }

        /// <summary>
        /// 直接完全隐藏对话框
        /// </summary>
        private void Hide()
        {
            _tmpText.color = new Color(_tmpText.color.r, _tmpText.color.g, _tmpText.color.b, 0.0f);
            _fadeState = 0;
        }
        private void Show()
        {
            _tmpText.color = new Color(_tmpText.color.r, _tmpText.color.g, _tmpText.color.b, 1.0f);
            _fadeState = 2;
        }
        private IEnumerator Fade_IE(bool isFadeOut)
        {
            float startTime = Time.time;

            while (Time.time - startTime < AVG_System.fade_time)
            {
                float alpha;
                alpha = isFadeOut ? 
                    1 - (Time.time - startTime) / AVG_System.fade_time 
                    : (Time.time - startTime) / AVG_System.fade_time;
                _tmpText.color = new Color(_tmpText.color.r, _tmpText.color.g, _tmpText.color.b, alpha);
                yield return null;
            }

            if (isFadeOut) 
            {
                _tmpText.color = new Color(_tmpText.color.r, _tmpText.color.g, _tmpText.color.b, 0.0f);
                _fadeState = 0;
            }
            else
            {
                _tmpText.color = new Color(_tmpText.color.r, _tmpText.color.g, _tmpText.color.b, 1.0f);
                _fadeState = 2;
            }
            _animation = null;
            yield break;
        }
    
    
        /// <summary>
        /// 开始逐字显示一段文字
        /// </summary>
        public string Text
        {
            set
            {
                if (_loadText_ie is not null)
                {
                    StopCoroutine(_loadText_ie);
                }
                _tmpText.text = value;
                _totalVisibleCharacters = 0;
                _tmpText.maxVisibleCharacters = _totalVisibleCharacters;
                _tmpText.ForceMeshUpdate();
                _loadText_ie = StartCoroutine(ShowTextByCharacter());
                return;
            }
        }
    
        // 逐字显示协程
        private IEnumerator ShowTextByCharacter()
        {
            while (_totalVisibleCharacters <= _tmpText.text.Length)
            {
                yield return new WaitForSecondsRealtime(AVG_System.speed_character);
                _totalVisibleCharacters++;
                _tmpText.maxVisibleCharacters = _totalVisibleCharacters;
                _tmpText.ForceMeshUpdate();
            }
            _loadText_ie = null;
            yield break;
        }


        /// <summary>
        /// 直接显示到最后  
        /// </summary>
        public void Skip() 
        {
        
            if ((_loadText_ie is null) && (_animation is null))
            {
                Debug.LogError("当尝试Skip()时，该对象并不在处理Fade。");
                return;
            }
            // 逐字部分
            if(_loadText_ie is not null)
            {
                StopCoroutine(_loadText_ie);
                _loadText_ie = null;
                _tmpText.maxVisibleCharacters = _tmpText.text.Length;
                _tmpText.ForceMeshUpdate();
            }

            //动画部分
            if (_animation is null) return;
            StopCoroutine(_animation);
            _animation = null;
            if (_fadeState == -1)
            {
                Hide();
            }
            else if (_fadeState == 1)
            {
                Show();
            }
            return;
        }


        /// <summary>
        /// 清空对话框所有的文字
        /// </summary>
        public void Clear()
        {
            _tmpText.text = "";
            _tmpText.ForceMeshUpdate();
            if (_loadText_ie is not null)
            {
                StopCoroutine(_loadText_ie);
                _loadText_ie = null;
            }
        
        }
        // 测试用
    }
}