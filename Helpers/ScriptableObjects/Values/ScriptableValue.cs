using System;
using UnityEngine;

namespace Common.Helpers.ScriptableObjects.Values
{
    public class ScriptableValue<T> : ScriptableObject
    {
        [SerializeField] private T value;
        
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                ValueChanged?.Invoke(value);
            }
        }

        public void ChangeWithoutNotify(T v)
        {
            value = v;
        }

        public event Action<T> ValueChanged;
    }
}