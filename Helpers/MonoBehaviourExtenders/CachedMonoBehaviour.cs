using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UniversalUnity.Helpers.MonoBehaviourExtenders
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CachedMonoBehaviour : MonoBehaviour
    {
        private readonly List<Component> _cashedComponents = new List<Component>();
        private readonly List<Component> _cashedParentComponents = new List<Component>();
        private Transform _transform = null;
        private GameObject _gameObject = null;

        public new Transform transform
        {
            get
            {
                if (_transform == null) _transform = base.transform;
                return _transform;
            }
        }
        
        public new GameObject gameObject
        {
            get
            {
                if (_gameObject == null) _gameObject = base.gameObject;
                return _gameObject;
            }
        }

        public new T GetComponent<T>() where T : Component
        {
            T temp = _cashedComponents.Find(o => o is T) as T;
            if (temp == null)
            {
                temp = base.GetComponent<T>();
                _cashedComponents.Add(temp);
                return temp;
            }
            return temp;
        }
        
        public new T GetComponentInParent<T>() where T : Component
        {
            T temp = _cashedParentComponents.Find(o => o is T) as T;
            if (temp == null)
            {
                temp = base.GetComponent<T>();
                _cashedParentComponents.Add(temp);
                return temp;
            }
            return temp;
        }
    }
}
