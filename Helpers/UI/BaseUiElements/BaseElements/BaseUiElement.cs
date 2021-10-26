using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.MonoBehaviourExtenders;

namespace UniversalUnity.Helpers.UI.BaseUiElements.BaseElements
{
    /// <summary>
    /// Base class for all UI elements.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseUiElement : CachedMonoBehaviour
    {
        [Header("= Base UI Element Fields =")]
        protected CanvasGroup CanvasGroup;

        [SerializeField]
        private float defaultEnableAnimationTime = 0.25f;
        
        private CancellationTokenSource _enableCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _disableCancellationTokenSource = new CancellationTokenSource();

        protected bool IsInitialized { get; private set; }

        public bool IsEnabled { get; protected set; }
        
        public bool WasEnabled { get; protected set; }

        public float EnableAnimationTime { get; set; } = 0.25f;
        
        public event Action OnEnableCalled;
        public event Action OnDisableCalled;
        public event Action OnEnabled;
        public event Action OnDisabled;
        
        private void Awake()
        {
            if(!IsInitialized) InitComponents();
            InheritAwake();
        }
        
        protected virtual void InheritAwake()
        {
        }

        /// <summary>
        /// Called ONCE after <see cref="Awake"/> or in <see cref="Enable(bool)"/>, <see cref="Disable"/> methods, if was not called before.
        /// Initialize here components or any other objects that is not changing references later.
        /// <para>Must be called in inheritors before using any methods or fields!</para>
        /// <remarks>Also can be called again if <see cref="DeInitAndRefresh"/> was called.</remarks>
        /// </summary>
        public void InitComponents(bool calledByEnable = false)
        {
            if (!IsInitialized)
            {
                CanvasGroup = GetComponent<CanvasGroup>();
                EnableAnimationTime = defaultEnableAnimationTime;
                IsInitialized = true;
                
                InheritInitComponents();

                if (!calledByEnable && !WasEnabled && CanvasGroup.alpha > 0.99 && gameObject.activeInHierarchy) 
                    Enable().Forget();
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
        
        public async UniTask EnableOrDisable(bool enable, bool forceBlockInputSwitch)
        {
            switch (enable)
            {
                case true:
                    await Enable(forceBlockInputSwitch);
                    break;
                case false:
                    await Disable(forceBlockInputSwitch);
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
            
            OnEnableCalled?.Invoke();

            _disableCancellationTokenSource.Cancel();
            _enableCancellationTokenSource.Cancel();
            _enableCancellationTokenSource = new CancellationTokenSource();
            
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

            await CanvasGroup
                .DOFade(1, EnableAnimationTime)
                .WithCancellation(_enableCancellationTokenSource.Token)
                .SuppressCancellationThrow();

            if (!_enableCancellationTokenSource.IsCancellationRequested)
            {
                OnEnableAnimationComplete();
            }
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
            
            OnDisableCalled?.Invoke();
            
            _enableCancellationTokenSource.Cancel();
            _disableCancellationTokenSource.Cancel();
            _disableCancellationTokenSource = new CancellationTokenSource();
            
            await CanvasGroup
                .DOFade(0, EnableAnimationTime)
                .WithCancellation(_disableCancellationTokenSource.Token)
                .SuppressCancellationThrow();
            
            if (!_disableCancellationTokenSource.IsCancellationRequested)
            {
                OnDisableAnimationComplete();
            }
        }

        public void ForceDisable()
        {
            if (!IsInitialized) InitComponents();
            _enableCancellationTokenSource.Cancel();
            InteractionBlock("Disabled", true, true);
            IsEnabled = false;
            CanvasGroup.alpha = 0;
            gameObject.SetActive(false);
            OnDisableAnimationComplete();
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
            OnEnableAnimationComplete();
        }
        
        protected virtual void OnEnableAnimationComplete()
        {
            LogHelper.LogInfo("Enabled!", nameof(OnEnableAnimationComplete));
            InteractionBlock("Disabled", false, true);
            OnEnabled?.Invoke();
        }

        protected virtual void OnDisableAnimationComplete()
        {
            LogHelper.LogInfo("Disabled!", nameof(OnDisableAnimationComplete));
            gameObject.SetActive(false);
            InteractionBlock("Disabled", true, true);
            OnDisabled?.Invoke();
        }

        public void SetAlphaImmediately(float targetAlpha)
        {
            if (!IsInitialized) InitComponents();
            CanvasGroup.alpha = targetAlpha;
        }
    }
}
