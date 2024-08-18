using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AVG_System.Scripts
{
    public class OneDialogInLog : MonoBehaviour
    {
        // 静态保存所有对象
        static private List<OneDialogInLog> oneDialogInLogs = new List<OneDialogInLog>();
        static public OneDialogInLog[] AllObjects
        {
            get 
            {
                return oneDialogInLogs.ToArray();
            }
        }
        private void Awake()
        {
            oneDialogInLogs.Add(this);
        }
        private void OnDestroy()
        {
            oneDialogInLogs.Remove(this);
        }

        // 子对象指定
        [SerializeField] private GameObject _Go_name;
        [SerializeField] private GameObject _Go_dialog;

        // 内存储数据
        private System.Action<string> audioAction = null;
        private string _audioPath = "";

        // 对对象的调用
        public string name
        {
            set
            {
                _Go_name.GetComponent<TextMeshProUGUI>().text = value;
            }
            get
            {
                return _Go_name.GetComponent<TextMeshProUGUI>().text;
            }
        }
        public string dialog
        {
            set
            {
                _Go_dialog.GetComponent<TextMeshProUGUI>().text = value;
            }
            get
            {
                return _Go_dialog.GetComponent<TextMeshProUGUI>().text;
            }
        }
        public System.Action<string> AudioAction
        {
            set
            {
                audioAction = value;
            }
        }
        public string AudioPath
        {
            set
            {
                _audioPath = value;
            }
        }
        // 按钮触发音频播放
        public void PlayAudio()
        {
            if (_audioPath == "")
            {
                return;
            }
            audioAction(_audioPath);
        }
    }
}
