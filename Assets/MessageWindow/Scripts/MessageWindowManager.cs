using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MessageWindowManager : MonoBehaviour
{
    [SerializeField] private GameObject messageWindowPrehab;
    private static MessageWindowManager _instance;
    public float fadeinTime;
    public float waitingTime;
    public float fadeoutTime;

    private void Awake()
    {
        if (_instance is not null)
        {
            Debug.LogError($"<{nameof(MessageWindowManager)}>...单例模式错误");
        }
        _instance = this;
    }

    public static void Message(string message)
    {
        var target = Instantiate(_instance.messageWindowPrehab, _instance.transform);
        var component = target.GetComponent<MessageWindow>();
        component.fadeinTime = _instance.fadeinTime;
        component.waitingTime = _instance.waitingTime;
        component.fadeoutTime = _instance.fadeoutTime;
        component.message = message;
        return;
    }
}
