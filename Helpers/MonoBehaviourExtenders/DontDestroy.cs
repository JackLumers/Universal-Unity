using UnityEngine;

namespace UniversalUnity.Helpers.MonoBehaviourExtenders
{
    public sealed class DontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}