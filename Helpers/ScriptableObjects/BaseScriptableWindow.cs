using System;
using UnityEditor;
using UnityEngine;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.ScriptableObjects
{
    public abstract class BaseScriptableWindow<T> : ScriptableObject where T: BaseUiElement
    {
        [SerializeField] private ScriptablePrefabInstance parentInstance;
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
        
        public virtual T Open()
        {
            if (_windowInstance != null)
            {
                _windowInstance.Enable();
                return _windowInstance;
            }
            else
            {
                _windowInstance = Instantiate(windowPrefab, parentInstance.Get().transform);
                _windowInstance.Enable();
                return _windowInstance;
            }
        }

        public virtual void Close()
        {
            if (_windowInstance != null)
            {
                _windowInstance.Disable(() => Destroy(parentInstance.Get()));
            }
        }
    }
}