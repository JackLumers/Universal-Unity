using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UniversalUnity.Helpers.UI.AnimatedElements
{
    public class ScalingBubbleAnimatedElement : BaseAnimatedElement
    {
        [SerializeField] private bool playOnEnable;
        
        private RectTransform _rectTransform;
        private Vector3 _startScale;

        protected override void InheritAwake()
        {
            base.InheritAwake();
            _rectTransform = (RectTransform) transform;
            _startScale = _rectTransform.localScale;
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

        void OnDestroy()
        {
            AnimationCancellationTokenSource?.Cancel();
            AnimationCancellationTokenSource?.Dispose();
        }
        
        public override async UniTask StopAnimation()
        {
            AnimationCancellationTokenSource?.Cancel();
            AnimationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            
            await _rectTransform.DOScale(new Vector3(_startScale.x, _startScale.y, 1f), 1f)
                .WithCancellation(AnimationCancellationTokenSource.Token);
        }

        public override async UniTaskVoid StartAnimation()
        {
            AnimationCancellationTokenSource?.Cancel();
            AnimationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            while (!AnimationCancellationTokenSource.IsCancellationRequested && gameObject.activeInHierarchy)
            {
                await _rectTransform.DOScale(new Vector3(_startScale.x * 1.15f, _startScale.y * 1.15f, 1f), 1f)
                    .WithCancellation(AnimationCancellationTokenSource.Token);
                await _rectTransform.DOScale(new Vector3(_startScale.x, _startScale.y, 1f), 1f)
                    .WithCancellation(AnimationCancellationTokenSource.Token);
            }
        }

        public override void ResetToInitialState()
        {
            AnimationCancellationTokenSource?.Cancel();
            _rectTransform.localScale = _startScale;
        }
    }
}