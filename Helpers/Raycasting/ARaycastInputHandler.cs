using System;
using Common.Helpers.MonoBehaviourExtenders;
using UnityEngine;

namespace Common.Helpers.Raycasting
{
    public abstract class ARaycastInputHandler : CachedMonoBehaviour, IRaycastInputHandler
    {
        public bool PointedDown { get; set; }
        public bool Holden { get; set; }
        public float HoldTime { get; set; }

        public Action<RaycastHit> PointerDown;
        public Action<RaycastHit> PointerUp;
        public Action<RaycastHit> PointerUpAndPointed;
        public Action<RaycastHit> Hold;
        public Action<RaycastHit> HoldAndPointed;
        public Action<RaycastHit> Pointed;
        
        public void OnPointerDown(RaycastHit raycastHit)
        {
            PointerDown?.Invoke(raycastHit);
        }

        public void OnPointerUp(RaycastHit raycastHit)
        {
            PointerUp?.Invoke(raycastHit);
        }

        public void OnPointed(RaycastHit raycastHit)
        {
            Pointed?.Invoke(raycastHit);
        }
        
        public void OnPointerUpAndPointed(RaycastHit raycastHit)
        {
            PointerUpAndPointed?.Invoke(raycastHit);
        }

        public void OnHold(RaycastHit raycastHit)
        {
            Hold?.Invoke(raycastHit);
        }

        public void OnHoldAndPointed(RaycastHit raycastHit)
        {
            HoldAndPointed?.Invoke(raycastHit);
        }
    }
}