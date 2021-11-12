using UnityEngine;
using UnityEngine.UI;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    [RequireComponent(typeof(Image))]
    public class TestInteractableUiElement : BaseInteractableUiElement
    {
        [SerializeField] private Image debugWhiteBlankImage;

        protected override void InheritAwake()
        {
            base.InheritAwake();
            OnPointerDownAction += (pointerEventData) =>
            {
                Debug.Log("OnPointerDownAction!");
                debugWhiteBlankImage.color = Color.black;
            };
            OnPointerEnterAction += (pointerEventData) =>
            {
                Debug.Log("OnPointerEnterAction!");
                debugWhiteBlankImage.color = Color.gray;
            };
            OnPointerExitAction += (pointerEventData) =>
            {
                Debug.Log("OnPointerExitAction!");
                debugWhiteBlankImage.color = Color.white;
            };
            OnPointerUpAction += (pointerEventData) =>
            {
                Debug.Log("OnPointerUpAction!");
                debugWhiteBlankImage.color = Color.magenta;
            };
            OnPointerUpAndPointedAction += (pointerEventData) =>
            {
                Debug.Log("OnPointerUpAndPointedAction!");
                debugWhiteBlankImage.color = Color.red;
            };
            OnHoldAction += (pointerEventData) =>
            {
                Debug.Log("OnHoldAction!");
                debugWhiteBlankImage.color = Color.cyan;
            };
            OnHoldAndPointedAction += (pointerEventData) =>
            {
                Debug.Log("OnHoldAndPointedAction!");
                debugWhiteBlankImage.color = Color.blue;
            };
        }
    }
}