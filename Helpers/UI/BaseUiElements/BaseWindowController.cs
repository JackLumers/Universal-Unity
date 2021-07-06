using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.MonoBehaviourExtenders;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    public class BaseWindowController<T> : GenericSingleton<T> where T: Component
    {
        [Header("= BaseWindowController Fields =")]
        [SerializeField] protected BaseUiElement uiContainer;
        
        [SerializeField] [CanBeNull] 
        protected BaseInteractableUiElement closeButton;

        public event Action OnClosed;
        public event Action OnOpened;
        
        public bool IsOpened => uiContainer.IsEnabled;

        protected override void InheritAwake()
        {
            base.InheritAwake();
            if (!(closeButton is null)) closeButton.OnClick += OnCloseButtonClick;
        }

        public virtual async UniTask Open([CanBeNull] Action onOpened = null)
        {
            await uiContainer.Enable(() =>
            {
                OnOpened?.Invoke();
                onOpened?.Invoke();
            });
        }

        public virtual async UniTask Close([CanBeNull] Action onClosed = null)
        {
            await uiContainer.Disable(() =>
            {
                OnClosed?.Invoke();
                onClosed?.Invoke();
            });
        }

        public virtual void ForceClose()
        {
            uiContainer.ForceDisable();
        }
        
        public virtual void ForceOpen()
        {
            uiContainer.ForceEnable();
        }
        
        public void RunClose()
        {
            Close();
        }

        public void SetClosable(bool closable)
        {
            closeButton?.Enable(closable);
        }
        
        protected virtual void OnCloseButtonClick()
        {
            Close();
        }
    }
}