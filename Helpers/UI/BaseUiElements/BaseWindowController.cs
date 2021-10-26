using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    public class BaseWindowController<T> : GenericSingleton<T> where T: Component
    {
        [Header("= BaseWindowController Fields =")]
        [SerializeField] protected BaseUiElement uiContainer;
        
        [SerializeField] [CanBeNull] 
        protected BaseInteractableUiElement closeButton;

        public event Action OnCloseCalled;
        public event Action OnOpenCalled;
        public event Action OnClosed;
        public event Action OnOpened;
        
        public bool IsOpened => uiContainer.IsEnabled;

        protected override void InheritAwake()
        {
            base.InheritAwake();
            if (!(closeButton is null)) closeButton.OnClick += OnCloseButtonClick;
        }

        protected virtual void OnCloseButtonClick()
        {
            Close().Forget();
        }

        public virtual async UniTask Open()
        {
            OnOpenCalled?.Invoke();
            await uiContainer.Enable();
            OnOpened?.Invoke();
        }

        public virtual async UniTask Close()
        {
            OnCloseCalled?.Invoke();
            await uiContainer.Disable();
            OnClosed?.Invoke();
        }

        public virtual void ForceClose()
        {
            OnCloseCalled?.Invoke();
            uiContainer.ForceDisable();
            OnClosed?.Invoke();
        }
        
        public virtual void ForceOpen()
        {
            OnOpenCalled?.Invoke();
            uiContainer.ForceEnable();
            OnOpened?.Invoke();
        }

        public void SetClosable(bool closable)
        {
            if (ReferenceEquals(closeButton, null))
            {
                LogHelper.LogError("Trying to make window closable without close button", nameof(SetClosable));                
            }
            else
            {
                closeButton.EnableOrDisable(closable, true).Forget();
            }
        }
    }
}