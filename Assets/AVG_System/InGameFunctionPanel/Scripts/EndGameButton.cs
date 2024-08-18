using AVG_System.Scripts.Buttons;
using UnityEngine;
namespace AVG_System.IngameFunctionPanel.Scripts
{
    public class EndGameButton:ButtonBehaviour
    {
        protected override void Onclick()
        {
            ConfirmWindow.Confirm(
                () =>
                {
                    Debug.Log("游戏结束");
                    Application.Quit();
                },
                "是否结束游戏？"
                );
        }
    }
}