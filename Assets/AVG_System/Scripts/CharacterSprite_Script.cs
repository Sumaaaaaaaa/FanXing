using System;
using System.Collections;
using AVG_System.Scripts.OutSide;
using UnityEngine;
using UnityEngine.UI;

namespace AVG_System.Scripts
{
    [RequireComponent(typeof(Image))]
    public class CharacterSprite_Script : MonoBehaviour
    {
        [SerializeField] private TableBase<string, Sprite,KeyAndValue<string, Sprite>> _emotions;
        // 子组件
        private Image _image;
        private RectTransform _rectTransform;

        private float _ori_alpha;
        private int _fadeState = 0;
        // 0-隐藏, 1-正在fadeTo显示, 2-显示, -1:正在fadeTo隐藏，用于让协程动画不会因为重复添加而叠加错乱
        private Coroutine _coroutine;
        // 移动功能
        private Coroutine _moveCoroutine;
        private float _endPosition;
        

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _ori_alpha = _image.color.a;
            Hide();
            FadeIn();
        }
   
        /// <summary>
        /// 直接隐藏角色
        /// </summary>
        public void Hide() 
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
            _fadeState = 0;
        }
        private void Show()
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1.0f);
            _fadeState = 2;
        }
        /// <summary>
        /// 缓动进入
        /// </summary>
        private void FadeIn() 
        {
            if (_fadeState == 0)
            {
                _coroutine = StartCoroutine(Fade_IE(false));
                _fadeState = 1;
            }
            return;
        }
        /// <summary>
        /// 缓动隐藏
        /// </summary>
        private void FadeOut() 
        {
            if (_fadeState == 2)
            {
                _coroutine = StartCoroutine(Fade_IE(true));
                _fadeState = -1;
            }
            return;
        }
        public void Skip()
        {
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

            if (_moveCoroutine is not null)
            {
                SkipMove();
            }
        }
        private IEnumerator Fade_IE(bool isFadeOut)
        {
            float startTime = Time.time;

            while (Time.time - startTime < AVG_System.fade_time)
            {

                float alpha;
                if (isFadeOut)
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
                Destroy(gameObject);
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
        public bool _isAnimation
        {
            get
            {
                return Math.Abs(_fadeState) == 1 | _moveCoroutine is not null;
            }
        }
        /// <summary>
        /// 通过字符串切换表情（Sprite）
        /// </summary>
        /// <param name="_emotion"></param>
        public void Emotion(Sprite sprite)
        {
            _image.sprite = sprite;
        }
        public void Out()
        {
            FadeOut();
        }

        public void MoveTo(float position, float time)
        {
            _moveCoroutine = StartCoroutine(Move_IE(position, time));
        }

        private void SkipMove()
        {
            if (_moveCoroutine is null)
            {
                Debug.LogError($"<{nameof(CharacterSprite_Script)}>...<{nameof(SkipMove)}>......未在进行移动，非法操作");
                return;
            }
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
            _rectTransform.anchoredPosition = new Vector2(_endPosition, 0);
        }

        private IEnumerator Move_IE(float targetPosition, float durationTime)
        {
            var startTime = Time.time;
            var startPosition = _rectTransform.anchoredPosition;
            var endPosition = new Vector2(targetPosition,0);
            _endPosition = targetPosition;
            while (Time.time - startTime < durationTime)
            {
                var t = ((Time.time - startTime) / durationTime);
                t = 1 - Mathf.Pow(2, -10 * t);
                _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
                yield return null;
            }
            _rectTransform.anchoredPosition = endPosition;
            _moveCoroutine = null;
            yield break;
        }
    }
}
