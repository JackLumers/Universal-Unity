using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UniversalUnity.Helpers.UI.CommonPatterns.FillableElement
{
    public class UiFillableLine : AFillableUiElement
    {
        [Header("UiFillableLine Fields")]
        public FillType fillType;
        public RectTransform fillContainer;
        public Image fillLine;

        protected Rect ContainerRect;
        protected RectTransform FillLineTransform;

        protected float ContainerWidth;
        protected float ContainerHeight;
        protected float StepLength;

        private CancellationTokenSource _fillCancellationTokenSource = new CancellationTokenSource();

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
            ContainerWidth = ContainerRect.width;
            ContainerHeight = ContainerRect.height;
            
            switch (fillType)
            {
                case FillType.LeftToRight:
                case FillType.RightToLeft:
                    StepLength = ContainerWidth / maxAmount;
                    break;
                case FillType.DownToUp:
                case FillType.UpToDown:
                    StepLength = ContainerHeight / maxAmount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void InheritInitComponents()
        {
            ContainerRect = fillContainer.rect;
            FillLineTransform = (RectTransform) fillLine.transform;
            Recalculate();
            
            base.InheritInitComponents();
        }

        protected override async UniTask OnFill(float amount, float amountPerSecond)
        {
            float timeInSeconds = Math.Abs(amount - FilledAmount) / amountPerSecond;

            _fillCancellationTokenSource?.Cancel();
            _fillCancellationTokenSource?.Dispose();
            _fillCancellationTokenSource = new CancellationTokenSource();
            
            switch (fillType)
            {
                case FillType.LeftToRight:
                    await FillLineTransform.DOAnchorPos(
                            new Vector3(-ContainerWidth + StepLength * amount, 0),
                            timeInSeconds)
                        .WithCancellation(_fillCancellationTokenSource.Token)
                        .SuppressCancellationThrow();
                    break;
                
                case FillType.RightToLeft:
                    await FillLineTransform.DOAnchorPos(
                            new Vector3(ContainerWidth - StepLength * amount, 0), 
                            timeInSeconds)
                        .WithCancellation(_fillCancellationTokenSource.Token)
                        .SuppressCancellationThrow();
                    break;

                case FillType.DownToUp:
                    await FillLineTransform.DOAnchorPos(
                            new Vector3(0, -ContainerHeight + StepLength * amount), 
                            timeInSeconds)
                        .WithCancellation(_fillCancellationTokenSource.Token)
                        .SuppressCancellationThrow();
                    break;

                case FillType.UpToDown:
                    await FillLineTransform.DOAnchorPos(
                            new Vector3(0, ContainerHeight + StepLength * amount), 
                            timeInSeconds)
                        .WithCancellation(_fillCancellationTokenSource.Token)
                        .SuppressCancellationThrow();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnForceFill(float amount)
        {
            switch (fillType)
            {
                case FillType.LeftToRight:
                    FillLineTransform.anchoredPosition = new Vector3(-ContainerWidth + StepLength * amount, 0);
                    break;
                case FillType.RightToLeft:
                    FillLineTransform.anchoredPosition = new Vector3(ContainerWidth - StepLength * amount, 0);
                    break;
                case FillType.DownToUp:
                    FillLineTransform.anchoredPosition = new Vector3(0, -ContainerHeight + StepLength * amount);
                    break;
                case FillType.UpToDown:
                    FillLineTransform.anchoredPosition = new Vector3(0, ContainerHeight + StepLength * amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ContextMenu("[RuntimeTest] -> Test Fillable Line")]
        public async void Test()
        {
            await Enable();
            await TestProcess();
        }

        private async UniTask TestProcess()
        {
            await Fill(100);
            await Fill(0);
            await Fill(10);
            await Fill(50);
            await Fill(60);
            await Fill(100);
            await Fill(70);
            await Fill(0);
        }
    }
}