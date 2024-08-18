using System.Collections;
using UnityEngine;

namespace AVG_System.Scripts.Interface
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CavansGroupBehaviour : MonoBehaviour
    {
        private Coroutine _fadeCoroutine = null;
        private static IEnumerator FadeIE(CanvasGroup canvasGroup,bool isFadeOut,float durationTime)
        {
            var startTime = Time.time;
            var targetAlpha = isFadeOut ? 0.0f : 1.0f;
            var startAlpha = canvasGroup.alpha;
            while (Time.time - startTime < durationTime)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, (Time.time - startTime) / durationTime);
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        public void FadeIn(float durationTime)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            if (_fadeCoroutine is not null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeIE(GetComponent<CanvasGroup>(), false,durationTime));
        }

        public void FadeOut(float durationTime)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
            if (_fadeCoroutine is not null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeIE(GetComponent<CanvasGroup>(), true,durationTime));
        }

        protected void Out()
        {
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
            GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}