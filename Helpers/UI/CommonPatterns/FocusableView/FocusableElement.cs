using UniversalUnity.Helpers.UI.BaseUiElements;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FocusableView
{
    public class FocusableElement : BaseInteractableUiElement
    {
        private bool _focused = false;

        public bool Focused
        {
            get => _focused;
            set
            {
                _focused = value;
                InteractionBlock("Unfocused", !value, true);
            }
        }
    }
}