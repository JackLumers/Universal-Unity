using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.UI.CommonPatterns.FillableElement;

namespace UniversalUnity.Helpers.UI.CommonPatterns.Timers
{
    public class UiFillableTimer : BaseUiTimer
    {
        [SerializeField] protected UiFillableLine uiFillableLine;
        
        protected override async UniTask UiHandleTimer(float durationInMillis, CancellationToken cancellationToken)
        {
            uiFillableLine.ForceFill(0);
            Enable().Forget();
            await uiFillableLine.Fill(100,durationInMillis / 1000)
                .AttachExternalCancellation(cancellationToken);
        }
    }
}