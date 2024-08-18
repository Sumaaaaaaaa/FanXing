using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AVG_System.Scripts
{
    [RequireComponent(typeof(Slider))]
    public class AudioSliderController : MonoBehaviour,IPointerUpHandler
    {
        private Slider _slider;
        [SerializeField] private AudioSystemTarget controllerTarget;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.value = AudioSystem.GetLoudness(controllerTarget);
            _slider.onValueChanged.AddListener(
                (value) => {AudioSystem.SetLoudness(controllerTarget,value);}
            );
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AudioSystem.Save();
        }
        
    }
}