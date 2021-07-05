using System;
using Common.Helpers.Coroutines;
using Common.Helpers.Tweeks.CurveAnimationHelper;
using Common.Helpers.UI.BaseUiElements;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Helpers.UI.CommonPatterns
{
    public class AcceptButton : BaseInteractableUiElement
    {
        [SerializeField] [CanBeNull] private Text submitText = null;
        [SerializeField] private float enabledScale = 1.3f;

        private RectTransform _rectTransform;
        private Coroutine _colorChangeCoroutine;
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

        public void ChangeSubmitButtonState(ButtonState state, [CanBeNull] Action onClick, [CanBeNull] string text = null)
        {
            if (_currentState == state) return;

            if (!IsInitialized) InitComponents();

            AnimateToState(state, text);
            SetClickEventToState(state, onClick);
        }

        protected virtual void SetClickEventToState(ButtonState state, [CanBeNull] Action onClick)
        {
            ClearOnClickEvents();
            
            switch (state)
            {
                case ButtonState.Disabled:
                    Interactable = false;
                    break;
                
                case ButtonState.Enabled:
                    Interactable = true;
                    if (onClick != null)
                    {
                        OnClick += onClick.Invoke;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected virtual Coroutine AnimateToState(ButtonState state, [CanBeNull] string text)
        {
            // Changing text
            if (!(submitText is null))
            {
                submitText.text = text;
            }
            
            switch (state)
            {
                case ButtonState.Disabled:
                    return CoroutineHelper.RestartCoroutine(ref _colorChangeCoroutine,
                        CurveAnimationHelper.Scale(_rectTransform,
                            Vector3.one,
                            speedOrTime: 0.5f), this);
                
                case ButtonState.Enabled:
                    return CoroutineHelper.RestartCoroutine(ref _colorChangeCoroutine,
                        CurveAnimationHelper.Scale(_rectTransform,
                            new Vector3(enabledScale, enabledScale, 1f),
                            speedOrTime: 0.5f), this);

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}