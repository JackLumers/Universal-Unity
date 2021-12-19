using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.UI.AnimatedElements;
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

        public int ElementsCount => elements.Length;
        
        /// <summary>
        /// Can be null before calling <see cref="InheritAwake"/>
        /// </summary>
        [CanBeNull] public FocusableElement FocusedElement { get; private set; }
        
        private void OnEnable()
        {
            ElementsAnimation[FocusedIndex].StartAnimation().Forget();
        }

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
        }

        private void ProcessNavigationClick(bool right)
        {
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