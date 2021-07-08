using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
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

        private CancellationTokenSource _movingCancellationTokenSource = new CancellationTokenSource();
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
                openPanelButton.OnClick += async () => await Switch();
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
            _movingCancellationTokenSource.Cancel();
            RectTransform.anchoredPosition = openLocalPosition;
            _isOpened = true;
        }

        public void ForceClose()
        {
            ForceEnable();
            _movingCancellationTokenSource.Cancel();
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
                _movingCancellationTokenSource.Cancel();
                _movingCancellationTokenSource = new CancellationTokenSource();
                _isOpened = true;
                
                await UniTask.WhenAll
                (
                    Enable().AttachExternalCancellation(_movingCancellationTokenSource.Token),
                    UniTask.Run(() =>
                    {
                        _isOpened = true;
                        // TODO: Add slide logic
                        // Animator.SetTrigger(_openTrigger);
                        // UniTask.Yield(PlayerLoopTiming.Update);
                        // UniTask.Delay(Animator.GetCurrentAnimatorClipInfo(0).Length, 
                        //     cancellationToken: _movingCancellationTokenSource.Token);
                        onOpened?.Invoke();
                    }));
            }
        }

        public async UniTask Close([CanBeNull] Action onClosed = null)
        {
            if (_isOpened)
            {
                _movingCancellationTokenSource.Cancel();
                _movingCancellationTokenSource = new CancellationTokenSource();
                _isOpened = false;
                
                await UniTask.WhenAll
                (
                    Enable().AttachExternalCancellation(_movingCancellationTokenSource.Token),
                    UniTask.Run(() =>
                    {
                        _isOpened = true;
                        // TODO: Add slide logic
                        // Animator.SetTrigger(_closeTrigger);
                        // UniTask.Yield(PlayerLoopTiming.Update);
                        // UniTask.Delay(Animator.GetCurrentAnimatorClipInfo(0).Length, 
                        //     cancellationToken: _movingCancellationTokenSource.Token);
                        onClosed?.Invoke();
                    }));
            }
        }
    }
}