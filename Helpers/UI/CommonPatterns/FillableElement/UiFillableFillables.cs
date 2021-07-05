using System;
using System.Collections;
using System.Collections.Generic;
using Common.Helpers.Coroutines;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Helpers.UI.CommonPatterns.FillableElement
{
    /// <summary>
    /// A fillable element that fills filling elements in it.
    /// </summary>
    public class UiFillableFillables : AFillableUiElement
    {
        [Header("UiFillableFillables Fields")]
        [SerializeField] private List<AFillableUiElement> counterElements;

        private Coroutine _fillCoroutine;

        protected override void InheritInitComponents()
        {
            if (maxAmount > counterElements.Count) maxAmount = counterElements.Count;
            base.InheritInitComponents();
        }

        protected override Coroutine OnFill(float amount, float amountPerSecond, Action onFilled)
        {
            float elementEnableTimeInSeconds = amountPerSecond / maxAmount;

            return CoroutineHelper.RestartCoroutine(
                ref _fillCoroutine,
                FillProcess(amount, elementEnableTimeInSeconds, onFilled),
                this
            );
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

        private IEnumerator FillProcess(float amount, float elementFillTimeInSeconds, [CanBeNull] Action onFilled)
        {
            for (int i = 0; i < counterElements.Count; i++)
            {
                var counterElement = counterElements[i];
                // if must be filled
                if (i < amount)
                {
                    yield return counterElement.Fill(counterElement.maxAmount, elementFillTimeInSeconds * counterElement.maxAmount);
                }
                // if must be empty
                else
                {
                    yield return counterElement.Fill(0, elementFillTimeInSeconds * counterElement.maxAmount);
                }
            }

            onFilled?.Invoke();
        }
    }
}