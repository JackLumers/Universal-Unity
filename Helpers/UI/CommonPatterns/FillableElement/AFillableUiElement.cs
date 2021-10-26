using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.UI.BaseUiElements;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FillableElement
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

        public async UniTask Fill(float amount, float amountPerSecond = 100f, bool fillOnEvenAmount = false)
        {
            if(!IsInitialized) InitComponents();
            amount = ClampAmount(amount);

            // Don't fill if already filled on this amount OR pass if fillOnEvenAmount = true
            if (fillOnEvenAmount || (Math.Abs(FilledAmount - amount) > Single.Epsilon))
            {
                FilledAmount = amount;
                await OnFill(amount, amountPerSecond);
            }
        }

        public void ForceFill(float amount)
        {
            if(!IsInitialized) InitComponents();
            amount = ClampAmount(amount);
            
            OnForceFill(amount);
            FilledAmount = amount;
        }

        protected abstract UniTask OnFill(float amount, float amountPerSecond);
        
        protected abstract void OnForceFill(float amount);

        private float ClampAmount(float amount)
        {
            if (amount < 0)
            {
                LogHelper.LogError($"{nameof(amount)} Must be more than 0. Force set to 0.", 
                    nameof(ClampAmount));
                
                amount = 0;
            }
            else if (amount > maxAmount)
            {
                LogHelper.LogError(
                    $"{nameof(amount)} Must be less than {nameof(maxAmount)} ({maxAmount}). " +
                    $"Force set to {maxAmount}.",
                    nameof(ClampAmount));
                
                amount = maxAmount;
            }

            return amount;
        }
    }
}