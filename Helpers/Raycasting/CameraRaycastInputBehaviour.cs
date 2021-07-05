using System.Collections.Generic;
using Common.Helpers._ProjectDependent;
using UnityEngine;
using UnityEngine.InputSystem;

#if !UNITY_EDITOR
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif

namespace Common.Helpers.Raycasting
{
    [RequireComponent(typeof(Camera))]
    public class CameraRaycastInputBehaviour : MonoBehaviour
    {
        private Camera _camera;

        private static HashSet<int> TouchesPointedDown { get; } = new HashSet<int>();
        private static Dictionary<int, TouchedObject> TouchedRaycastables { get; } = new Dictionary<int, TouchedObject>();
        private static Dictionary<int, TouchedObject2D> TouchedRaycastables2D { get; } = new Dictionary<int, TouchedObject2D>();
        
        private class TouchedObject
        {
            public IRaycastInputHandler Handler;
            public RaycastHit Pointer;

            public TouchedObject(IRaycastInputHandler handler, RaycastHit pointer)
            {
                Handler = handler;
                Pointer = pointer;
            }
        }
        
        private class TouchedObject2D
        {
            public IRaycastInputHandler2D Handler;
            public RaycastHit2D Pointer;

            public TouchedObject2D(IRaycastInputHandler2D handler, RaycastHit2D pointer)
            {
                Handler = handler;
                Pointer = pointer;
            }
        }
        
        protected void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            // Touch started
            if (Mouse.current.leftButton.wasPressedThisFrame && !TouchesPointedDown.Contains(Constants.MouseTouchId))
            {
                //LogHelper.LogHelper.Log($"Click start: {Mouse.current.position.ReadValue()}", MethodBase.GetCurrentMethod());
                HandleTouchStart(Constants.MouseTouchId, Mouse.current.position.ReadValue());
            }
            // Touch ended
            else if (Mouse.current.leftButton.wasReleasedThisFrame && TouchesPointedDown.Contains(Constants.MouseTouchId))
            {
                //LogHelper.LogHelper.Log($"Click end: {Mouse.current.position.ReadValue()}", MethodBase.GetCurrentMethod());
                HandleTouchEnd(Constants.MouseTouchId, Mouse.current.position.ReadValue());
            }
            // Touch continue
            else if (Mouse.current.leftButton.isPressed)
            {
                //LogHelper.LogHelper.Log($"Click continue: {Mouse.current.position.ReadValue()}", MethodBase.GetCurrentMethod());
                HandleTouchContinue(Constants.MouseTouchId, Mouse.current.position.ReadValue());
            }
            else if (TouchesPointedDown.Contains(Constants.MouseTouchId))
            {
                TouchesPointedDown.Remove(Constants.MouseTouchId);
                if (TouchedRaycastables.TryGetValue(Constants.MouseTouchId, out var touchedObject))
                {
                    touchedObject.Handler.OnPointerUp(touchedObject.Pointer);
                    touchedObject.Handler.PointedDown = true;
                    touchedObject.Handler.HoldTime = 0;
                    TouchedRaycastables.Remove(Constants.MouseTouchId);
                }
                
                if (TouchedRaycastables2D.TryGetValue(Constants.MouseTouchId, out var touchedObject2d))
                {
                    touchedObject2d.Handler.OnPointerUp(touchedObject2d.Pointer);
                    touchedObject2d.Handler.PointedDown = true;
                    touchedObject2d.Handler.HoldTime = 0;
                    TouchedRaycastables2D.Remove(Constants.MouseTouchId);
                }
            }
#else
            foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                // Touch started
                if (touch.phase == TouchPhase.Began && !TouchesPointedDown.Contains(touch.touchId))
                {
                    //Log($"Touch start: {Input.mousePosition}", MethodBase.GetCurrentMethod());
                    HandleTouchStart(touch.touchId, touch.screenPosition);
                }
                // Touch ended
                else if (touch.phase == TouchPhase.Ended && TouchesPointedDown.Contains(touch.touchId))
                {
                    //Log($"Touch end: {Input.mousePosition}", MethodBase.GetCurrentMethod());
                    HandleTouchEnd(touch.touchId, touch.screenPosition);
                }
                // Touch continue
                else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    //Log($"Touch continue: {Input.mousePosition}", MethodBase.GetCurrentMethod());
                    HandleTouchContinue(touch.touchId, touch.screenPosition);
                }
                else if(TouchesPointedDown.Contains(touch.touchId))
                {
                    TouchesPointedDown.Remove(touch.touchId);
                    if (TouchedRaycastables.TryGetValue(touch.touchId, out var touchedObject))
                    {
                        touchedObject.Handler.OnPointerUp(touchedObject.Pointer);
                        touchedObject.Handler.PointedDown = true;
                        touchedObject.Handler.HoldTime = 0;
                        TouchedRaycastables.Remove(touch.touchId);
                    }
                
                    if (TouchedRaycastables2D.TryGetValue(touch.touchId, out var touchedObject2d))
                    {
                        touchedObject2d.Handler.OnPointerUp(touchedObject2d.Pointer);
                        touchedObject2d.Handler.PointedDown = true;
                        touchedObject2d.Handler.HoldTime = 0;
                        TouchedRaycastables2D.Remove(touch.touchId);
                    }
                }
            }
