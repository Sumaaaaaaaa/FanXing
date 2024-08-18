using UnityEngine;
namespace AVG_System.Scripts.Buttons
{
    public class QuickLoadButton : ButtonBehaviour
    {
        protected override void Onclick()
        {
            SaveAndLoadSystem.SaveAndLoadSystem.QuickLoad();
        }
    }
}