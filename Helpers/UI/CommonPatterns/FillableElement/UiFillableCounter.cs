using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Coroutines;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FillableElement
{
    public class UiFillableCounter : AFillableUiElement
    {
        [Header("UiFillableCounter Fields")]
        [SerializeField] private List<BaseUiElement> counterElements;

        private Coroutine _fillCoroutine;

        protected override void InheritInitComponents()
        {
            if (maxAmount > counterElements.Count) maxAmount = counterElements.Count;
            base.InheritInitComponents();
        }

        protected override Coroutine OnFill(float amount, float amountPerSecond, Action onFilled)
        {
            float elementEnableTimeInSeconds = maxAmount / amountPerSecond;

            foreach (var counterElement in counterElements)
            {
                counterElement.EnableAnimationTime = elementEnableTimeInSeconds;
            }
            
            return CoroutineHelper.RestartCoroutine(
                ref _fillCoroutine,
                FillProcess(amount, onFilled),
                this
            );
        }

        protected override void OnForceFill(float amount)
        {
            for (var i = 0; i < counterElements.Count; i++)
            {
                if (i < amount)
                {
                    counterElements[i].ForceEnable();
                }
                else
                {
                    counterElements[i].ForceDisable();
                }
            }
        }

        private IEnumerator FillProcess(float amount, [CanBeNull] Action onFilled)
        {
            for (var i = 0; i < counterElements.Count; i++)
            {
                if (i < amount)
                {
                    yield return counterElements[i].Enable();
                }
                else
                {
                    yield return counterElements[i].Disable();
                }
            }

            onFilled?.Invoke();
        }
    }
}