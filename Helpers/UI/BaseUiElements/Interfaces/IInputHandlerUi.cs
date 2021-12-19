using System;
using UnityEngine.EventSystems;

namespace UniversalUnity.Helpers.UI.BaseUiElements.Interfaces
{
    public interface IInputHandlerUi : IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
        bool PointedDown { get; set; }
        float HoldTime { get; set; }
        bool Pointed { get; set; }
        
        /// <summary>
        /// Called when <see cref="IPointerDownHandler.OnPointerDown"/> called and pointer still down for a short time.
        /// <remarks>
        /// When this method called, pointer not necessarily pointing to this object.
        /// </remarks>
        /// </summary>
        Action<PointerEventData> OnHoldAction { get; set; }
        
        /// <summary>
        /// Called when <see cref="IPointerDownHandler.OnPointerDown"/> called and pointer still down for a short time.
        /// <remarks>
        /// Called only when pointer pointing to this object.
        /// </remarks>
        /// </summary>
        Action<PointerEventData> OnHoldAndPointedAction { get; set; }
        
        /// <summary>
        /// Called when pointer set to down, pointing on this object.
        /// </summary>
        Action<PointerEventData> OnPointerDownAction { get; set; }
        
        /// <summary>
        /// Called when pointer set to up, after <see cref="IPointerDownHandler.OnPointerDown"/> called.
        /// <remarks>
        /// When this method called, pointer not necessarily pointing to this object.
        /// </remarks>
        /// </summary>
        Action<PointerEventData> OnPointerUpAction { get; set; }
        
        /// <summary>
        /// Called when pointer set to up, after <see cref="IPointerDownHandler.OnPointerDown"/> called.
        /// <remarks>
        /// Called only when pointer pointing to this object.
        /// </remarks>
        /// </summary>
        Action<PointerEventData> OnPointerUpAndPointedAction { get; set; }

        /// <summary>
        /// Called every time when pointer pointing on object.
        /// </summary>
        Action<PointerEventData> OnPointerEnterAction { get; set; }
        
        /// <summary>
        /// Called every time when pointer pointing on object.
        /// </summary>
        Action<PointerEventData> OnPointerExitAction { get; set; }
        
        //Action OnClickAction { get; set; }
    }
}