using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AVG_System.Scripts
{
    [RequireComponent(typeof(Image),typeof(CanvasGroup))]
    public class Log : MonoBehaviour
    {
        private Coroutine _coroutine;
        private int _fadeState = 0;

        [SerializeField] private GameObject _OneDialogInLog;
        [SerializeField] private Transform _content;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            // 开始时先隐藏图层
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0.0f;
        }
        // 清空
        public void Clear()
        {
            for(int i = 0; i< _content.transform.childCount; i++)
            {
                Destroy(_content.transform.GetChild(i).gameObject);
            }
        }
        // 增加一句
        public void Add(string name, string dialog, System.Action<string> audioAction = null, string audioPath = "")
        {
            GameObject target = Instantiate(_OneDialogInLog, _content);
            target.GetComponent<OneDialogInLog>().name = name;
            target.GetComponent<OneDialogInLog>().dialog = dialog;
            target.GetComponent<OneDialogInLog>().AudioAction = audioAction;
            target.GetComponent<OneDialogInLog>().AudioPath = audioPath;
        }
        // 自身的缓动进入和缓动出
        private IEnumerator Fade_IE(bool isFadeOut)
        {
            var startTime = Time.time;
            var fromValue = _canvasGroup.alpha;
            var toValue = isFadeOut ? 0.0f : 1.0f;
            while (Time.time - startTime < AVG_System.fade_time)
            {
                _canvasGroup.alpha = Mathf.Lerp(fromValue, toValue, (Time.time - startTime) / AVG_System.fade_time);
                yield return null;
            }
            _canvasGroup.alpha = isFadeOut ? 0.0f : 1.0f;
            _fadeState = isFadeOut ? 0 : 2;
            yield break;
            //_coroutine = null;
        }
        // 缓动入
        public void FadeIn()
        {
            // 阻挡鼠标
            _canvasGroup.blocksRaycasts = true;
            // 将ScrollView转到底部
            GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
            
            // 动画处理
            if (_coroutine is not null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Fade_IE(false));
            
            /*
            if (_fadeState == 0)
            {
                _coroutine = StartCoroutine(Fade_IE(false));
                _fadeState = 1;
            }
            return;
            */
        }
        // 缓动出
        public void FadeOut()
        {
            // 不阻挡鼠标
            _canvasGroup.blocksRaycasts = false;
            
            // 动画效果
            if (_coroutine is not null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Fade_IE(true));
            
            /*
            if (_fadeState == 2)
            {
                _coroutine = StartCoroutine(Fade_IE(true));
                _fadeState = -1;
            }
            else
            {
                return;
            }
            return;
            */
        }
    }
}
