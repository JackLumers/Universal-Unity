using System.Collections.Generic;
using UnityEngine;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    public static class AnimationConstants
    {
        private static Dictionary<string, int> AnimatorHashes = new Dictionary<string, int>();
        
        public static int GetAnimatorHash(AnimationParamNames name)
        {
            var nameAsString = name.ToString();
            if (AnimatorHashes.ContainsKey(nameAsString))
            {
                return AnimatorHashes[nameAsString];
            }
            else
            {
                AnimatorHashes[nameAsString] = Animator.StringToHash(nameAsString);
                return AnimatorHashes[nameAsString];
            }
        }
    }
    
    public enum AnimationParamNames
    {
        Enabling,
        Disabling,
        EnableTrigger,
        DisableTrigger,
    }
}