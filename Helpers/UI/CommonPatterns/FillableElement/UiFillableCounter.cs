using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        protected override async UniTask OnFill(float amount, float amountPerSecond)
        {
            float elementEnableTimeInSeconds = maxAmount / amountPerSecond;

            foreach (var counterElement in counterElements)
            {
                counterElement.EnableAnimationTime = elementEnableTimeInSeconds;
            }

            await FillProcess(amount);
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

        private async UniTask FillProcess(float amount)
        {
            for (var i = 0; i < counterElements.Count; i++)
            {
                if (i < amount)
                {
                    await counterElements[i].Enable();
                }
                else
                {
                    await counterElements[i].Disable();
                }
            }
        }
    }
}