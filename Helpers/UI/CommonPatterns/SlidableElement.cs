using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns
{
    public class SlidableElement : BaseUiElement
    {
        [Header("= SlidableElement Fields =")]
        [SerializeField] [CanBeNull] public BaseInteractableUiElement openPanelButton;
        [SerializeField] public Vector3 openLocalPosition;
        [SerializeField] public Vector3 closeLocalPosition;
        [SerializeField] protected bool isOpenedOnInit;

        private Vector2 _lastFrameTouchPos;
        private Vector2 _currentTouchPos;
        
        private Vector2 _currentLocalPos;

        private bool _isSwiped;
        private float _rangeForSwipe;
        
        private CancellationTokenSource _openCancellationTokenSource = new CancellationTokenSource();
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
            RectTransform.anchoredPosition = openLocalPosition;
            _isOpened = true;
        }

        public void ForceClose()
        {
            ForceEnable();
            _openCancellationTokenSource.Cancel();
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

        public async UniTask Open()
        {
            if (!_isOpened)
            {
                _isOpened = true;

                await OpenAnimation();
            }
        }

        public async UniTask Close()
        {
            if (_isOpened)
            {
                _isOpened = false;
                
                await CloseAnimation();
            }
        }

        protected virtual async UniTask OpenAnimation()
        {
            _openCancellationTokenSource.Cancel();
            _openCancellationTokenSource = new CancellationTokenSource();

            Enable().AttachExternalCancellation(_openCancellationTokenSource.Token);
                
            await RectTransform.DOAnchorPos(openLocalPosition, EnableAnimationTime)
                .WithCancellation(_openCancellationTokenSource.Token)
                .SuppressCancellationThrow();
        }
        
        protected virtual async UniTask CloseAnimation()
        {
            _openCancellationTokenSource.Cancel();
            _openCancellationTokenSource = new CancellationTokenSource();
            
            Enable().AttachExternalCancellation(_openCancellationTokenSource.Token);
            
            await RectTransform.DOAnchorPos(closeLocalPosition, EnableAnimationTime)
                .WithCancellation(_openCancellationTokenSource.Token)
                .SuppressCancellationThrow();
        }
    }
}