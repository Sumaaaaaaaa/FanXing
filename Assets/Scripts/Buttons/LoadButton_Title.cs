using AVG_System.SaveAndLoadSystem;

namespace Buttons
{
    using UnityEngine.UI;
    using UnityEngine;
    public class LoadButton_Title : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            SaveAndLoadSystem.Open("Load");
        }
    }
}
