using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Coroutines;
using UniversalUnity.Helpers.Tweeks.CurveAnimationHelper;
using UniversalUnity.Helpers.UI.BaseUiElements;

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

        protected List<RectTransform> _elementsRects = new List<RectTransform>();
        protected float _childWidth;
        protected float _initialPosition;
        
        protected Coroutine _focusCoroutine;
        protected int _focusedIndex;
        protected Vector3 _savedPosition;

        public Action<int> OnFocusChanged;
        
        public int ElementsCount => _elementsRects.Count;
        
        /// <summary>
        /// Can be null before calling <see cref="InheritAwake"/>
        /// </summary>
        [CanBeNull] public FocusableElement FocusedElement { get; private set; }

        private void UpdateFocus(int focusIndex)
        {
            if (FocusedElement != null) FocusedElement.Focused = false;
            FocusedElement = elements[focusIndex];
            // ReSharper disable once PossibleNullReferenceException
            FocusedElement.Focused = true;

            OnFocusChanged?.Invoke(focusIndex);
        }

        protected override void InheritAwake()
        {
            base.InheritAwake();

            /* Checks */
            if (initialFocusIndex < 0)
            {
                LogHelper.LogHelper.Log($"{nameof(initialFocusIndex)} Must be more than 0.",
                    MethodBase.GetCurrentMethod(), LogHelper.LogHelper.LogType.Error);
                return;
            }
            
            /* Getting child rects and making focusable elements*/
            foreach (var element in elements)
            {
                _elementsRects.Add(element.GetComponent<RectTransform>());
                element.Focused = false;
            }
            
            /* Setting initial position */
            if (initialFocusIndex > ElementsCount)
            {
                initialFocusIndex = ElementsCount-1;
            }

            _childWidth = _elementsRects[1].rect.width;
            
            // even elements count
            if (ElementsCount % 2 == 0)
            {
                var stepLength = _childWidth / 2;
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

            _initialPosition = content.localPosition.x;
            _savedPosition = new Vector3(_initialPosition, 0);
            _focusedIndex = initialFocusIndex;

            UpdateFocus(_focusedIndex);

            /* Listeners */
            buttonLeft.OnClick += () => {Focus(false);};
            buttonRight.OnClick += () => {Focus(true);};
            
            UpdateButtonsState();
        }

        public Coroutine Focus(int childIndex)
        {
            if (childIndex < 0)
            {
                LogHelper.LogHelper.Log($"{nameof(childIndex)} Must be more than 0.", MethodBase.GetCurrentMethod(), LogHelper.LogHelper.LogType.Error);
                return null;
            }
            
            if (childIndex > ElementsCount)
            {
                childIndex = ElementsCount - 1;
            }
            
            _focusedIndex = childIndex;
            UpdateFocus(_focusedIndex);
            
            UpdateButtonsState();

            // right
            if (childIndex > (float) ElementsCount / 2)
            {
                _savedPosition = new Vector3(_initialPosition + childIndex * _childWidth, 0);
                return CoroutineHelper.RestartCoroutine
                (
                    ref _focusCoroutine,
                    CurveAnimationHelper.Move(content, _savedPosition, speedOrTime: focusSpeed),
                    this
                );
            }
            // left
            else
            {
                _savedPosition = new Vector3(-_initialPosition + childIndex * _childWidth, 0);
                return CoroutineHelper.RestartCoroutine
                (
                    ref _focusCoroutine,
                    CurveAnimationHelper.Move(content, _savedPosition, speedOrTime: focusSpeed),
                    this
                );
            }
        }

        public Coroutine Focus(bool right)
        {
            // right
            if (right)
            {
                if (_focusedIndex < ElementsCount - 1)
                {
                    _focusedIndex++;
                    UpdateFocus(_focusedIndex);
                    UpdateButtonsState();
                    
                    _savedPosition = new Vector3(_savedPosition.x - _childWidth, 0);
                    return CoroutineHelper.RestartCoroutine
                    (
                        ref _focusCoroutine,
                        CurveAnimationHelper.Move(content, _savedPosition, speedOrTime: focusSpeed),
                        this
                    );
                }
            }
            // left
            else
            {
                if (_focusedIndex > 0)
                {
                    _focusedIndex--;
                    UpdateFocus(_focusedIndex);
                    UpdateButtonsState();
                    
                    _savedPosition = new Vector3(_savedPosition.x + _childWidth, 0);
                    return CoroutineHelper.RestartCoroutine
                    (
                        ref _focusCoroutine,
                        CurveAnimationHelper.Move(content, _savedPosition, speedOrTime: focusSpeed),
                        this
                    );
                }
            }


            LogHelper.LogHelper.Log($"Can't focus, no elements in this direction. Right: {right}.", MethodBase.GetCurrentMethod());
            return null;
        }

        public bool CanFocus(bool right)
        {
            if (right)
            {
                if (_focusedIndex < ElementsCount - 1) return true;
            }
            else
            {
                if (_focusedIndex > 0) return true;
            }

            return false;
        }

        private void UpdateButtonsState()
        {
            if (dynamicButtons)
            {
                if (!CanFocus(true))
                {
                    buttonRight.Disable();
                }
                else
                {
                    buttonRight.Enable();
                }

                if (!CanFocus(false))
                {
                    buttonLeft.Disable();
                }
                else
                {
                    buttonLeft.Enable();
                } 
            }
        }
    }
}