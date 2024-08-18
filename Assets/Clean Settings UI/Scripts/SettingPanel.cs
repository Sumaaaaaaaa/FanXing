using System;
using UnityEngine;
using AVG_System.Scripts.Interface;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using UnityEngine.UI;

namespace Clean_Settings_UI.Scripts
{
    public class SettingPanel : CavansGroupBehaviour
    {
        [SerializeField] private UnityEngine.UI.ScrollRect scrollRect;
        [SerializeField] private float fadeTime;
        private static SettingPanel _instance;
        private static SettingPanel Instance
        {
            get
            {
                if (_instance is not null) return _instance;
                Debug.LogError($"<{nameof(SettingPanel)}>...单例错误...没有单例");
                return null;
            }
            set
            {
                if (_instance is null)
                {
                    _instance = value;
                    return;
                }
                Debug.LogError($"<{nameof(SettingPanel)}>...单例错误...单例重复");
                return;
            }
        }
        private void Awake()
        {
            Instance = this;
            Out();
        }

        public static void Open()
        {
            Instance.FadeIn(Instance.fadeTime);
            Instance.scrollRect.verticalNormalizedPosition = 1;
        }

        public void Close()
        {
            Instance.FadeOut(Instance.fadeTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Open();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Close();
            }
        }
    }
}