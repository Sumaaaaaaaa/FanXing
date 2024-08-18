using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AVG_System.Scripts.Interface;

public class Title : CavansGroupBehaviour
{
    [SerializeField] private float fadeTime;
    // 单例模式
    private static Title _instance;
    public static Title Instance
    {
        get
        {
            if (_instance is not null) return _instance;
            Debug.LogError($"<{nameof(Title)}>...单例模式错误...没有对象");
            return null;

        }
        set
        {
            if (_instance is null)
            {
                _instance = value;
                return;
            }
            Debug.LogError($"<{nameof(Title)}>...单例模式错误...对象重复出现");
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public new static void Out()
    {
        Instance.FadeOut(Instance.fadeTime);
    }

    public static void In()
    {
        Instance.FadeIn(Instance.fadeTime);
    }
}
