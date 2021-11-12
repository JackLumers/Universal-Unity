using System.Threading;
using Cysharp.Threading.Tasks;
using UniversalUnity.Helpers.UI.BaseUiElements;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.AnimatedElements
{
    public abstract class BaseAnimatedElement : BaseUiElement
    {
        protected CancellationTokenSource AnimationCancellationTokenSource;

        public abstract UniTask StopAnimation();
        public abstract UniTaskVoid StartAnimation();
        public abstract void ResetToInitialState();
    }
}