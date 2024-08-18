using AVG_System.Scripts.Buttons;
using Clean_Settings_UI.Scripts;
using UnityEngine;
namespace AVG_System.IngameFunctionPanel.Scripts
{
    public class SettingWindowButton : ButtonBehaviour
    {
        protected override void Onclick()
        {
            SettingPanel.Open();
        }
    }
}