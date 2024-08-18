using AVG_System.IngameFunctionPanel.Scripts;
using UnityEngine;
namespace AVG_System.Scripts.Buttons
{
    public class PauseButton:ButtonBehaviour
    {
        [SerializeField] private InGameFunctionPanel inGameFunctionPanel;
        protected override void Onclick()
        {
            inGameFunctionPanel.Open();
        }
    }
}