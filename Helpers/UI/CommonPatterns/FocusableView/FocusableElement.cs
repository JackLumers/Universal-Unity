using Common.Helpers.UI.BaseUiElements;

namespace Common.Helpers.UI.CommonPatterns.FocusableView
{
    public class FocusableElement : BaseInteractableUiElement
    {
        private bool _focused = false;

        public bool Focused
        {
            get { return _focused; }
            set
            {
                _focused = value;
                RayCastBlock("Unfocused", !value, true);
            }
        }
    }
}