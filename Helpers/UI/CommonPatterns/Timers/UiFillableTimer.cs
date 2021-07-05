using Common.Helpers.UI.CommonPatterns.FillableElement;
using UnityEngine;

namespace Common.Helpers.UI.CommonPatterns.Timers
{
    public class UiFillableTimer : BaseUiTimer
    {
        [SerializeField] protected UiFillableLine uiFillableLine;
        
        protected override Coroutine UiHandleTimer(float durationInMillis)
        {
            uiFillableLine.ForceFill(0);
            Enable();

            return uiFillableLine.Fill(100,  uiFillableLine.maxAmount / (durationInMillis / 1000));
        }
    }
}