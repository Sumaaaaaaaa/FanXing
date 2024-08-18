using AVG_System.Scripts.Buttons;
using UnityEngine;
namespace AVG_System.IngameFunctionPanel.Scripts
{
    public class BackToTitleButton : ButtonBehaviour
    {
        [SerializeField] private InGameFunctionPanel _inGameFunctionPanel;
        protected override void Onclick()
        {
            ConfirmWindow.Confirm(() => 
            {
                AVG_System.Scripts.AVG_System.ForceAllEnd();
                _inGameFunctionPanel.Close(); 
            }
                ,"确认是否回到主菜单？");
            
        }
    }
}