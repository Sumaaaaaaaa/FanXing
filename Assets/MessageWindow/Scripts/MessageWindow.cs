using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    public string message;
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float imageAddLength = 15 ;
    public float fadeinTime;
    public float waitingTime;
    public float fadeoutTime;
    private void Awake()
    {
        StartCoroutine(Show());
    }

    private IEnumerator Show()
    {
        image.enabled = text.enabled = false;
        yield return new WaitForEndOfFrame();
        image.enabled = text.enabled = true;
        // 改变文字
        Text = message;
        // 设置外框大小适配文字
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            text.preferredWidth+imageAddLength);
        // 先设置图像右侧卡在屏幕外不显示
        var startPosition = -image.rectTransform.rect.width;
        image.rectTransform.anchoredPosition = new Vector2(-image.rectTransform.rect.width, 
                                                            image.rectTransform.anchoredPosition.y);
        yield return null;
        // 开始FadeIn
        var startTime = Time.time;
        while (Time.time - startTime < fadeinTime)
        {
            var value = Mathf.Lerp(startPosition, 0, easeOutCubic((Time.time - startTime) / fadeinTime));
            image.rectTransform.anchoredPosition = new Vector2(value, image.rectTransform.anchoredPosition.y);
            yield return null;
        }
        // 结束修正
        image.rectTransform.anchoredPosition = new Vector2(0, image.rectTransform.anchoredPosition.y);

        // 等待时间
        yield return new WaitForSeconds(waitingTime);
        
        // 开始FadeOut
        startTime = Time.time;
        while (Time.time - startTime < fadeoutTime)
        {
            var value = easeOutCubic(Mathf.Lerp(1.0f, 0.0f, (Time.time - startTime) / fadeoutTime));
            canvasGroup.alpha = value;
            yield return null;
        }
        // 结束修正
        canvasGroup.alpha = 0.0f;
        yield return null;
        Destroy(gameObject);
        /*
        */
        yield break;
    }
    private string Text
    {
        set => text.text = value;
    }

    private static float easeOutCubic(float x)
    {
        //https://easings.net/zh-cn#easeOutCubic
        return 1 - Mathf.Pow(1 - x, 3);
    }
}
