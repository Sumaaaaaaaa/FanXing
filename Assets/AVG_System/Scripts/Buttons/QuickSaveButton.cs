using UnityEngine;
namespace AVG_System.Scripts.Buttons
{
    public class QuickSaveButton : ButtonBehaviour
    {
        protected override void Onclick()
        {
            SaveAndLoadSystem.SaveAndLoadSystem.QuickSave();
        }
    }
}