using UnityEngine;
namespace AVG_System.Scripts.Buttons
{
    public class OpenSorLPanelButtonSave : ButtonBehaviour
    {
        protected override void Onclick()
        {
            SaveAndLoadSystem.SaveAndLoadSystem.Open(isSaving:true);
        }
    }
}