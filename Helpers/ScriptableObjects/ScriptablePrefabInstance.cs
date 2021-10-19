using System;
using UnityEditor;
using UnityEngine;

namespace Common.Helpers.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewScriptablePrefabInstance", menuName = "ScriptableObjects/ScriptablePrefabInstance")]
    public class ScriptablePrefabInstance<T> : ScriptableObject where T: MonoBehaviour
    {
        [SerializeField] protected T prefab;
        [NonSerialized] protected T PrefabInstance;

        private void Awake()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    PrefabInstance = null;
                }
            };
#endif
        }

        public T Get()
        {
            if (PrefabInstance != null) return PrefabInstance;
            
            PrefabInstance = Instantiate(prefab);
            return PrefabInstance;
        }

        public void Return()
        {
            if (PrefabInstance != null)
            {
                Destroy(PrefabInstance);
                PrefabInstance = null;
            }
        }
    }
}