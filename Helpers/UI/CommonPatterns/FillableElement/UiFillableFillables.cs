using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FillableElement
{
    /// <summary>
    /// A fillable element that fills filling elements in it.
    /// </summary>
    public class UiFillableFillables : AFillableUiElement
    {
        [Header("UiFillableFillables Fields")]
        [SerializeField] private List<AFillableUiElement> counterElements;

        private CancellationTokenSource _fillCancellationTokenSource = new CancellationTokenSource();

        protected override void InheritInitComponents()
        {
            if (maxAmount > counterElements.Count) maxAmount = counterElements.Count;
            base.InheritInitComponents();
        }

        protected override async UniTask OnFill(float amount, float timeInSeconds, CancellationToken cancellationToken)
        {
            _fillCancellationTokenSource.Cancel();
            _fillCancellationTokenSource = new CancellationTokenSource();

            for (int i = 0; i < counterElements.Count; i++)
            {
                var counterElement = counterElements[i];
                // if must be filled
                if (i < amount)
                {
                    await counterElement.Fill(counterElement.maxAmount, timeInSeconds)
                        .AttachExternalCancellation(cancellationToken);
                }
                // if must be empty
                else
                {
                    await counterElement.Fill(0, timeInSeconds).AttachExternalCancellation(cancellationToken);;
                }
            }
        }

        protected override void OnForceFill(float amount)
        {
            for (var i = 0; i < counterElements.Count; i++)
            {
                // if must be filled
                if (i < amount)
                {
                    counterElements[i].ForceFill(counterElements[i].maxAmount);
                }
                // if must be empty
                else
                {
                    counterElements[i].ForceFill(0);
                }
            }
        }
    }
}