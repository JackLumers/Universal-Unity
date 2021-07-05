using System;
using UnityEditor;
using UnityEngine;

namespace UniversalUnity.Helpers.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewScriptablePrefabInstance", menuName = "ScriptableObjects/ScriptablePrefabInstance")]
    public class ScriptablePrefabInstance : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [NonSerialized] private GameObject _prefabInstance;

        private void Awake()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    _prefabInstance = null;
                }
            };
#endif
        }

        public GameObject Get()
        {
            if (_prefabInstance != null) return _prefabInstance;
            
            _prefabInstance = Instantiate(prefab);
            return _prefabInstance;
        }
    }
}