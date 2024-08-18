using System.Collections;
using TMPro;
using UnityEngine;

namespace AVG_System.Scripts
{
    [RequireComponent(typeof(TMP_Text))]
    public class Dialog_Name_Script : MonoBehaviour
    {
        private TMP_Text _tmpText; // 获取文本组件
        private int _fadeState = 2; 
        // 0-隐藏, 1-正在fadeTo显示, 2-显示, -1:正在fadeTo隐藏，用于让协程动画不会因为重复添加而叠加错乱
        private Coroutine _coroutine;

        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            // 初始清空
            Clear();
        }
    
        /// <summary>
        /// 窗口缓动进入
        /// </summary>
        public void FadeIn()
        {
            if (_fadeState == 0)
            {
                _coroutine = StartCoroutine(Fade_IE(false));
                _fadeState = 1;
            }
            return;
        }
        /// <summary>
        /// 窗口缓动隐藏
        /// </summary>
        public void FadeOut() 
        {
            if (_fadeState == 2)
            {
                _coroutine = StartCoroutine(Fade_IE(true));
                _fadeState = -1;
            }
            return;
        }
        /// <summary>
        /// 跳过动画
        /// </summary>
        public void Skip()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            if (_fadeState == -1)
            {
                Hide();
            }
            else if (_fadeState == 1)
            {
                Show();
            }
        }
        // 缓动协程方法
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
            _coroutine = null;
            yield break;
        }
    
        /// <summary>
        /// 是否在动画
        /// </summary>
        public bool isAnimation
        {
            get
            {
                return (_fadeState == 1) | (_fadeState == -1);
            }
        }

        /// <summary>
        /// 直接完全隐藏对话框
        /// </summary>
        private void Hide()
        {
            _tmpText.color = new Color(_tmpText.color.r, _tmpText.color.g, _tmpText.color.b, 0.0f);
            _fadeState = 0;
        }
        /// <summary>
        /// 直接完全显示对话框
        /// </summary>
        private void Show()
        {
            _tmpText.color = new Color(_tmpText.color.r, _tmpText.color.g, _tmpText.color.b, 1.0f);
            _fadeState = 2;
        }

        /// <summary>
        /// 修改名字显示字符
        /// </summary>
        public string Text
        {
            set
            {
                _tmpText.text = value;
                _tmpText.ForceMeshUpdate();
            }
        }

        /// <summary>
        /// 清空名字所有文字
        /// </summary>
        public void Clear()
        {
            _tmpText.text = "";
            _tmpText.ForceMeshUpdate();
        }

    }
}
