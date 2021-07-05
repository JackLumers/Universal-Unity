using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FocusableView
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
                InteractionBlock("Unfocused", !value, true);
            }
        }
    }
}