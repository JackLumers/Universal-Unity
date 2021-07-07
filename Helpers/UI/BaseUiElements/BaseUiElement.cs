using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.Tweeks.CurveAnimationHelper;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    /// <summary>
    /// Base class for all UI elements.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Animator))]
    public class BaseUiElement : CachedMonoBehaviour
    {
        [Header("= Base UI Element Fields =")]
        protected CanvasGroup CanvasGroup;
        protected Animator Animator;

        protected readonly Dictionary<string, AnimationClip> AnimationClips = new Dictionary<string, AnimationClip>();

        private CancellationTokenSource _enableCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _movingCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _alphaChangeCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _rotationCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _scalingCancellationTokenSource = new CancellationTokenSource();
        
        protected bool IsInitialized { get; private set; }

        public bool IsEnabled { get; protected set; }
        
        public bool WasEnabled { get; protected set; }
        
        public float EnableAnimationTime { get; set; }
        
        private void Awake()
        {
            if(!IsInitialized) InitComponents();
            InheritAwake();
        }
        
        protected virtual void InheritAwake()
        {
        }

        /// <summary>
        /// Called ONCE after <see cref="Awake"/> or in <see cref="Enable"/>, <see cref="Disable"/> methods, if was not called before.
        /// Initialize here components or any other objects that is not changing references later.
        /// <para>Must be called in inheritors before using any methods or fields!</para>
        /// <remarks>Also can be called again if <see cref="DeInitAndRefresh"/> was called.</remarks>
        /// </summary>
        public void InitComponents(bool calledByEnable = false)
        {
            if (!IsInitialized)
            {
                CanvasGroup = GetComponent<CanvasGroup>();
                Animator = GetComponent<Animator>();

                foreach (var animationClip in Animator.runtimeAnimatorController.animationClips)
                {
                    AnimationClips.Add(animationClip.name, animationClip);
                }
                
                IsInitialized = true;
                
                InheritInitComponents();
                
                if (!calledByEnable && !WasEnabled && CanvasGroup.alpha > 0.99 && gameObject.activeInHierarchy) Enable();
            }
            else
            {
                Debug.LogWarning("[BaseUiElement] Already initialized! Call rejected.");
            }
        }

        /// <summary>
        /// Called after <see cref="InitComponents"/>. Use to write additional init logic.
        /// </summary>
        protected virtual void InheritInitComponents()
        {
        }
        
        #region Raycast Block Logic

        private HashSet<string> _blockContexts = new HashSet<string>();

        [ContextMenu("> Write all RayCasts block contexts in console")]
        public void LogAllRayCastBlockContexts()
        {
            Debug.Log($"[{name}] Beginning of RayCasts block contexts...");
            foreach (var blockContext in _blockContexts)
            {
                Debug.Log(blockContext);
            }

            Debug.Log($"[{name}] End of RayCasts block contexts.");
        }

        /// <summary>
        /// Returns false if any context blocks raycast
        /// </summary>
        /// <returns></returns>
        public bool Interactable()
        {
            return !_blockContexts.Any();
        }

        public bool CheckBlockContext(string context)
        {
            return _blockContexts.Contains(context);
        }

        /// <summary>
        /// Use to disable all click events on this item.
        /// </summary>
        public void InteractionBlock(string context, bool block, bool suppressWarnings = false)
        {
            if (!IsInitialized) InitComponents();
            if (block)
            {
                if (!_blockContexts.Add(context) && !suppressWarnings)
                {
                    Debug.LogWarning("[BaseUserInterfaceItem.RayCastBlock] There is already such context added!");
                }
            }
            else
            {
                if (_blockContexts.Remove(context) && !suppressWarnings)
                {
                    Debug.LogWarning("[BaseUserInterfaceItem.RayCastBlock] There is no such context to remove!");
                }
            }

            CanvasGroup.blocksRaycasts = !_blockContexts.Any();
            CanvasGroup.interactable = !_blockContexts.Any();
        }

        #endregion

        /// <summary>
        /// Use to make element like in first load.
        /// <remarks>
        /// <see cref="IsEnabled"/> value depends on this <see cref="GameObject"/> state (active or not).
        /// </remarks>
        /// </summary>
        public void DeInitAndRefresh()
        {
            if (IsInitialized)
            {
                IsEnabled = gameObject.activeInHierarchy;
                WasEnabled = false;
                IsInitialized = false;
                _blockContexts.Clear();
            
                InheritDeInitAndRefresh();
            }
        }

        /// <summary>
        /// See <see cref="DeInitAndRefresh"/>. Use to write additional deinit logic.
        /// </summary>
        protected virtual void InheritDeInitAndRefresh()
        {
            
        }
        
        public async UniTask Enable(bool enable, bool forceSwitchBlockInput)
        {
            switch (enable)
            {
                case true:
                    await Enable(forceSwitchBlockInput);
                    break;
                case false:
                    await Disable(forceSwitchBlockInput);
                    break;
            }
        }
        
        public void ForceEnable(bool enable)
        {
            switch (enable)
            {
                case true:
                    ForceEnable();
                    break;
                case false:
                    ForceDisable();
                    break;
            }
        }

        /// <summary>
        /// Enables item with animation.
        /// </summary>
        public async UniTask Enable(bool forceEnableInput = true)
        {
            if (!IsInitialized)
            {
                InitComponents(true);
            }

            if (!WasEnabled)
            {
                WasEnabled = true;
                if (CanvasGroup.alpha > 0.99f)
                {
                    CanvasGroup.alpha = 0;
                }
            }

            gameObject.SetActive(true);
            if (forceEnableInput) InteractionBlock("Disabled", false, true);
            IsEnabled = true;

            Animator.Play(AnimationConstants.GetAnimatorHash(AnimationParamNames.Enabling));

            var millis = AnimationClips[AnimationParamNames.Enabling.ToString()].length * 1000;
            var speed = Animator.speed;
            _enableCancellationTokenSource.Cancel();
            _enableCancellationTokenSource = new CancellationTokenSource();
            await UniTask.Delay((int) (millis * speed),
                cancellationToken: _enableCancellationTokenSource.Token);

            OnEnabled();
        }

        /// <summary>
        /// Disables item with animation.
        /// </summary>
        public async UniTask Disable(bool forceDisableInput = true)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (!IsInitialized) InitComponents();
            if (forceDisableInput) InteractionBlock("Disabled", true, true);
            IsEnabled = false;
            Animator.Play(AnimationConstants.GetAnimatorHash(AnimationParamNames.Enabling));

            var millis = AnimationClips[AnimationParamNames.Disabling.ToString()].length * 1000;
            var speed = Animator.speed;

            _enableCancellationTokenSource.Cancel();
            _enableCancellationTokenSource = new CancellationTokenSource();
            await UniTask.Delay((int) (millis * speed),
                cancellationToken: _enableCancellationTokenSource.Token);

            gameObject.SetActive(false);
            OnDisabled();
        }

        public void ForceDisable()
        {
            if (!IsInitialized) InitComponents();
            _enableCancellationTokenSource.Cancel();
            InteractionBlock("Disabled", true, true);
            IsEnabled = false;
            CanvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        public void ForceEnable()
        {
            if (!IsInitialized)
            {
                InitComponents();
            }

            if (!WasEnabled)
            {
                WasEnabled = true;
            }
            
            _enableCancellationTokenSource.Cancel();
            gameObject.SetActive(true);
            InteractionBlock("Disabled", false, true);
            IsEnabled = true;
            CanvasGroup.alpha = 1;
        }
        
        protected virtual void OnEnabled()
        {
            InteractionBlock("Disabled", false, true);
        }
        
        protected virtual void OnDisabled()
        {
            InteractionBlock("Disabled", true, true);
            gameObject.SetActive(false);
        }

        public async UniTask Move(Vector3 targetLocalPosition, float timeOrSpeed, bool fixedTime, [CanBeNull] AnimationCurve curve)
        {
            _movingCancellationTokenSource.Cancel();
            _movingCancellationTokenSource = new CancellationTokenSource();
            await CurveAnimationHelper.MoveAnchored((RectTransform) transform, targetLocalPosition,
                speedOrTime: timeOrSpeed,
                fixedTime: fixedTime, curve: curve, cancellationToken: _movingCancellationTokenSource.Token);
        }

        public async UniTask Rotate(Quaternion targetLocalRotation, float timeOrSpeed, bool fixedTime)
        {
            _rotationCancellationTokenSource.Cancel();
            _rotationCancellationTokenSource = new CancellationTokenSource();

            await CurveAnimationHelper.Rotate(transform, targetLocalRotation, speedOrTime: timeOrSpeed,
                fixedTime: fixedTime, cancellationToken: _rotationCancellationTokenSource.Token);
        }

        public async UniTask Scale(Vector3 targetLocalScale, float timeOrSpeed, bool fixedTime)
        {
            _scalingCancellationTokenSource.Cancel();
            _scalingCancellationTokenSource = new CancellationTokenSource();
            
            await CurveAnimationHelper.Scale(transform, targetLocalScale, speedOrTime: timeOrSpeed,
                fixedTime: fixedTime, cancellationToken: _scalingCancellationTokenSource.Token);
        }

        public async UniTask ChangeAlpha(float targetAlpha, float timeInSeconds)
        {
            _alphaChangeCancellationTokenSource.Cancel();
            _alphaChangeCancellationTokenSource = new CancellationTokenSource();
            
            await CurveAnimationHelper.LerpFloatByCurve
            (
                result => CanvasGroup.alpha = result,
                CanvasGroup.alpha,
                targetAlpha,
                timeOrSpeed: timeInSeconds,
                fixedTime: true,
                cancellationToken: _alphaChangeCancellationTokenSource.Token
            );
        }
        
        public void SetAlphaImmediately(float targetAlpha)
        {
            if (!IsInitialized) InitComponents();
            CanvasGroup.alpha = targetAlpha;
        }
    }
}
