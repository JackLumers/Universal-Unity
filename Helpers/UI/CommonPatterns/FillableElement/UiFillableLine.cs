using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniversalUnity.Helpers.Coroutines;
using UniversalUnity.Helpers.Tweeks.CurveAnimationHelper;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FillableElement
{
    public class UiFillableLine : AFillableUiElement
    {
        [Header("UiFillableLine Fields")]
        public FillType fillType;
        public RectTransform fillContainer;
        public Image fillLine;

        protected Rect _containerRect;
        protected RectTransform _fillLineTransform;

        protected float _containerWidth;
        protected float _containerHeight;
        protected float _stepLength;

        private Coroutine _fillCoroutine;

        public enum FillType
        {
            LeftToRight,
            RightToLeft,
            DownToUp,
            UpToDown
        }

        /// <summary>
        /// Call if any transform width, height or <see cref="fillType"/>, <see cref="AFillableUiElement.maxAmount"/> changed
        /// </summary>
        public void Recalculate()
        {
            _containerWidth = _containerRect.width;
            _containerHeight = _containerRect.height;
            
            switch (fillType)
            {
                case FillType.LeftToRight:
                case FillType.RightToLeft:
                    _stepLength = _containerWidth / maxAmount;
                    break;
                case FillType.DownToUp:
                case FillType.UpToDown:
                    _stepLength = _containerHeight / maxAmount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void InheritInitComponents()
        {
            _containerRect = fillContainer.rect;
            _fillLineTransform = (RectTransform) fillLine.transform;
            Recalculate();
            
            base.InheritInitComponents();
        }

        protected override Coroutine OnFill(float amount, float amountPerSecond, Action onFilled)
        {
            float timeInSeconds = Math.Abs(amount - FilledAmount) / amountPerSecond;
                
            switch (fillType)
            {
                case FillType.LeftToRight:
                    return CoroutineHelper.RestartCoroutine(
                        ref _fillCoroutine,
                        CurveAnimationHelper.MoveAnchored
                        (
                            _fillLineTransform,
                            new Vector3(-_containerWidth + _stepLength * amount, 0),
                            speedOrTime: timeInSeconds, onDone: onFilled
                        ),
                        this
                    );
                case FillType.RightToLeft:
                    return CoroutineHelper.RestartCoroutine(
                        ref _fillCoroutine,
                        CurveAnimationHelper.MoveAnchored
                        (
                            _fillLineTransform,
                            new Vector3(_containerWidth - _stepLength * amount, 0),
                            speedOrTime: timeInSeconds, onDone: onFilled
                        ),
                        this
                    );
                case FillType.DownToUp:
                    return CoroutineHelper.RestartCoroutine(
                        ref _fillCoroutine,
                        CurveAnimationHelper.MoveAnchored
                        (
                            _fillLineTransform,
                            new Vector3(0, -_containerHeight + _stepLength * amount),                   
                            speedOrTime: timeInSeconds, onDone: onFilled
                        ),
                        this
                    );
                case FillType.UpToDown:
                    return CoroutineHelper.RestartCoroutine(
                        ref _fillCoroutine,
                        CurveAnimationHelper.MoveAnchored
                        (
                            _fillLineTransform,
                            new Vector3(0, _containerHeight + _stepLength * amount),
                            speedOrTime: timeInSeconds, onDone: onFilled
                        ),
                        this
                    );
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnForceFill(float amount)
        {
            switch (fillType)
            {
                case FillType.LeftToRight:
                    _fillLineTransform.anchoredPosition = new Vector3(-_containerWidth + _stepLength * amount, 0);
                    break;
                case FillType.RightToLeft:
                    _fillLineTransform.anchoredPosition = new Vector3(_containerWidth - _stepLength * amount, 0);
                    break;
                case FillType.DownToUp:
                    _fillLineTransform.anchoredPosition = new Vector3(0, -_containerHeight + _stepLength * amount);
                    break;
                case FillType.UpToDown:
                    _fillLineTransform.anchoredPosition = new Vector3(0, _containerHeight + _stepLength * amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ContextMenu("[RuntimeTest] -> Test Fillable Line")]
        public void Test()
        {
            Enable();
            StartCoroutine(TestProcess());
        }

        private IEnumerator TestProcess()
        {
            yield return Fill(100);
            yield return Fill(0);
            yield return Fill(10);
            yield return Fill(50);
            yield return Fill(60);
            yield return Fill(100);
            yield return Fill(70);
            yield return Fill(0);
        }
    }
}