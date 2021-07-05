using UnityEngine;

namespace Common.Helpers.MonoBehaviourExtenders
{
    public class NotDestroyable : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
            InheritAwake();
        }

        protected virtual void InheritAwake()
        {
            
        }
    }
}