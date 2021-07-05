using System.Collections;
using Common.Helpers.Tweeks.CurveAnimationHelper;
using UnityEngine;

namespace Common.Helpers.UI.CommonPatterns.AnimatedElements
{
    [RequireComponent(typeof(RectTransform))]
    public class FloatingAnimatedElement : BaseAnimatedElement
    {
        [Header("= FloatingAnimatedElement Fields =")] 
        [SerializeField] private float floatingOnePeriodTime = 1f; // In seconds
        [SerializeField] private float floatingStrength = 5f; // how many units for up and down

        private RectTransform _rectTransform;
        private Vector3 _startPosition;
        private Vector3 _upPosition;
        private Vector3 _downPosition;

        protected override void InheritAwake()
        {
            base.InheritAwake();
            _rectTransform = (RectTransform) transform;
            _startPosition = _rectTransform.anchoredPosition;
            _upPosition = _startPosition + Vector3.up * floatingStrength;
            _downPosition = _startPosition + Vector3.down * floatingStrength;
        }

        protected override IEnumerator AnimationProcess()
        {
            // first positioning
            yield return CurveAnimationHelper.MoveAnchored(_rectTransform, _upPosition,
                speedOrTime: floatingOnePeriodTime / 2f);
            yield return CurveAnimationHelper.MoveAnchored(_rectTransform, _downPosition,
                speedOrTime: floatingOnePeriodTime);
            
            while (gameObject.activeInHierarchy)
            {
                yield return CurveAnimationHelper.MoveAnchored(_rectTransform, _upPosition,
                    speedOrTime: floatingOnePeriodTime);
                yield return CurveAnimationHelper.MoveAnchored(_rectTransform, _downPosition,
                    speedOrTime: floatingOnePeriodTime);
            }
        }
    }
}