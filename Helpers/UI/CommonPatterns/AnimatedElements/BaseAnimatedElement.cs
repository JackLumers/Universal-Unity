using System.Collections;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.AnimatedElements
{
    public abstract class BaseAnimatedElement : BaseUiElement
    {
        private void OnEnable()
        {
            StartCoroutine(AnimationProcess());
        }

        protected abstract IEnumerator AnimationProcess();
    }
}