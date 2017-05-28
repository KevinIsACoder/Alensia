using Alensia.Core.Control;
using Alensia.Core.Input;
using Alensia.Core.UI;

namespace Alensia.Demo.Controller
{
    public class DemoControl : GameControl
    {
        public DemoControl(IUIManager uiManager, IInputManager inputManager) :
            base(uiManager, inputManager)
        {
        }

        protected override IComponent CreateMainMenu() => new MainMenu(UIManager);
    }
}