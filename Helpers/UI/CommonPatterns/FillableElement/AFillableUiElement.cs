﻿using System;
using System.Reflection;
using Common.Helpers.UI.BaseUiElements;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Helpers.UI.CommonPatterns.FillableElement
{
    public abstract class AFillableUiElement : BaseUiElement
    {
        [Header("AFillableUiElement Fields")]
        [SerializeField] public float maxAmount = 100;
        [SerializeField] public float initialAmount = 0;

        public float FilledAmount { get; private set; }

        protected override void InheritInitComponents()
        {
            base.InheritInitComponents();

            initialAmount = ClampAmount(initialAmount);
            ForceFill(initialAmount);
        }

        public Coroutine Fill(float amount, float amountPerSecond = 100f, bool fillOnEvenAmount = false, [CanBeNull] Action onFilled = null)
        {
            if(!IsInitialized) InitComponents();
            amount = ClampAmount(amount);

            // Don't fill if aready filled on this amount OR ! pass if fillOnEvenAmount = true !
            if (fillOnEvenAmount || (Math.Abs(FilledAmount - amount) > Single.Epsilon))
            {
                var onFill = OnFill(amount, amountPerSecond, onFilled);
                FilledAmount = amount;
                return onFill;
            }
            else
            {
                //return CoroutineHelper.Skip(this);
                return null;
            }
        }

        public void ForceFill(float amount)
        {
            if(!IsInitialized) InitComponents();
            amount = ClampAmount(amount);
            
            OnForceFill(amount);
            FilledAmount = amount;
        }

        protected abstract Coroutine OnFill(float amount, float amountPerSecond, [CanBeNull] Action onFilled);

        protected abstract void OnForceFill(float amount);

        private float ClampAmount(float amount)
        {
            if (amount < 0)
            {
                LogHelper.LogHelper.Log($"{nameof(amount)} Must be more than 0. Force set to 0.",
                    MethodBase.GetCurrentMethod(),
                    LogHelper.LogHelper.LogType.Error);
                
                amount = 0;
            }
            else if (amount > maxAmount)
            {
                LogHelper.LogHelper.Log(
                    $"{nameof(amount)} Must be less than {nameof(maxAmount)} ({maxAmount}). " +
                    $"Force set to {maxAmount}.",
                    MethodBase.GetCurrentMethod(),
                    LogHelper.LogHelper.LogType.Error);
                
                amount = maxAmount;
            }

            return amount;
        }
    }
}