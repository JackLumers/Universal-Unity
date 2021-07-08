using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.ScriptableObjects
{
    public abstract class AScriptableWindow<T> : ScriptableObject where T: BaseUiElement
    {
        [SerializeField] private ScriptablePrefab parent;
        [SerializeField] private T windowPrefab;
        [NonSerialized] private T _windowInstance;
        
        private void Awake()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    _windowInstance = null;
                }
            };
#endif
        }

        public virtual T Get()
        {
            if (_windowInstance != null)
            {
                return _windowInstance;
            }
            else
            {
                _windowInstance = Instantiate(windowPrefab, parent.Get().transform);
                return _windowInstance;
            }
        }

        public async UniTask Return(bool withParent)
        {
            if (_windowInstance != null)
            {
                await _windowInstance.Disable();
                if (withParent)
                {
                    parent.Return();
                }
                else
                {
                    Destroy(_windowInstance);
                }
            }
        }

        public virtual async UniTask<T> Open()
        {
            if (_windowInstance != null)
            {
                await _windowInstance.Enable();
                return _windowInstance;
            }
            else
            {
                _windowInstance = Instantiate(windowPrefab, parent.Get().transform);
                await _windowInstance.Enable();
                return _windowInstance;
            }
        }

        public virtual async UniTask Close()
        {
            if (_windowInstance != null)
            {
                await _windowInstance.Disable();
            }
        }
    }
}