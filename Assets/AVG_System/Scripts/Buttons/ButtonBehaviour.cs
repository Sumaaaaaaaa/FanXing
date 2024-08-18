using System;
using UnityEngine.UI;
using UnityEngine;
namespace AVG_System.Scripts.Buttons
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonBehaviour : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Onclick);
        }

        protected abstract void Onclick();
    }
}