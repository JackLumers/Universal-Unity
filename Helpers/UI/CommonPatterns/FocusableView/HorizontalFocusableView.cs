using System;
using System.Collections.Generic;
using System.Threading;
using Common.UI.Swipe;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.UI.AnimatedElements;
using UniversalUnity.Helpers.UI.BaseUiElements;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FocusableView
{
    // Work now only with same size elements and not tested properly. 
    // Must be tested and rewrote.
    public class HorizontalFocusableView : BaseUiElement
    {
        [SerializeField] protected float focusSpeed = 0.5f;
        [SerializeField] protected int initialFocusIndex;
        [SerializeField] protected RectTransform content;
        [SerializeField] protected FocusableElement[] elements;
        [SerializeField] protected BaseInteractableUiElement buttonLeft;
        [SerializeField] protected BaseInteractableUiElement buttonRight;
        [SerializeField] protected bool dynamicButtons = true;

        protected readonly List<RectTransform> ElementsRects = new List<RectTransform>();
        protected FloatingAnimatedElement[] ElementsAnimation;
        protected float ChildWidth;
        protected float InitialPosition;

        protected CancellationTokenSource FocusProcessCancellationTokenSource;
        protected int FocusedIndex;
        protected Vector3 SavedPosition;

        public Action<int> OnFocusChanged;
        
        [Header("=Swipe Element Fields=")]
        [SerializeField] private bool canSwipe;
        [SerializeField] [Range(0, 1f)] private float thresholdSwipe;
        
        private float _rangeForSwipe;
        
        private bool _isSwiped;

        private float _startTouchPosX;
        private float _endTouchPosX;

        private float _lastFrameTouchPosX;
        private float _currentTouchPosX;

        private float _currentSwipeRange;
        
        public int ElementsCount => elements.Length;
        
        /// <summary>
        /// Can be null before calling <see cref="InheritAwake"/>
        /// </summary>
        [CanBeNull] public FocusableElement FocusedElement { get; private set; }
        
        private void OnEnable()
        {
            ElementsAnimation[FocusedIndex].StartAnimation().Forget();
        }

        #region Swipe

        private void InitSwipe()
        {
            if (!canSwipe) return;

            buttonLeft.OnPointerDownAction -= StartSwipe;
            buttonLeft.OnPointerDownAction += StartSwipe;

            buttonRight.OnPointerDownAction -= StartSwipe;
            buttonRight.OnPointerDownAction += StartSwipe;

            buttonLeft.OnPointerUpAction -= EndSwipe;
            buttonLeft.OnPointerUpAction += EndSwipe;

            buttonRight.OnPointerUpAction -= EndSwipe;
            buttonRight.OnPointerUpAction += EndSwipe;

            ElementsAnimation[FocusedIndex].StopAnimation();
            _rangeForSwipe = ChildWidth * thresholdSwipe;
        }

        private void StartSwipe(PointerEventData data)
        {
            _startTouchPosX = data.position.x;
            _currentTouchPosX = SavedPosition.x;
            _lastFrameTouchPosX = data.position.x;
            _currentSwipeRange = 0;
            
            _isSwiped = true;

            ElementsAnimation[FocusedIndex].enabled = false;
        }

        private void EndSwipe(PointerEventData data)
        {
            if (!_isSwiped) return;
            
            _endTouchPosX = data.position.x;
            _currentSwipeRange = 0;
            _isSwiped = false;
            
            ControlSwipe();
        }

        private void ControlSwipe()
        {
            float currentRange = _startTouchPosX - _endTouchPosX;
            if (currentRange == 0) return;

            if (Math.Abs(currentRange) > _rangeForSwipe)
            {
                int index = 0;
                float minDistance = Math.Abs(-_currentTouchPosX - ElementsRects[index].localPosition.x);
                
                for (int i = 1; i < elements.Length; i++)
                {
                    if (i == FocusedIndex) continue;
                    if (minDistance < Math.Abs(-_currentTouchPosX - ElementsRects[i].localPosition.x) ||
                        !CanFocus(currentRange > 0)) continue;

                    index = i;
                    minDistance = -_currentTouchPosX - ElementsRects[i].localPosition.x;
                }

                Focus(index).Forget();
                _currentTouchPosX = SavedPosition.x;
            }
            else
            {
                Focus().Forget();
                _currentTouchPosX = SavedPosition.x;
            }
        }

        private void MoveSwipe()
        {
            _currentSwipeRange = 
                SwipingController.Instance.playerActions.UI.ClickPostion.ReadValue<Vector2>().x - _lastFrameTouchPosX;

            _currentTouchPosX += _currentSwipeRange;
            
            content.localPosition = 
                Vector2.Lerp(content.localPosition, new Vector2(_currentTouchPosX, 0), focusSpeed);

            _lastFrameTouchPosX = SwipingController.Instance.playerActions.UI.ClickPostion.ReadValue<Vector2>().x;
        }

        private void Update()
        {
            if (_isSwiped) MoveSwipe();
        }

        #endregion

        private void UpdateFocus(int focusIndex)
        {
            if (FocusedElement != null) FocusedElement.Focused = false;
            FocusedElement = elements[focusIndex];
            // ReSharper disable once PossibleNullReferenceException
            FocusedElement.Focused = true;
            FocusedIndex = focusIndex;
            
            ElementsAnimation[focusIndex].StartAnimation().Forget();
            OnFocusChanged?.Invoke(focusIndex);
        }

        protected override void InheritAwake()
        {
            base.InheritAwake();

            /* Checks */
            if (initialFocusIndex < 0)
            {
                LogHelper.LogError($"{nameof(initialFocusIndex)} Must be more than 0.",
                    nameof(InheritAwake));
                return;
            }

            /* Getting child rects and making focusable elements*/
            ElementsAnimation = new FloatingAnimatedElement[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                ElementsRects.Add(elements[i].GetComponent<RectTransform>());
                ElementsAnimation[i] = elements[i].GetComponent<FloatingAnimatedElement>();
                elements[i].Focused = false;
            }
            
            /* Setting initial position */
            if (initialFocusIndex > ElementsCount)
            {
                initialFocusIndex = ElementsCount-1;
            }

            ChildWidth = ElementsRects[1].rect.width;
            
            // even elements count
            if (ElementsCount % 2 == 0)
            {
                var stepLength = ChildWidth / 2;
                // right
                if (initialFocusIndex > (float) ElementsCount / 2)
                {
                    content.localPosition = new Vector3(-stepLength, 0);
                }
                // left
                else if (initialFocusIndex < (float) ElementsCount / 2)
                {
                    content.localPosition = new Vector3(stepLength, 0);
                }
            }

            InitialPosition = content.localPosition.x;
            SavedPosition = new Vector3(InitialPosition, 0);
            FocusedIndex = initialFocusIndex;

            UpdateFocus(FocusedIndex);

            /* Listeners */
            buttonLeft.OnClick += () => {ProcessNavigationClick(false);};
            buttonRight.OnClick += () => {ProcessNavigationClick(true);};
            
            UpdateButtonsState();
            InitSwipe();
        }

        private void ProcessNavigationClick(bool right)
        {
            if(_currentSwipeRange > 10) return;
            Focus(right).Forget();
        }
        
        public async UniTask Focus(int childIndex)
        {
            if (childIndex < 0)
            {
                LogHelper.LogError($"{nameof(childIndex)} Must be more than 0.", nameof(Focus));
                return;
            }
            
            if (childIndex > ElementsCount)
            {
                childIndex = ElementsCount - 1;
            }
            
            FocusProcessCancellationTokenSource?.Cancel();
            FocusProcessCancellationTokenSource = new CancellationTokenSource();
            
            ElementsAnimation[FocusedIndex].StopAnimation().Forget();
            
            int offset = Math.Abs(FocusedIndex - childIndex);
            int direction = FocusedIndex - childIndex;
            FocusedIndex = childIndex;
            
            UpdateFocus(FocusedIndex);
            
            UpdateButtonsState();

            // right
            if (direction < 0)
            {
                SavedPosition = new Vector3(SavedPosition.x - ChildWidth * offset, 0);
                await content.DOLocalMove(SavedPosition, focusSpeed)
                    .WithCancellation(FocusProcessCancellationTokenSource.Token);
                return;
            }
            
            // left
            SavedPosition = new Vector3(SavedPosition.x + ChildWidth * offset, 0);
            await content.DOLocalMove(SavedPosition, focusSpeed)
                .WithCancellation(FocusProcessCancellationTokenSource.Token);
        }

        private async UniTask Focus()
        {
            FocusProcessCancellationTokenSource?.Cancel();
            FocusProcessCancellationTokenSource = new CancellationTokenSource();
            
            await content.DOLocalMove(SavedPosition, focusSpeed)
                .WithCancellation(FocusProcessCancellationTokenSource.Token);
        }

        public async UniTask Focus(bool right)
        {
            // right
            ElementsAnimation[FocusedIndex].StopAnimation().Forget();
            
            FocusProcessCancellationTokenSource?.Cancel();
            FocusProcessCancellationTokenSource = new CancellationTokenSource();
            
            if (right)
            {
                if (FocusedIndex < ElementsCount - 1)
                {
                    
                    FocusedIndex++;
                    UpdateFocus(FocusedIndex);
                    UpdateButtonsState();
                    
                    SavedPosition = new Vector3(SavedPosition.x - ChildWidth, 0);
                    await content.DOLocalMove(SavedPosition, focusSpeed)
                        .WithCancellation(FocusProcessCancellationTokenSource.Token);
                    return;
                }
            }
            // left
            else
            {
                if (FocusedIndex > 0)
                {
                    FocusedIndex--;
                    UpdateFocus(FocusedIndex);
                    UpdateButtonsState();
                    
                    SavedPosition = new Vector3(SavedPosition.x + ChildWidth, 0);
                    await content.DOLocalMove(SavedPosition, focusSpeed)
                        .WithCancellation(FocusProcessCancellationTokenSource.Token);
                    return;
                }
            }
            
            LogHelper.LogInfo($"Can't focus, no elements in this direction. Right: {right}.", nameof(Focus));
        }

        public bool CanFocus(bool right)
        {
            if (right)
            {
                if (FocusedIndex < ElementsCount - 1) return true;
            }
            else
            {
                if (FocusedIndex > 0) return true;
            }

            return false;
        }

        private void UpdateButtonsState()
        {
            if (dynamicButtons)
            {
                if (!CanFocus(true))
                {
                    buttonRight.Disable().Forget();
                }
                else
                {
                    buttonRight.Enable().Forget();
                }

                if (!CanFocus(false))
                {
                    buttonLeft.Disable().Forget();
                }
                else
                {
                    buttonLeft.Enable().Forget();
                } 
            }
        }
    }
}