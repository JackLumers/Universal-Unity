using System;
using System.Collections;
using System.Reflection;
using Common.Helpers._ProjectDependent;
using Common.Helpers.Coroutines;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.Helpers.UI.BaseUiElements.GenericDynamicContainer
{
    /// <summary>
    /// UI item that is interactable and can be initialized with different parameters.
    /// </summary>
    /// <seealso cref="BaseDynamicUiElement{TItem, TData}"/>
    [RequireComponent(typeof(Button))]
    public abstract class BaseInteractableDynamicUiElement<TItem, TData> : BaseDynamicUiElement<TItem, TData>, 
        IInputHandlerUi
        where TItem : BaseDynamicUiElement<TItem, TData> 
        where TData : BaseDynamicUiElement<TItem, TData>.DynamicUiElementData
    {
        [Header("Base Interactable Dynamic UI Item Fields")]
        [SerializeField] [CanBeNull] public AudioClip onClickAudioFeedback = null;
        private Button _button;

        private void PrivateClick()
        {
            ProtectedOnClick();
            OnClick?.Invoke();
        }

        #region Protected API

        protected virtual void ProtectedOnClick()
        {
            if (onClickAudioFeedback != null && AudioManager.AudioManager.Instance != null)
                AudioManager.AudioManager.Instance.GetSource(AudioManager.AudioManager.EAudioSource.UiElementResponse)
                    .Play(onClickAudioFeedback);
            
            if (!IsDataInitialized)
            {
                LogHelper.LogHelper.Log("Data not initialized on click call.", MethodBase.GetCurrentMethod(), LogHelper.LogHelper.LogType.Error);
            }
        }
        
        protected override void InheritInitComponents()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(PrivateClick);
        }

        #endregion

        #region Public API

        public delegate void OnClickHandler();

        /// <summary>
        /// Event that is called when <see cref="PrivateClick"/> called after <see cref="ProtectedOnClick"/>.
        /// </summary>
        public event OnClickHandler OnClick;

        /// <summary>
        /// Clears all method groups that is linked with the <see cref="OnClick"/> event.
        /// </summary>
        public virtual void ClearOnClickEvents()
        {
            OnClick = null;
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

        private IEnumerator PointingProcess(PointerEventData eventData)
        {
            while (gameObject.activeInHierarchy)
            {
                HoldTime += Time.deltaTime;
                if (HoldTime >= Constants.HoldTimeBorder)
                {
                    // Only hold if not pointed
                    if (!Pointed)
                    {
                        OnHoldAction?.Invoke(eventData);
                        HoldTime = 0;
                        yield break;
                    }
                    else
                    {
                        OnHoldAction?.Invoke(eventData);
                        OnHoldAndPointedAction?.Invoke(eventData);
                        HoldTime = 0;
                        yield break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}