#endif
        }

        private void HandleTouchStart(int touchId, Vector3 inputOriginPosition)
        {
            Ray ray = _camera.ScreenPointToRay(inputOriginPosition);

            var hit2D = Physics2D.Raycast(inputOriginPosition, ray.direction, _camera.farClipPlane,
                _camera.cullingMask);

            if (hit2D.transform != null)
            {
                // If hit IRaycastable object
                if (hit2D.transform.gameObject.TryGetComponent<IRaycastInputHandler2D>(out var pointedObjectHandler))
                {
                    pointedObjectHandler.OnPointed(hit2D);
                
                    pointedObjectHandler.OnPointerDown(hit2D);
                    pointedObjectHandler.PointedDown = true;
                    pointedObjectHandler.HoldTime = 0;
                
                    TouchedRaycastables2D.Add(touchId, new TouchedObject2D(pointedObjectHandler, hit2D));
                }
            }

            if (Physics.Raycast(ray, out var hit, _camera.farClipPlane, _camera.cullingMask))
            {
                //Log($"Raycast hit object with name: {hit.collider.gameObject.name}", MethodBase.GetCurrentMethod());
                
                // If hit IRaycastable object
                if (hit.transform.gameObject.TryGetComponent<IRaycastInputHandler>(out var pointedObjectHandler))
                {
                    pointedObjectHandler.OnPointed(hit);
                
                    pointedObjectHandler.OnPointerDown(hit);
                    pointedObjectHandler.PointedDown = true;
                    pointedObjectHandler.HoldTime = 0;
                
                    TouchedRaycastables.Add(touchId, new TouchedObject(pointedObjectHandler, hit));
                }
            }
            
            TouchesPointedDown.Add(touchId);
        }
        
        private void HandleTouchContinue(int touchId, Vector3 inputOriginPosition)
        {
            Ray ray = _camera.ScreenPointToRay(inputOriginPosition);
            
            var hit2D = Physics2D.Raycast(inputOriginPosition, ray.direction, _camera.farClipPlane,
                _camera.cullingMask);

            if (hit2D.transform != null)
            {
                // If hit IRaycastable object
                if (hit2D.transform.gameObject.TryGetComponent<IRaycastInputHandler2D>(out var pointedObjectHandler))
                {
                    pointedObjectHandler.OnPointed(hit2D);

                    // Trying to get old object that was affected by this ray during last raycast
                    if (TouchedRaycastables2D.TryGetValue(touchId, out var oldTouchedObject))
                    {
                        // If old object is not affected by this ray anymore
                        if (!oldTouchedObject.Handler.Equals(pointedObjectHandler))
                        {
                            oldTouchedObject.Handler.HoldTime += Time.deltaTime;
                            // If hold time exceeded and old touched object was not holden before
                            if (oldTouchedObject.Handler.HoldTime >= Constants.HoldTimeBorder && !oldTouchedObject.Handler.Holden)
                            {
                                oldTouchedObject.Handler.Holden = true;
                                oldTouchedObject.Handler.OnHold(oldTouchedObject.Pointer);
                            }
                        }
                        // Ray affects object, that was affected during last raycast
                        else
                        {
                            pointedObjectHandler.HoldTime += Time.deltaTime;
                            // If hold time exceeded and touched object was not holden before
                            if (pointedObjectHandler.HoldTime >= Constants.HoldTimeBorder && !pointedObjectHandler.Holden)
                            {
                                pointedObjectHandler.Holden = true;
                                pointedObjectHandler.OnHold(hit2D);
                                pointedObjectHandler.OnHoldAndPointed(hit2D);
                            }
                        }
                    }
                }
            }
            else
            {
                // If there was an object that was affected by this raycast before, but now not
                if (TouchedRaycastables2D.TryGetValue(touchId, out var notTouchedAnymore2D))
                {
                    notTouchedAnymore2D.Handler.HoldTime += Time.deltaTime;
                    // If hold time exceeded and touched object was not holden before
                    if (notTouchedAnymore2D.Handler.HoldTime >= Constants.HoldTimeBorder && !notTouchedAnymore2D.Handler.Holden)
                    {
                        notTouchedAnymore2D.Handler.Holden = true;
                        notTouchedAnymore2D.Handler.OnHold(notTouchedAnymore2D.Pointer);
                    }
                }
            }
            
            if (Physics.Raycast(ray, out var hit, _camera.farClipPlane, _camera.cullingMask))
            {
                //Log($"Raycast hit object with name: {hit.collider.gameObject.name}", MethodBase.GetCurrentMethod());

                // If hit IRaycastable object
                if (hit.transform.gameObject.TryGetComponent<IRaycastInputHandler>(out var pointedObjectHandler))
                {
                    pointedObjectHandler.OnPointed(hit);

                    // Trying to get old object that was affected by this ray during last raycast
                    if (TouchedRaycastables.TryGetValue(touchId, out var oldTouchedObject))
                    {
                        // If old object is not affected by this ray anymore
                        if (!oldTouchedObject.Handler.Equals(pointedObjectHandler))
                        {
                            oldTouchedObject.Handler.HoldTime += Time.deltaTime;
                            // If hold time exceeded and old touched object was not holden before
                            if (oldTouchedObject.Handler.HoldTime >= Constants.HoldTimeBorder && !oldTouchedObject.Handler.Holden)
                            {
                                oldTouchedObject.Handler.Holden = true;
                                oldTouchedObject.Handler.OnHold(oldTouchedObject.Pointer);
                            }
                        }
                        // Ray affects object, that was affected during last raycast
                        else
                        {
                            pointedObjectHandler.HoldTime += Time.deltaTime;
                            // If hold time exceeded and touched object was not holden before
                            if (pointedObjectHandler.HoldTime >= Constants.HoldTimeBorder && !pointedObjectHandler.Holden)
                            {
                                pointedObjectHandler.Holden = true;
                                pointedObjectHandler.OnHold(hit);
                                pointedObjectHandler.OnHoldAndPointed(hit);
                            }
                        }
                    }
                }
            }
            else
            {
                // If there was an object that was affected by this raycast before, but now not
                if (TouchedRaycastables.TryGetValue(touchId, out var notTouchedAnymore))
                {
                    notTouchedAnymore.Handler.HoldTime += Time.deltaTime;
                    // If hold time exceeded and touched object was not holden before
                    if (notTouchedAnymore.Handler.HoldTime >= Constants.HoldTimeBorder && !notTouchedAnymore.Handler.Holden)
                    {
                        notTouchedAnymore.Handler.Holden = true;
                        notTouchedAnymore.Handler.OnHold(notTouchedAnymore.Pointer);
                    }
                }
            }
        }

        private void HandleTouchEnd(int touchId, Vector3 inputOriginPosition)
        {
            Ray ray = _camera.ScreenPointToRay(inputOriginPosition);
            
            var hit2D = Physics2D.Raycast(inputOriginPosition, ray.direction, _camera.farClipPlane,
                _camera.cullingMask);
            
            if (hit2D.transform != null)
            {
                // If hit IRaycastable object
                if (hit2D.transform.gameObject.TryGetComponent<IRaycastInputHandler2D>(out var pointedObject))
                {
                    pointedObject.OnPointed(hit2D);

                    // Trying to get old object that was affected by this ray during last raycast
                    if (TouchedRaycastables2D.TryGetValue(touchId, out var oldTouchedObject))
                    {
                        // If old object is not affected anymore
                        if (!oldTouchedObject.Handler.Equals(pointedObject))
                        {
                            // If object was pointed down
                            if (oldTouchedObject.Handler.PointedDown)
                            {
                                oldTouchedObject.Handler.HoldTime = 0;
                                oldTouchedObject.Handler.Holden = false;
                                oldTouchedObject.Handler.PointedDown = false;
                                oldTouchedObject.Handler.OnPointerUp(hit2D);
                            }
                        }
                        // Ray affects this object, that was affected during last raycast
                        else
                        {
                            // If object was pointed down
                            if (pointedObject.PointedDown)
                            {
                                pointedObject.HoldTime = 0;
                                pointedObject.Holden = false;
                                pointedObject.PointedDown = false;
                                pointedObject.OnPointerUp(hit2D);
                                pointedObject.OnPointerUpAndPointed(hit2D);
                            }
                        }
                    }
                }
                else
                {
                    // If there was an object that was affected by this raycast before, but now not
                    if (TouchedRaycastables2D.TryGetValue(touchId, out var notTouchedAnymore))
                    {
                        notTouchedAnymore.Handler.HoldTime = 0;
                        notTouchedAnymore.Handler.Holden = false;
                        notTouchedAnymore.Handler.PointedDown = false;
                        notTouchedAnymore.Handler.OnPointerUp(notTouchedAnymore.Pointer);
                    }
                }
            }

            if (Physics.Raycast(ray, out var hit, _camera.farClipPlane, _camera.cullingMask))
            {
                //Log($"Raycast hit object with name: {hit.collider.gameObject.name}", MethodBase.GetCurrentMethod());

                // If hit IRaycastable object
                if (hit.transform.gameObject.TryGetComponent<IRaycastInputHandler>(out var pointedObject))
                {
                    pointedObject.OnPointed(hit);

                    // Trying to get old object that was affected by this ray during last raycast
                    if (TouchedRaycastables.TryGetValue(touchId, out var oldTouchedObject))
                    {
                        // If old object is not affected anymore
                        if (!oldTouchedObject.Handler.Equals(pointedObject))
                        {
                            // If object was pointed down
                            if (oldTouchedObject.Handler.PointedDown)
                            {
                                oldTouchedObject.Handler.HoldTime = 0;
                                oldTouchedObject.Handler.Holden = false;
                                oldTouchedObject.Handler.PointedDown = false;
                                oldTouchedObject.Handler.OnPointerUp(hit);
                            }
                        }
                        // Ray affects this object, that was affected during last raycast
                        else
                        {
                            // If object was pointed down
                            if (pointedObject.PointedDown)
                            {
                                pointedObject.HoldTime = 0;
                                pointedObject.Holden = false;
                                pointedObject.PointedDown = false;
                                pointedObject.OnPointerUp(hit);
                                pointedObject.OnPointerUpAndPointed(hit);
                            }
                        }
                    }
                }
                else
                {
                    // If there was an object that was affected by this raycast before, but now not
                    if (TouchedRaycastables.TryGetValue(touchId, out var notTouchedAnymore))
                    {
                        notTouchedAnymore.Handler.HoldTime = 0;
                        notTouchedAnymore.Handler.Holden = false;
                        notTouchedAnymore.Handler.PointedDown = false;
                        notTouchedAnymore.Handler.OnPointerUp(notTouchedAnymore.Pointer);
                    }
                }
            }
            
            TouchedRaycastables2D.Remove(touchId);
            TouchedRaycastables.Remove(touchId);
            TouchesPointedDown.Remove(touchId);
        }
    }
}