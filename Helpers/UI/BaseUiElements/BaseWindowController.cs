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

        public bool IsOpened => uiContainer.IsEnabled;

        protected override void InheritAwake()
        {
            base.InheritAwake();
            if (!(closeButton is null)) closeButton.OnClick += async () => await Close();
        }

        public virtual async UniTask Open()
        {
            await uiContainer.Enable();
        }

        public virtual async UniTask Close()
        {
            await uiContainer.Disable();
        }

        public virtual void ForceClose()
        {
            uiContainer.ForceDisable();
        }
        
        public virtual void ForceOpen()
        {
            uiContainer.ForceEnable();
        }

        public void SetClosable(bool closable)
        {
            closeButton?.Enable(closable);
        }
    }
}