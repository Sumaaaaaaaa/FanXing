using System.Collections;
using UnityEngine;

namespace AVG_System.Scripts
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class DialogBackGround_Script : MonoBehaviour
    {
        private UnityEngine.UI.Image _image; // 获取图像组件
        private int _fadeState = 2;
        // 0-隐藏, 1-正在fadeTo显示, 2-显示, -1:正在fadeTo隐藏，用于让协程动画不会因为重复添加而叠加错乱
        private float _ori_alpha; // 记录原始图片的alpha
        private Coroutine _coroutine;

        private void Awake()
        {
            _image = GetComponent<UnityEngine.UI.Image>();
            _ori_alpha = _image.color.a;

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
        }
        /// <summary>
        /// 跳过动画
        /// </summary>
        public void Skip()
        {
            if(!isAnimation)
            {
                Debug.LogError("当尝试Skip()时，该对象并不在处理Fade。");
            }
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
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
        private IEnumerator Fade_IE(bool isFadeOut)
        {
            float startTime = Time.time;

            while (Time.time - startTime < AVG_System.fade_time)
            {
            
                float alpha;
                if(isFadeOut)
                {
                    alpha = _ori_alpha * (1 - (Time.time - startTime) / AVG_System.fade_time);
                }
                else
                {
                    alpha = (Time.time - startTime) / AVG_System.fade_time * _ori_alpha;
                }
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
                yield return null;
            }

            if (isFadeOut)
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.0f);
                _fadeState = 0;
            }
            else
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _ori_alpha);
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
        public void Hide()
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.0f);
            _fadeState = 0;
        }
        /// <summary>
        /// 直接完全显示对话框
        /// </summary>
        private void Show()
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _ori_alpha);
            _fadeState = 2;
        }
    }
}
