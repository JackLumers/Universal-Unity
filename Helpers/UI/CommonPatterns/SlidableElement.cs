using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Coroutines;
using UniversalUnity.Helpers.Tweeks.CurveAnimationHelper;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns
{
    public class SlidableElement : BaseUiElement
    {
        [Header("= SlidableElement Fields =")]
        [SerializeField] [CanBeNull] public BaseInteractableUiElement openPanelButton;
        [SerializeField] public Vector3 openLocalPosition;
        [SerializeField] public Vector3 closeLocalPosition;
        [SerializeField] protected bool isOpenedOnInit;
        
        private Coroutine _openCoroutine;
        private Coroutine _closeCoroutine;
        private bool _isOpened;

        private RectTransform _rectTransform;

        private RectTransform RectTransform
        {
            get
            {
                if (!ReferenceEquals(_rectTransform, null))
                {
                    return _rectTransform;
                }

                _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        protected override void InheritAwake()
        {
            base.InheritAwake();
            if (!ReferenceEquals(openPanelButton, null))
            {
                openPanelButton.OnClick += () => Switch();
            }
        }

        public void ForceOpen()
        {
            ForceEnable();
            CoroutineHelper.StopCoroutine(ref _closeCoroutine, this);
            CoroutineHelper.StopCoroutine(ref _openCoroutine, this);
            RectTransform.anchoredPosition = openLocalPosition;
            _isOpened = true;
        }

        protected override void InheritInitComponents()
        {
            base.InheritInitComponents();
            _isOpened = isOpenedOnInit;
            RectTransform.anchoredPosition = isOpenedOnInit ? openLocalPosition : closeLocalPosition;
        }

        public void ForceClose()
        {
            CoroutineHelper.StopCoroutine(ref _closeCoroutine, this);
            CoroutineHelper.StopCoroutine(ref _openCoroutine, this);
            RectTransform.anchoredPosition = closeLocalPosition;
            _isOpened = false;
        }
        
        public Coroutine Switch()
        {
            return _isOpened ? Close() : Open();
        }
        
        public Coroutine Open([CanBeNull] Action onOpened = null)
        {
            if (!_isOpened)
            {
                Enable();
                _isOpened = true;
                CoroutineHelper.StopCoroutine(ref _closeCoroutine, this);
                return CoroutineHelper.RestartCoroutine(ref _openCoroutine, OpenAnimation(onOpened), this);
            }

            return null;
        }

        public Coroutine Close([CanBeNull] Action onClosed = null)
        {
            if (_isOpened)
            {
                _isOpened = false;
                CoroutineHelper.StopCoroutine(ref _openCoroutine, this);
                return CoroutineHelper.RestartCoroutine(ref _closeCoroutine, CloseAnimation(onClosed), this);
            }
            
            return null;
        }
        
        protected virtual IEnumerator OpenAnimation([CanBeNull] Action onOpened)
        {
            yield return CurveAnimationHelper.MoveAnchored(RectTransform, openLocalPosition, speedOrTime: enableAnimationTime);
            onOpened?.Invoke();
        }
        
        protected virtual IEnumerator CloseAnimation([CanBeNull] Action onClosed)
        {
            yield return CurveAnimationHelper.MoveAnchored(RectTransform, closeLocalPosition, speedOrTime: enableAnimationTime);
            onClosed?.Invoke();
        }
    }
}