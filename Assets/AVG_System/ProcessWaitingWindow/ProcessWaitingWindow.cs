using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessWaitingWindow : MonoBehaviour
{
    private static ProcessWaitingWindow _instance;

    private static ProcessWaitingWindow Instance
    {
        get
        {
            if (_instance is not null) return _instance;
            Debug.LogError($"<{nameof(ProcessWaitingWindow)}>...单例模式错误...没有单例");
            return null;
        }
        set
        {
            if (_instance is null)
            {
                _instance = value;
                return;
            }
            Debug.LogError($"<{nameof(ProcessWaitingWindow)}>...单例模式错误...单例重复");
        }
    }
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public static void SetActive(bool active)
    {
        Instance.gameObject.SetActive(active);
    }
}
