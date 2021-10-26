using System;
using System.Threading;
using Common.UI.Swipe;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UniversalUnity.Helpers.UI.BaseUiElements;
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

        [Header("= SwipeElement Fields =")] 
        [SerializeField] private bool canSwipe;
        [SerializeField] private SwipingAxis swipeAxis;
        [SerializeField] [Range(0, 1f)] private float thresholdSwipe;

        private Vector2 _lastFrameTouchPos;
        private Vector2 _currentTouchPos;
        
        private Vector2 _currentLocalPos;

        private bool _isSwiped;
        private float _rangeForSwipe;
        
        private CancellationTokenSource _openCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _closeCancellationTokenSource = new CancellationTokenSource();
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
            
            if (canSwipe) InitSwipe();
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

        public async UniTask Open()
        {
            if (!_isOpened)
            {
                if (canSwipe) _currentLocalPos = openLocalPosition;
                
                _openCancellationTokenSource.Cancel();
                _closeCancellationTokenSource.Cancel();
                
                _isOpened = true;

                _openCancellationTokenSource = new CancellationTokenSource();
                await UniTask.WhenAll
                (
                    Enable().AttachExternalCancellation(_openCancellationTokenSource.Token),
                    OpenAnimation()
                );
            }
        }

        public async UniTask Close()
        {
            if (_isOpened)
            {
                if (canSwipe) _currentLocalPos = closeLocalPosition;
                
                _openCancellationTokenSource.Cancel();
                _closeCancellationTokenSource.Cancel();
                
                _isOpened = false;
                
                _closeCancellationTokenSource = new CancellationTokenSource();
                await UniTask.WhenAll
                (
                    Enable().AttachExternalCancellation(_closeCancellationTokenSource.Token),
                    CloseAnimation()
                );
            }
        }

        #region Swipe

        private void InitSwipe()
        {
            if (openPanelButton == null) return;
            
            openPanelButton.OnPointerDownAction += StartTouch;
            SwipingController.Instance.playerActions.UI.Click.canceled += EndTouch;

            _rangeForSwipe = swipeAxis switch
            {
                SwipingAxis.Vertical => Math.Abs(openLocalPosition.y - closeLocalPosition.y),
                SwipingAxis.Horizontal => Math.Abs(openLocalPosition.x - closeLocalPosition.x),
                _ => throw new ArgumentOutOfRangeException()
            } * thresholdSwipe;
        }

        private void StartTouch(PointerEventData data)
        {
            _lastFrameTouchPos =
                GetNeededAxisVector(SwipingController.Instance.playerActions.UI.ClickPostion.ReadValue<Vector2>());

            _isSwiped = true;
        }

        private void EndTouch(InputAction.CallbackContext context)
        {
            if (!_isSwiped) return;
            _isSwiped = false;
            Vector2 endPos = SwipingController.Instance.playerActions.UI.ClickPostion.ReadValue<Vector2>();

            ControlSwipe();
        }

        private void MoveTouch()
        {
            if (!_isSwiped) return;
            
            _currentTouchPos = GetNeededAxisVector(SwipingController.Instance.playerActions.UI.ClickPostion.ReadValue<Vector2>());
            float rangeCurrentSwipe = GetNeededAxisValue(_currentTouchPos - _lastFrameTouchPos);

            _currentLocalPos = FoldWithNeededAxis(_currentLocalPos, rangeCurrentSwipe);
            
            if (rangeCurrentSwipe > 0) ;
            
            // border panel check
            if (GetNeededAxisValue(openLocalPosition) > GetNeededAxisValue(closeLocalPosition))
            {
                if (GetNeededAxisValue(_currentLocalPos) > GetNeededAxisValue(openLocalPosition))
                {
                    _currentLocalPos = openLocalPosition;
                }
                if (GetNeededAxisValue(_currentLocalPos) < GetNeededAxisValue(closeLocalPosition))
                {
                    _currentLocalPos = closeLocalPosition;
                }
            }
            else
            {
                if (GetNeededAxisValue(_currentLocalPos) < GetNeededAxisValue(openLocalPosition))
                {
                    _currentLocalPos = openLocalPosition;
                }
                else if (GetNeededAxisValue(_currentLocalPos) > GetNeededAxisValue(closeLocalPosition))
                {
                    _currentLocalPos = closeLocalPosition;
                }
            }
            
            _lastFrameTouchPos = _currentTouchPos;
            _rectTransform.anchoredPosition =
                Vector2.Lerp(_rectTransform.anchoredPosition, _currentLocalPos, EnableAnimationTime);
        }

        private void ControlSwipe()
        {
            float rangeCurrentSwipe;

            if (_isOpened)
            {
                rangeCurrentSwipe = GetNeededAxisValue(GetNeededAxisVector(openLocalPosition - new Vector3(_currentLocalPos.x, _currentLocalPos.y)));

                if (_rangeForSwipe <= Math.Abs(rangeCurrentSwipe) &&
                    GetDistance(openLocalPosition, closeLocalPosition) >
                    GetDistance(_currentLocalPos, closeLocalPosition))
                {
                    Close().Forget();
                }
                else
                {
                    Open().Forget();
                }
            }
            else // Close
            {
                rangeCurrentSwipe = GetNeededAxisValue(GetNeededAxisVector(closeLocalPosition - new Vector3(_currentLocalPos.x, _currentLocalPos.y)));

                if (_rangeForSwipe <= Math.Abs(rangeCurrentSwipe) &&
                    GetDistance(openLocalPosition, closeLocalPosition) >
                    GetDistance(openLocalPosition, _currentLocalPos))
                {
                    Open().Forget();
                }
                else
                {
                    Close().Forget();
                }
            }
        }

        private Vector2 GetNeededAxisVector(Vector2 vector)
        {
            return swipeAxis switch
            {
                SwipingAxis.Vertical => new Vector2(0, vector.y),
                SwipingAxis.Horizontal => new Vector2(vector.x, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float GetNeededAxisValue(Vector2 vector)
        {
            return swipeAxis switch
            {
                SwipingAxis.Vertical => vector.y,
                SwipingAxis.Horizontal => vector.x,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float GetDistance(Vector2 start, Vector2 finish)
        {
            return Vector2.Distance(start, finish);
        }

        private Vector2 FoldWithNeededAxis(Vector2 vector, float value)
        {
             return swipeAxis switch
            {
                SwipingAxis.Vertical => new Vector2(vector.x, vector.y + value),
                SwipingAxis.Horizontal => new Vector2(vector.x + value, vector.y),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void Update()
        {
            if (_isSwiped) MoveTouch();
        }

        #endregion

        protected virtual async UniTask OpenAnimation()
        {
            await RectTransform.DOAnchorPos(openLocalPosition, EnableAnimationTime)
                .WithCancellation(_openCancellationTokenSource.Token)
                .SuppressCancellationThrow();
        }
        
        protected virtual async UniTask CloseAnimation()
        {
            await RectTransform.DOAnchorPos(closeLocalPosition, EnableAnimationTime)
                .WithCancellation(_openCancellationTokenSource.Token)
                .SuppressCancellationThrow();
        }
    }
}