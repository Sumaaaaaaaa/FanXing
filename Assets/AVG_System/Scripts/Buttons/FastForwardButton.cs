using UnityEngine;
using UnityEngine.EventSystems;

namespace AVG_System.Scripts.Buttons
{
    public class FastForwardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            AVG_System.StartFastForward();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AVG_System.EndFastForward();
        }
    }
}
