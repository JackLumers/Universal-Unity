using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Common.Helpers.UI.CommonPatterns
{
    public class SimpleScrollView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler {
        private Camera _mainCamera;
        private RectTransform _canvasRect;
        public RectTransform viewport;
        public RectTransform content;
        private Rect _viewportOld;
        private Rect _contentOld;

        private readonly List<Vector2> _dragCoordinates = new List<Vector2>();
        private readonly List<float> _offsets = new List<float>();
        private const int OffsetsAveraged = 4;
        private float _offset;
        private float _velocity = 0;
        private bool _changesMade = false;

        public float deceleration = 0.135f;
        public float scrollSensitivity;
        public OnValueChanged onValueChanged;


        [System.Serializable]
        public class OnValueChanged : UnityEvent { }

        [HideInInspector]
        public float VerticalNormalizedPosition
        {
            get
            {
                float sizeDelta = CaculateDeltaSize();
                if (sizeDelta == 0) {
                    return 0;
                } else {
                    return 1 - content.transform.localPosition.y / sizeDelta;
                }
            }
            set
            {
                float oVerticalNormalizedPosition = VerticalNormalizedPosition;
                float mVerticalNormalizedPosition = Mathf.Max(0, Mathf.Min(1, value));
                float maxY = CaculateDeltaSize();
                var localPosition = content.transform.localPosition;
                localPosition = new Vector3(localPosition.x, Mathf.Max(0, (1 - mVerticalNormalizedPosition) * maxY), localPosition.z);
                content.transform.localPosition = localPosition;
                float nVerticalNormalizedPosition = VerticalNormalizedPosition;
                if (oVerticalNormalizedPosition != nVerticalNormalizedPosition) {
                    onValueChanged.Invoke();
                }
            }
        }

        private float CaculateDeltaSize() {
            return Mathf.Max(0, content.rect.height - viewport.rect.height); ;
        }


        private void Awake() {
            _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            _canvasRect = transform.root.GetComponent<RectTransform>();
        }

        private Vector2 ConvertEventDataDrag(PointerEventData eventData) {
            return new Vector2(eventData.position.x / _mainCamera.pixelWidth * _canvasRect.rect.width, eventData.position.y / _mainCamera.pixelHeight * _canvasRect.rect.height);
        }

        private Vector2 ConvertEventDataScroll(PointerEventData eventData) {
            return new Vector2(eventData.scrollDelta.x / _mainCamera.pixelWidth * _canvasRect.rect.width, eventData.scrollDelta.y / _mainCamera.pixelHeight * _canvasRect.rect.height) * scrollSensitivity;
        }

        public void OnPointerDown(PointerEventData eventData) {
            _velocity = 0;
            _dragCoordinates.Clear();
            _offsets.Clear();
            _dragCoordinates.Add(ConvertEventDataDrag(eventData));
        }

        public void OnScroll(PointerEventData eventData) {
            UpdateOffsetsScroll(ConvertEventDataScroll(eventData));
            OffsetContent(_offsets[_offsets.Count - 1]);
        }

        public void OnDrag(PointerEventData eventData) {
            _dragCoordinates.Add(ConvertEventDataDrag(eventData));
            UpdateOffsetsDrag();
            OffsetContent(_offsets[_offsets.Count - 1]);
        }

        public void OnPointerUp(PointerEventData eventData) {
            _dragCoordinates.Add(ConvertEventDataDrag(eventData));
            UpdateOffsetsDrag();
            OffsetContent(_offsets[_offsets.Count - 1]);
            float totalOffsets = 0;
            foreach (float offset in _offsets) {
                totalOffsets += offset;
            }
            _velocity = totalOffsets / OffsetsAveraged;
            _dragCoordinates.Clear();
            _offsets.Clear();
        }

        private void OffsetContent(float givenOffset) {
            float newY = Mathf.Max(0, Mathf.Min(CaculateDeltaSize(), content.transform.localPosition.y + givenOffset));
            if (content.transform.localPosition.y != newY)
            {
                var transform1 = content.transform;
                var localPosition = transform1.localPosition;
                localPosition = new Vector3(localPosition.x, newY, localPosition.z);
                transform1.localPosition = localPosition;
            }
            onValueChanged.Invoke();
        }

        private void UpdateOffsetsDrag() {
            _offsets.Add(_dragCoordinates[_dragCoordinates.Count - 1].y - _dragCoordinates[_dragCoordinates.Count - 2].y);
            if (_offsets.Count > OffsetsAveraged) {
                _offsets.RemoveAt(0);
            }
        }

        private void UpdateOffsetsScroll(Vector2 givenScrollDelta) {
            _offsets.Add(givenScrollDelta.y);
            if (_offsets.Count > OffsetsAveraged) {
                _offsets.RemoveAt(0);
            }
        }

        private void LateUpdate() {
            if (viewport.rect != _viewportOld) {
                _changesMade = true;
                _viewportOld = new Rect(viewport.rect);
            }
            if (content.rect != _contentOld) {
                _changesMade = true;
                _contentOld = new Rect(content.rect);
            }
            if (_velocity != 0) {
                _changesMade = true;
                _velocity = (_velocity / Mathf.Abs(_velocity)) * Mathf.FloorToInt(Mathf.Abs(_velocity) * (1 - deceleration));
                _offset = _velocity;
            }
            if (_changesMade) {
                OffsetContent(_offset);
                _changesMade = false;
                _offset = 0;
            }
        }
    }
}