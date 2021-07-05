using System.Collections;
using Common.Helpers.UI.BaseUiElements;

namespace Common.Helpers.UI.CommonPatterns.AnimatedElements
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