using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UniversalUnity.Helpers.UI.BaseUiElements.BaseElements
{
    public class AcceptButton : BaseInteractableUiElement
    {
        [SerializeField] [CanBeNull] private Text submitText = null;
        [SerializeField] private float enabledScale = 1.3f;

        private RectTransform _rectTransform;
        private CancellationTokenSource _stateChangeCancellationTokenSource;
        private ButtonState _currentState = ButtonState.None;

        public enum ButtonState
        {
            None,
            Disabled,
            Enabled
        }

        protected override void InheritInitComponents()
        {
            base.InheritInitComponents();
            _rectTransform = (RectTransform) transform;
        }

        public void ChangeSubmitButtonState(ButtonState state, [CanBeNull] Action onClick,
            [CanBeNull] string text = null)
        {
            if (_currentState == state) return;

            if (!IsInitialized) InitComponents();

            AnimateToState(state, text).Forget();
            SetClickEventToState(state, onClick);

            _currentState = state;
        }

        protected virtual void SetClickEventToState(ButtonState state, [CanBeNull] Action onClick)
        {
            ClearOnClickEvents();

            switch (state)
            {
                case ButtonState.Disabled:
                    InteractionBlock("ButtonState", true, true);
                    break;

                case ButtonState.Enabled:
                    InteractionBlock("ButtonState", false, true);
                    if (onClick != null)
                    {
                        OnClick += onClick.Invoke;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected virtual async UniTask AnimateToState(ButtonState state, [CanBeNull] string text)
        {
            // Changing text
            if (!(submitText is null))
            {
                submitText.text = text;
            }

            switch (state)
            {
                case ButtonState.Disabled:
                    await _rectTransform.DOScale(Vector3.one, EnableAnimationTime)
                        .WithCancellation(_stateChangeCancellationTokenSource.Token);
                    break;

                case ButtonState.Enabled:
                    await _rectTransform.DOScale(new Vector3(enabledScale, enabledScale, 1f), EnableAnimationTime)
                        .WithCancellation(_stateChangeCancellationTokenSource.Token);
                    break;

                case ButtonState.None:
                    throw new NotImplementedException(nameof(state) + "State");

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}