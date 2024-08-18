using AVG_System.Scripts.Buttons;
using UnityEngine;
namespace AVG_System.IngameFunctionPanel.Scripts
{
    public class BackToGameButton :ButtonBehaviour
    {
        [SerializeField] private InGameFunctionPanel _inGameFunctionPanel;
        protected override void Onclick()
        {
            _inGameFunctionPanel.Close();
        }
    }
}