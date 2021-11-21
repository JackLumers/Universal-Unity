using System;
using Common.Helpers.ScriptableObjects;
using Cysharp.Threading.Tasks;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.ScriptableObjects
{
    public abstract class BaseScriptableElement<T> : ScriptablePrefabInstance<T> where T: BaseUiElement
    {
        public event Action OnDisableCalled;
        public event Action OnEnableCalled;
        public event Action OnDisabled;
        public event Action OnEnabled;

        public virtual async UniTask<T> Enable()
        {
            OnEnableCalled?.Invoke();
            await Get().Enable();
            OnEnabled?.Invoke();
            return PrefabInstance;
        }

        public virtual async UniTaskVoid Disable()
        {
            if (PrefabInstance != null)
            {
                OnDisableCalled?.Invoke();
                await PrefabInstance.Disable();
                Return();
                OnDisabled?.Invoke();
            }
        }
    }
}