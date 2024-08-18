using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AVG_System.Scripts
{
    public class BackGround_Script : MonoBehaviour
    {
        private Image _higherLayer;
        private Image _lowerLayer;
        private bool _enabled = false;
        private Coroutine _animation = null;
        private bool _isFadeOut = false;

        private void Awake()
        {
            // 获取底部的两个层
            _lowerLayer = transform.GetChild(0).GetComponent<Image>();
            _higherLayer = transform.GetChild(1).GetComponent<Image>();
            // 将两个图层的透明度转为0
            _higherLayer.color = new Color(_higherLayer.color.r, _higherLayer.color.g, _higherLayer.color.b, 0.0f);
            _lowerLayer.color = new Color(_lowerLayer.color.r, _lowerLayer.color.g, _lowerLayer.color.b, 0.0f);
        }

        public void Goto(Sprite sprite)
        {
            if (sprite is null)
            {
                // 退出
                _lowerLayer.color = new Color(_lowerLayer.color.r, _lowerLayer.color.g, _lowerLayer.color.b, 0f);
                if (_higherLayer.color.a != 0)
                {
                    _animation = StartCoroutine(Fade_IE(true, _higherLayer));
                }
                _enabled = false;
                _isFadeOut = true;
                return;
            }
            else
            {
                if (_enabled)
                {
                    // 切换
                    _lowerLayer.sprite = _higherLayer.sprite;
                    _lowerLayer.color = new Color(_lowerLayer.color.r, _lowerLayer.color.g, _lowerLayer.color.b, 1.0f);
                    _higherLayer.sprite = sprite;
                    _higherLayer.color = new Color(_higherLayer.color.r, _higherLayer.color.g, _higherLayer.color.b, 0.0f);
                    _animation = StartCoroutine(Fade_IE(false, _higherLayer));
                    _isFadeOut = false ;
                }
                else
                {
                    // 进入
                    _higherLayer.sprite = sprite;
                    _animation = StartCoroutine(Fade_IE(false,_higherLayer));
                    _enabled = true;
                    _isFadeOut = false;
                    return;
                }
            }
        }
        public bool IsAnimation
        {
            get
            {
                return _animation is not null;
            }
        }
        public void Skip()
        {
            if (_animation is null)
            {
                Debug.LogError("并没有正在Fade中的动画");
                return;
            }
            StopCoroutine(_animation);
            _animation = null;
            if (_isFadeOut) 
            {
                _higherLayer.color = new Color(_lowerLayer.color.r, _lowerLayer.color.g, _lowerLayer.color.b, 0f);
            }
            else
            {
                _higherLayer.color = new Color(_lowerLayer.color.r, _lowerLayer.color.g, _lowerLayer.color.b, 1f);
            }

        }
        private IEnumerator Fade_IE(bool isFadeOut, Image target)
        {
            float startTime = Time.time;

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
                target.color = new Color(target.color.r, target.color.g, target.color.b, alpha);
                yield return null;
            }

            if (isFadeOut)
            {
                target.color = new Color(target.color.r, target.color.g, target.color.b, 0.0f);
            }
            else
            {
                target.color = new Color(target.color.r, target.color.g, target.color.b, 1.0f);
            }
            _animation = null;
            yield break;
        }
    }
}
