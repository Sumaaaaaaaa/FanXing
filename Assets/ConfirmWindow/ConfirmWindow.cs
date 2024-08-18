using System;
using System.Collections;
using System.Collections.Generic;
using AVG_System.Scripts.Interface;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmWindow : CavansGroupBehaviour
{
    // 子组件
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Text _text;

    private static ConfirmWindow _instance = null;
    // 设定值
    private const float DurationTime = 0.6f;
    private void Awake()
    {
        if (_instance is not null) Debug.LogError($"<{nameof(ConfirmWindow)}>...单例模式错误");
        _instance = this;
        cancelButton.onClick.AddListener(Onclick_Cancel);
        Out();
    }

    private void Confirm_instance(Action action,string showText)
    {
        _text.text = showText;
        this.enabled = true;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => { 
            action();
            this.enabled = false;
        });
    }
    public static void Confirm(Action action, string showText)
    {
        _instance.Confirm_instance(action,showText);
    }
    private void Onclick_Cancel()
    {
        this.enabled = false;
    }
    
    private void OnEnable()
    {
        FadeIn(DurationTime);
    }
    private void OnDisable()
    {
        FadeOut(DurationTime);
    }
}
