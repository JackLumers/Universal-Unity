using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniversalUnity.Helpers._ProjectDependent;
using UniversalUnity.Helpers.Coroutines;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    /// <summary>
    /// Base class for all UI elements that can be clicked by user.
    /// It contains basic animation behavior that can be overriden by inheritors if needed.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class BaseInteractableUiElement : BaseUiElement, IInputHandlerUi
    {
        [Header("= BaseInteractableUiElement Fields =")]
        [SerializeField] [CanBeNull] protected AudioClip onClickAudioFeedback;
        private Button _button;

        private void PrivateClick()
        {
            ProtectedOnClick();
            OnClick?.Invoke();
        }

        #region Protected API

        protected override void InheritInitComponents()
        {
            base.InheritInitComponents();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(PrivateClick);
        }

        /// <summary>
        /// Called when <see cref="PrivateClick"/> called before <see cref="OnClick"/> event invoked.
        /// </summary>
        protected virtual void ProtectedOnClick()
        {
            if (onClickAudioFeedback != null)
                AudioManager.AudioManager.Instance?.GetSource(AudioManager.AudioManager.EAudioSource.UiElementResponse).Play(onClickAudioFeedback);
        }

        #endregion

        #region Public API
        
        /// <summary>
        /// Event that is called when <see cref="PrivateClick"/> called after <see cref="ProtectedOnClick"/>.
        /// </summary>
        public event Action OnClick;

        /// <summary>
        /// Clears all method groups that is linked with the <see cref="OnClick"/> event.
        /// </summary>
        public void ClearOnClickEvents()
        {
            OnClick = null;
        }

        public Image ButtonImage()
        {
            if(!IsInitialized) InitComponents();
            return _button.image;
        }

        #endregion

        #region IRaycastInputUiHandler Impl

        public bool PointedDown { get; set; }
        public float HoldTime { get; set; }
        public bool Pointed { get; set; }
        public Action<PointerEventData> OnHoldAction { get; set; }
        public Action<PointerEventData> OnHoldAndPointedAction { get; set; }
        public Action<PointerEventData> OnPointerDownAction { get; set; }
        public Action<PointerEventData> OnPointerUpAction { get; set; }
        public Action<PointerEventData> OnPointerUpAndPointedAction { get; set; }
        public Action<PointerEventData> OnPointerEnterAction { get; set; }
        public Action<PointerEventData> OnPointerExitAction { get; set; }

        private Coroutine _pointingCoroutine;

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownAction?.Invoke(eventData);
            
            PointedDown = true;
            HoldTime = 0;
            Pointed = true;
            CoroutineHelper.RestartCoroutine(ref _pointingCoroutine, PointingProcess(eventData), this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpAction?.Invoke(eventData);
            
            if (Pointed)
            {
                OnPointerUpAndPointedAction?.Invoke(eventData);
            }
            
            PointedDown = false;
            HoldTime = 0;
            Pointed = false;
            
            CoroutineHelper.StopCoroutine(ref _pointingCoroutine, this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitAction?.Invoke(eventData);
            Pointed = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterAction?.Invoke(eventData);
            Pointed = true;
        }

        private IEnumerator PointingProcess(PointerEventData pointerEventData)
        {
            while (gameObject.activeInHierarchy)
            {
                HoldTime += Time.deltaTime;
                if (HoldTime >= Constants.HoldTimeBorder)
                {
                    // Only hold if not pointed
                    if (!Pointed)
                    {
                        OnHoldAction?.Invoke(pointerEventData);
                        HoldTime = 0;
                        yield break;
                    }
                    else
                    {
                        OnHoldAndPointedAction?.Invoke(pointerEventData);
                        HoldTime = 0;
                        yield break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            HoldTime = 0;
        }

        #endregion
    }
}
