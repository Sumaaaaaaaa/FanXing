using UnityEngine;
namespace AVG_System.Scripts.Buttons
{
    public class OpenSorLPanelButtonLoad :ButtonBehaviour
    {
        protected override void Onclick()
        {
            SaveAndLoadSystem.SaveAndLoadSystem.Open(isSaving:false);
        }
    }
}