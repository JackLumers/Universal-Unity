using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
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

        private CancellationTokenSource _openCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _closeCancellationTokenSource = new CancellationTokenSource();
        private bool _isOpened;

        private string _openTrigger = "OpenTrigger";
        private string _closeTrigger = "CloseTrigger";
        
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
                openPanelButton.OnClick += UniTask.Action(async () => await Switch());
            }
        }
        
        protected override void InheritInitComponents()
        {
            base.InheritInitComponents();
            _isOpened = isOpenedOnInit;
            RectTransform.anchoredPosition = isOpenedOnInit ? openLocalPosition : closeLocalPosition;
        }

        public void ForceOpen()
        {
            ForceEnable();
            _openCancellationTokenSource.Cancel();
            _closeCancellationTokenSource.Cancel();
            RectTransform.anchoredPosition = openLocalPosition;
            _isOpened = true;
        }

        public void ForceClose()
        {
            ForceEnable();
            _openCancellationTokenSource.Cancel();
            _closeCancellationTokenSource.Cancel();
            RectTransform.anchoredPosition = closeLocalPosition;
            _isOpened = false;
        }
        
        public async UniTask Switch()
        {
            switch (_isOpened)
            {
                case true:
                    await Close();
                    break;
                case false:
                    await Open();
                    break;
            }
        }

        public async UniTask Open([CanBeNull] Action onOpened = null)
        {
            if (!_isOpened)
            {
                _openCancellationTokenSource.Cancel();
                _closeCancellationTokenSource.Cancel();
                
                _isOpened = true;

                _openCancellationTokenSource = new CancellationTokenSource();
                await UniTask.WhenAll
                (
                    Enable().AttachExternalCancellation(_openCancellationTokenSource.Token),
                    CurveAnimationHelper.MoveAnchored(RectTransform, openLocalPosition,
                        speedOrTime: EnableAnimationTime,
                        cancellationToken: _openCancellationTokenSource.Token)
                );
                onOpened?.Invoke();
            }
        }

        public async UniTask Close([CanBeNull] Action onClosed = null)
        {
            if (_isOpened)
            {
                _openCancellationTokenSource.Cancel();
                _closeCancellationTokenSource.Cancel();
                
                _isOpened = false;
                
                _closeCancellationTokenSource = new CancellationTokenSource();
                await UniTask.WhenAll
                (
                    Enable().AttachExternalCancellation(_closeCancellationTokenSource.Token),
                    CurveAnimationHelper.MoveAnchored(RectTransform, closeLocalPosition,
                        speedOrTime: EnableAnimationTime, 
                        cancellationToken: _closeCancellationTokenSource.Token)
                );
                
                onClosed?.Invoke();
            }
        }
    }
}