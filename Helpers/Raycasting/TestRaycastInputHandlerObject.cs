using UnityEngine;

namespace Common.Helpers.Raycasting
{
    public class TestRaycastInputHandlerObject : MonoBehaviour, IRaycastInputHandler
    {
        public TextMesh textMesh;
        
        public bool PointedDown { get; set; }
        public bool Holden { get; set; }
        public float HoldTime { get; set; }
        
        public void OnPointerDown(RaycastHit raycastHit)
        {
            textMesh.text = "pointer down!";
        }

        public void OnPointerUp(RaycastHit raycastHit)
        {
            textMesh.text = "pointer up!";
        }

        public void OnPointerUpAndPointed(RaycastHit raycastHit)
        {
            textMesh.text = "pointer up here!";
        }

        public void OnHold(RaycastHit raycastHit)
        {
            textMesh.text = "holden!";
        }

        public void OnHoldAndPointed(RaycastHit raycastHit)
        {
            textMesh.text = "holden here!";
        }

        public void OnPointed(RaycastHit raycastHit)
        {
            throw new System.NotImplementedException();
        }
        
        public void OnPointed()
        {
            // --
        }
    }
}