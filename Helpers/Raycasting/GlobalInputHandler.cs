using System;
using Common.Helpers.MonoBehaviourExtenders;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Common.Helpers.Raycasting
{
    public class GlobalInputHandler : GenericSingleton<GlobalInputHandler>
    {
        public static event Action OnPointerDown;
        public static event Action OnPointerUp;

        //private static HashSet<int> TouchesPointedDown { get; } = new HashSet<int>();

        private void LateUpdate()
        {
#if UNITY_EDITOR
            // Touch started
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                OnPointerDown?.Invoke();
                //LogHelper.LogHelper.Log("OnPointerDown! GLOBAL click from the EDITOR.", MethodBase.GetCurrentMethod());
            }
            // Touch ended
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                OnPointerUp?.Invoke();
                //LogHelper.LogHelper.Log("OnPointerUp! GLOBAL click from the EDITOR.", MethodBase.GetCurrentMethod());
            }
#endif
            foreach (var touch in Touch.activeTouches)
            {
                switch (touch.phase)
                {
                    // Touch started
                    case TouchPhase.Began:
                        OnPointerDown?.Invoke();
                        //LogHelper.LogHelper.Log("OnPointerDown!", MethodBase.GetCurrentMethod());
                        break;
                    // Touch ended
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnPointerUp?.Invoke();
                        //LogHelper.LogHelper.Log("OnPointerUp!", MethodBase.GetCurrentMethod());
                        break;
                }
            }
        }
    }
}