using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UniversalUnity.Helpers.UI.AnimatedElements
{
    [RequireComponent(typeof(RectTransform))]
    public class FloatingAnimatedElement : BaseAnimatedElement
    {
        [Header("= FloatingAnimatedElement Fields =")] 
        [SerializeField] private float floatingOnePeriodTime = 1f; // In seconds
        [SerializeField] private float floatingStrength = 5f; // how many units for up and down
        [SerializeField] private bool playOnAwake;
        [SerializeField] private bool playOnEnable;
        
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
            
            if (playOnAwake) StartAnimation().Forget();
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                StartAnimation().Forget();
            }
        }

        private void OnDisable()
        {
            ResetToInitialState();
        }

        public override async UniTask StopAnimation()
        {
            AnimationCancellationTokenSource?.Cancel();
            AnimationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            await _rectTransform.DOAnchorPos(_startPosition, floatingOnePeriodTime / 2f)
                .WithCancellation(AnimationCancellationTokenSource.Token);
        }

        public override async UniTaskVoid StartAnimation()
        {
            AnimationCancellationTokenSource?.Cancel();
            AnimationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            await _rectTransform.DOAnchorPos(_upPosition, floatingOnePeriodTime / 2f)
                .WithCancellation(AnimationCancellationTokenSource.Token);

            await _rectTransform.DOAnchorPos(_downPosition, floatingOnePeriodTime)
                .WithCancellation(AnimationCancellationTokenSource.Token);

            while (gameObject.activeInHierarchy && !AnimationCancellationTokenSource.IsCancellationRequested)
            {
                await _rectTransform.DOAnchorPos(_upPosition, floatingOnePeriodTime)
                    .WithCancellation(AnimationCancellationTokenSource.Token);

                await _rectTransform.DOAnchorPos(_downPosition, floatingOnePeriodTime)
                    .WithCancellation(AnimationCancellationTokenSource.Token);
            }
        }

        public override void ResetToInitialState()
        {
            AnimationCancellationTokenSource?.Cancel();
            _rectTransform.anchoredPosition = _startPosition;
        }
    }
}