using AVG_System.Scripts.Buttons;

namespace AVG_System.SaveAndLoadSystem
{
    public class SorLPanelCloseButton : ButtonBehaviour
    {
        protected override void Onclick()
        {
            SaveAndLoadSystem.Close();
        }
    }
}