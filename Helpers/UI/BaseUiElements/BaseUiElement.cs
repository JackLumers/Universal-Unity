using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Coroutines;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.Tweeks.CurveAnimationHelper;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    /// <summary>
    /// Base class for all UI elements.
    /// By using methods from this class, elements can be rotated, moved, enabled and disabled with animation.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseUiElement : CachedMonoBehaviour
    {
        [Header("= Base UI Element Fields =")] 
        [SerializeField] protected float enableAnimationTime = 0.5f;
        [Space]
        
        protected CanvasGroup CanvasGroup;

        protected Coroutine EnableCoroutine;
        protected Coroutine DisableCoroutine;

        protected Coroutine MovingCoroutine;
        protected Coroutine AlphaCoroutine;
        protected Coroutine RotationCoroutine;
        protected Coroutine ScalingCoroutine;

        protected bool IsInitialized { get; private set; }

        public bool IsEnabled { get; protected set; }
        
        public bool WasEnabled { get; protected set; }

        public float EnableAnimationTime
        {
            get => enableAnimationTime;
            set => enableAnimationTime = value;
        }

        
        private void Awake()
        {
            if(!IsInitialized) InitComponents();
            InheritAwake();
        }
        
        protected virtual void InheritAwake()
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
        /// Returns true if any context blocks raycast
        /// </summary>
        /// <returns></returns>
        public bool IsRayCastBlocked()
        {
            return _blockContexts.Any();
        }

        public bool CheckBlockContext(string context)
        {
            return _blockContexts.Contains(context);
        }

        /// <summary>
        /// Use to disable all click events on this item without changing button colors.
        /// </summary>
        public void RayCastBlock(string context, bool block, bool suppressWarnings = false)
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
        }

        public bool Interactable
        {
            get
            {
                if (!IsInitialized)
                {
                    InitComponents();
                }

                return CanvasGroup.interactable;
            }
            set
            {
                if (!IsInitialized)
                {
                    InitComponents();
                }

                CanvasGroup.interactable = value;
            }
        }

        #endregion

        /// <summary>
        /// Called ONCE after <see cref="Awake"/> or in <see cref="Enable"/>, <see cref="Disable"/> methods, if was not called before.
        /// Initialize here components or any other objects that is not changing references later.
        /// <para>Must be called in inheritors before using any methods or fields!</para>
        /// <remarks>Also can be called again if <see cref="DeInitAndRefresh"/> was called.</remarks>
        /// </summary>
        protected void InitComponents(bool calledByEnable = false)
        {
            if (!IsInitialized)
            {
                CanvasGroup = GetComponent<CanvasGroup>();
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

        /// <summary>
        /// Enables/Disables item with coroutine animation. Inheritors can modify this animations if needed.
        /// <see cref="EnablingAnimation"/> and <see cref="DisablingAnimation"/>
        /// </summary>
        public virtual Coroutine Enable(bool enable, [CanBeNull] Action onDone = null,
            bool forceSwitchBlockInput = true)
        {
            return enable
                ? Enable(onDone, forceSwitchBlockInput)
                : Disable(onDone, forceSwitchBlockInput);
        }

        /// <summary>
        /// Enables item with coroutine animation. Inheritors can modify this animations if needed.
        /// <see cref="EnablingAnimation"/>
        /// </summary>
        public virtual Coroutine Enable([CanBeNull]Action onEnabled = null, bool forceEnableInput = true)
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
            
            if (DisableCoroutine != null) StopCoroutine(DisableCoroutine);
            gameObject.SetActive(true);
            if(forceEnableInput) RayCastBlock("Disabled", false, true);
            IsEnabled = true;
            EnableCoroutine = CoroutineHelper.RestartCoroutine(ref EnableCoroutine, EnablingAnimation(onEnabled), this);
            return EnableCoroutine;
        }

        /// <summary>
        /// Disables item with coroutine animation. Inheritors can modify this animations if needed.
        /// <see cref="DisablingAnimation"/>
        /// </summary>
        public virtual Coroutine Disable([CanBeNull]Action onDisabled = null, bool forceDisableInput = true)
        {
            if (!gameObject.activeInHierarchy)
            {
                ForceDisable();
                return null;
            }
            if (!IsInitialized) InitComponents();
            if (EnableCoroutine != null) StopCoroutine(EnableCoroutine);
            if(forceDisableInput) RayCastBlock("Disabled", true, true);
            IsEnabled = false;
            DisableCoroutine = CoroutineHelper.RestartCoroutine(ref DisableCoroutine, DisablingAnimation(onDisabled), this);
            return DisableCoroutine;
        }
        
        public void ForceDisable([CanBeNull]Action onDisabled = null)
        {
            if (!IsInitialized) InitComponents();
            if (DisableCoroutine != null) StopCoroutine(DisableCoroutine);
            if (EnableCoroutine != null) StopCoroutine(EnableCoroutine);
            RayCastBlock("Disabled", true, true);
            IsEnabled = false;
            CanvasGroup.alpha = 0;
            onDisabled?.Invoke();
            gameObject.SetActive(false);
        }

        public void ForceEnable([CanBeNull]Action onEnabled = null)
        {
            if (!IsInitialized)
            {
                InitComponents();
            }

            if (!WasEnabled)
            {
                WasEnabled = true;
            }
            
            if (DisableCoroutine != null) StopCoroutine(DisableCoroutine);
            if (EnableCoroutine != null) StopCoroutine(EnableCoroutine);
            gameObject.SetActive(true);
            RayCastBlock("Disabled", false, true);
            IsEnabled = true;
            CanvasGroup.alpha = 1;
            onEnabled?.Invoke();
        }

        /// <summary>
        /// Coroutine animation that used for enabling. Inheritors can modify this animations if needed.
        /// </summary>
        protected virtual IEnumerator EnablingAnimation([CanBeNull]Action onEnabled)
        {
            yield return CurveAnimationHelper.LerpFloatByCurve(
                result => CanvasGroup.alpha = result, CanvasGroup.alpha, 1, timeOrSpeed: enableAnimationTime);
            RayCastBlock("Disabled", false, true);
            onEnabled?.Invoke();
        }

        /// <summary>
        /// Coroutine animation that used for disabling. Inheritors can modify this animations if needed.
        /// </summary>
        protected virtual IEnumerator DisablingAnimation([CanBeNull]Action onDisabled)
        {
            yield return CurveAnimationHelper.LerpFloatByCurve(
                result => CanvasGroup.alpha = result, CanvasGroup.alpha, 0, timeOrSpeed: enableAnimationTime);
            RayCastBlock("Disabled", true, true);
            onDisabled?.Invoke();
            gameObject.SetActive(false);
        }

        public Coroutine Move(Vector3 targetLocalPosition, float timeOrSpeed, bool fixedTime, [CanBeNull] AnimationCurve curve)
        {
            return CoroutineHelper.RestartCoroutine
            (
                ref MovingCoroutine,
                CurveAnimationHelper.MoveAnchored((RectTransform)transform, targetLocalPosition, speedOrTime: timeOrSpeed,
                    fixedTime: fixedTime, curve: curve),
                this
            );
        }

        public Coroutine Rotate(Quaternion targetLocalRotation, float timeOrSpeed, bool fixedTime)
        {
            return CoroutineHelper.RestartCoroutine
            (
                ref RotationCoroutine,
                CurveAnimationHelper.Rotate(transform, targetLocalRotation, speedOrTime: timeOrSpeed,
                    fixedTime: fixedTime),
                this
            );
        }

        public Coroutine Scale(Vector3 targetLocalScale, float timeOrSpeed, bool fixedTime)
        {
            return CoroutineHelper.RestartCoroutine
            (
                ref ScalingCoroutine,
                CurveAnimationHelper.Scale(transform, targetLocalScale, speedOrTime: timeOrSpeed, fixedTime: fixedTime),
                this
            );
        }

        public Coroutine ChangeAlpha(float targetAlpha, float timeInSeconds)
        {
            return CoroutineHelper.RestartCoroutine
            (
                ref AlphaCoroutine,
                CurveAnimationHelper.LerpFloatByCurve
                (
                    result => CanvasGroup.alpha = result,
                    CanvasGroup.alpha,
                    targetAlpha,
                    timeOrSpeed: timeInSeconds,
                    fixedTime: true
                ),
                this
            );
        }
        
        public void SetAlphaImmediately(float targetAlpha)
        {
            if (!IsInitialized) InitComponents();
            CanvasGroup.alpha = targetAlpha;
        }
    }
}
