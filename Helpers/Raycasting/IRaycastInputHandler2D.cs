using UnityEngine;

namespace Common.Helpers.Raycasting
{
    /// <summary>
    /// Interface that provides input alike Unity UI input events.
    /// <see cref="GameObject"/> must have a collider to handle this events and <see cref="Camera"/>
    /// that renders this <see cref="GameObject"/> must have a <see cref="CameraRaycastInputBehaviour"/>
    /// </summary>
    public interface IRaycastInputHandler2D
    {
        bool PointedDown { get; set; }
        
        bool Holden { get; set; }
        
        float HoldTime { get; set; }

        /// <summary>
        /// Called when pointer set to down, pointing on this object.
        /// </summary>
        /// <param name="raycastHit">Hit from <see cref="CameraRaycastInputBehaviour"/> that pointing
        /// on this <see cref="IRaycastInputHandler"/></param>
        void OnPointerDown(RaycastHit2D raycastHit);
        
        /// <summary>
        /// Called when pointer set to up, after <see cref="OnPointerDown"/> called.
        /// <remarks>
        /// When this method called, pointer not necessarily pointing to this object.
        /// </remarks>
        /// </summary>
        /// <param name="raycastHit">Hit from <see cref="OnPointerDown"/></param>
        void OnPointerUp(RaycastHit2D raycastHit);
        
        /// <summary>
        /// Called when pointer set to up, after <see cref="OnPointerDown"/> called.
        /// <remarks>
        /// Called only when pointer pointing to this object.
        /// </remarks>
        /// </summary>
        /// <param name="raycastHit">Hit from <see cref="CameraRaycastInputBehaviour"/> that pointing
        /// on this <see cref="IRaycastInputHandler"/></param>
        void OnPointerUpAndPointed(RaycastHit2D raycastHit);

        /// <summary>
        /// Called when <see cref="OnPointerDown"/> called and pointer still down for a short time.
        /// <remarks>
        /// When this method called, pointer not necessarily pointing to this object.
        /// </remarks>
        /// </summary>
        /// <param name="raycastHit">Hit from <see cref="OnPointerDown"/></param>
        void OnHold(RaycastHit2D raycastHit);
        
        /// <summary>
        /// Called when <see cref="OnPointerDown"/> called and pointer still down for a short time.
        /// <remarks>
        /// Called only when pointer pointing to this object.
        /// </remarks>
        /// </summary>
        /// <param name="raycastHit">Hit from <see cref="CameraRaycastInputBehaviour"/> that pointing
        /// on this <see cref="IRaycastInputHandler"/></param>
        void OnHoldAndPointed(RaycastHit2D raycastHit);
        
        /// <summary>
        /// Called every time when pointer pointing on object.
        /// </summary>
        /// <param name="raycastHit">Hit from <see cref="CameraRaycastInputBehaviour"/> that pointing
        /// on this <see cref="IRaycastInputHandler"/></param>
        void OnPointed(RaycastHit2D raycastHit);
    }
}