using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UniversalUnity.Helpers.Tweeks.CurveAnimationHelper
{
    /// <summary>
    /// Class for helping animating using animation curve.
    /// Curve must start with (0, 0) end end with (1, 1) for proper interpolation reason,
    /// but intermediate values can be below 0 and above 1, if you want to do "bounce" effects and e.t.c.
    /// </summary>
    public static class CurveAnimationHelper
    {
        private static readonly int MaterialColor = Shader.PropertyToID("_Color");
        
        #region Public API

        /// <summary>
        /// Moves object to the <paramref name="targetPosition"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// <para></para>
        /// You can change global or local position, defining <paramref name="isLocalPosition"/>
        /// </summary>
        public static async UniTask Move(Transform transform, Vector3 targetPosition,
            [CanBeNull] AnimationCurve curve = null, float speedOrTime = 1f, bool isLocalPosition = true,
            bool fixedTime = true, [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                transform.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    switch (isLocalPosition)
                    {
                        case true:
                            LerpVectorByCurve
                            (
                                result => transform.localPosition = result,
                                curve, transform.localPosition, targetPosition, speedOrTime, fixedTime, onDone
                            );
                            break;
                        default:
                            LerpVectorByCurve
                            (
                                result => transform.position = result,
                                curve, transform.position, targetPosition, speedOrTime, fixedTime, onDone
                            );
                            break;
                    }
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        /// <summary>
        /// Moves object to the <paramref name="targetPosition"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// </summary>
        public static async UniTask MoveAnchored(RectTransform transform, Vector3 targetPosition,
            [CanBeNull] AnimationCurve curve = null, float speedOrTime = 1f,
            bool fixedTime = true, [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                transform.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpVectorByCurve
                    (
                        result => transform.anchoredPosition = result,
                        curve, transform.anchoredPosition, targetPosition, speedOrTime, fixedTime, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        /// <summary>
        /// Rotates object to the <paramref name="targetQuaternion"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// <para></para>
        /// You can change global or local rotation, defining <paramref name="isLocalRotation"/>
        /// </summary>
        public static async UniTask Rotate(Transform transform, Quaternion targetQuaternion,
            [CanBeNull] AnimationCurve curve = null, float speedOrTime = 1f, bool isLocalRotation = true,
            bool fixedTime = true, [CanBeNull] Action onDone = null, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                transform.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    switch (isLocalRotation)
                    {
                        case true:
                            LerpQuarternionByCurve
                            (
                                result => transform.localRotation = result,
                                curve, transform.localRotation, targetQuaternion, speedOrTime, fixedTime, onDone
                            );
                            break;
                        default:
                            LerpQuarternionByCurve
                            (
                                result => transform.rotation = result,
                                curve, transform.rotation, targetQuaternion, speedOrTime, fixedTime, onDone
                            );
                            break;
                    }
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        /// <summary>
        /// Rotates object to the <paramref name="targetRotation"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// <para></para>
        /// You can change global or local rotation, defining <paramref name="isLocalRotation"/>
        /// </summary>
        public static async UniTask Rotate(Transform transform, Vector3 targetRotation,
            [CanBeNull] AnimationCurve curve = null, float speedOrTime = 1f, bool isLocalRotation = true,
            bool fixedTime = true, [CanBeNull] Action onDone = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                transform.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    switch (isLocalRotation)
                    {
                        case true:
                            LerpVectorAsRotationByCurve
                            (
                                result => transform.localRotation = Quaternion.Euler(result.x, result.y, result.z),
                                curve, transform.localRotation.eulerAngles, targetRotation, speedOrTime, fixedTime, onDone
                            );
                            break;
                        default:
                            LerpVectorAsRotationByCurve
                            (
                                result => transform.rotation = Quaternion.Euler(result.x, result.y, result.z),
                                curve, transform.rotation.eulerAngles, targetRotation, speedOrTime, fixedTime, onDone
                            );
                            break;
                    }
                },
                cancellationToken: cancellationTokenSource.Token);
            
        }

        /// <summary>
        /// Changes objects local scale to the <paramref name="targetScale"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// </summary>
        public static async UniTask Scale(Transform transform, Vector3 targetScale,
            [CanBeNull] AnimationCurve animationCurve = null, float speedOrTime = 1f, bool fixedTime = true,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                transform.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpVectorByCurve
                    (
                        result => transform.localScale = result, animationCurve, transform.localScale, targetScale, speedOrTime,
                        fixedTime, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        /// <summary>
        /// Changes graphic's alpha to the <paramref name="targetAlpha"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// </summary>
        public static async UniTask Fade(MaskableGraphic graphic, float targetAlpha,
            [CanBeNull] AnimationCurve animationCurve = null, float speedOrTime = 1f, bool fixedTime = true,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                graphic.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpFloatByCurve
                    (
                        result =>
                        {
                            Color originalColor = graphic.color;
                            graphic.color = new Color(originalColor.r, originalColor.g, originalColor.b, result);
                        },
                        animationCurve, graphic.color.a, targetAlpha, speedOrTime, fixedTime, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        /// <summary>
        /// Changes graphic's color to the <paramref name="targetColor"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// </summary>
        public static async UniTask Recolor(MaskableGraphic graphic, Color targetColor,
            [CanBeNull] AnimationCurve curve = null, float speedOrTime = 1f, bool fixedTime = true,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                graphic.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpColorByCurve
                    (
                        result => graphic.color = result, curve, graphic.color, targetColor, speedOrTime, fixedTime, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        /// <summary>
        /// Changes graphic's color to the <paramref name="targetColor"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// </summary>
        public static async UniTask Recolor(TextMesh textMesh, Color targetColor,
            [CanBeNull] AnimationCurve curve = null, float speedOrTime = 1f, bool fixedTime = true,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                textMesh.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpColorByCurve
                    (
                        result => textMesh.color = result, curve, textMesh.color, targetColor, speedOrTime, fixedTime,
                        onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        /// <summary>
        /// Changes graphic's color to the <paramref name="targetColor"/> with specified <paramref name="speedOrTime"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// </summary>
        public static async UniTask Recolor(Material material, Color targetColor,
            [CanBeNull] AnimationCurve curve = null, float speedOrTime = 1f, bool fixedTime = true,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            await UniTask.Run(() =>
                {
                    LerpColorByCurve
                    (
                        result => material.SetColor(MaterialColor, result), curve, material.color, targetColor,
                        speedOrTime,
                        fixedTime, onDone
                    );
                },
                cancellationToken: cancellationToken);
        }

        public static async UniTask LerpFloatByCurve(Action<float> variable, float startValue, float targetValue,
            [CanBeNull] AnimationCurve curve = null, float timeOrSpeed = 1f, bool fixedTime = true,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            await UniTask.Run(() =>
                {
                    LerpFloatByCurve
                    (
                        variable, curve, startValue, targetValue, timeOrSpeed, fixedTime, onDone
                    );
                },
                cancellationToken: cancellationToken);
        }

        public static async UniTask ChangeTextNumber(Text textObject, float startValue, float targetValue,
            float timeInSeconds = 1.0f, [CanBeNull] AnimationCurve curve = null, [CanBeNull] Action onDone = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                textObject.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpFloatByCurve
                    (
                        result => textObject.text = result.ToString(CultureInfo.InvariantCulture),
                        curve, startValue, targetValue, timeInSeconds, true, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        public static async UniTask AnimateImgFill(Image imageObject, float startValue, float targetValue,
            float timeInSeconds = 1.0f, [CanBeNull] AnimationCurve curve = null, [CanBeNull] Action onDone = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                imageObject.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpFloatByCurve
                    (
                        result => imageObject.fillAmount = result,
                        curve, startValue, targetValue, timeInSeconds, true, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        public static async UniTask AnimateSliderFill(Slider targetObject, float startValue, float targetValue,
            float timeInSeconds = 1.0f, [CanBeNull] AnimationCurve curve = null, [CanBeNull] Action onDone = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                targetObject.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpFloatByCurve
                    (
                        result => targetObject.value = result,
                        curve, startValue, targetValue, timeInSeconds, true, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        public static async UniTask FlipUiElement(RectTransform target, Vector3 rotationAxis,
            float timeInSeconds = 1.0f,
            [CanBeNull] AnimationCurve curve = null, [CanBeNull] Action onDone = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                target.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    Vector3 end1 = rotationAxis * 90f;
                    Vector3 end2 = rotationAxis * 0f;
                    LerpQuarternionByCurve
                    (
                        result => target.localRotation = result,
                        curve, target.localRotation, Quaternion.Euler(end1.x, end1.y, end1.z), timeInSeconds, true,
                        onDone
                    );
                    LerpQuarternionByCurve
                    (
                        result => target.localRotation = result,
                        curve, target.localRotation, Quaternion.Euler(end2.x, end2.y, end2.z), timeInSeconds, true,
                        onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        public static async UniTask FlipObject(Transform target, Vector3 rotationAxis, float timeInSeconds = 1.0f,
            [CanBeNull] AnimationCurve curve = null, [CanBeNull] Action onDone = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                target.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    Vector3 end = rotationAxis * 180f;
                    LerpQuarternionByCurve
                    (
                        result => target.localRotation = result,
                        curve, target.localRotation, Quaternion.Euler(end.x, end.y, end.z), timeInSeconds, true, onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        public static async UniTask Pulse(Transform target, Vector3 startScale, Vector3 maxScale,
            float timeInSeconds = 1.0f, float delayTime = 0.2f, [CanBeNull] AnimationCurve curve = null,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                target.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
            {
                LerpVectorByCurve
                (
                    result => target.localScale = result, curve, target.localScale, maxScale,
                    timeInSeconds,
                    true, onDone
                );

                UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: cancellationToken);

                LerpVectorByCurve
                (
                    result => target.localScale = result, curve, target.localScale, startScale,
                    timeInSeconds,
                    true, onDone
                );
            }, cancellationToken: cancellationTokenSource.Token);
        }

        public static async UniTask ImageBlink(MaskableGraphic targetObject, Color startColor, Color targetColor,
            float timeInSeconds = 1.0f, float delayTime = 0.2f, [CanBeNull] AnimationCurve curve = null,
            [CanBeNull] Action onDone = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                targetObject.GetCancellationTokenOnDestroy());
            await UniTask.Run(() =>
                {
                    LerpColorByCurve
                    (
                        result => targetObject.color = result, curve, startColor, targetColor, timeInSeconds, true,
                        onDone
                    );

                    UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: cancellationToken);

                    LerpColorByCurve
                    (
                        result => targetObject.color = result, curve, targetColor, startColor, timeInSeconds, true,
                        onDone
                    );
                },
                cancellationToken: cancellationTokenSource.Token);
        }

        public static async UniTask MaterialBlink(Material targetObject, Color startColor, Color targetColor,
            float timeInSeconds = 1.0f, float delayTime = 0.2f, string colorPropertyName = "_Color",
            [CanBeNull] AnimationCurve curve = null, [CanBeNull] Action onDone = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await UniTask.Run(() =>
                {
                    LerpColorByCurve(
                        result => targetObject.SetColor(colorPropertyName, result),
                        curve, startColor, targetColor, timeInSeconds, true, onDone);

                    UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: cancellationToken);

                    LerpColorByCurve(
                        result => targetObject.SetColor(colorPropertyName, result),
                        curve, targetColor, startColor, timeInSeconds, true, onDone);
                },
                cancellationToken: cancellationToken);
        }

        #endregion

        #region Private Tweek Methods

        private static async void LerpFloatByCurve(Action<float> variable, [CanBeNull] AnimationCurve curve,
            float startValue,
            float targetValue, float timeOrSpeed, bool fixedTime, [CanBeNull] Action onDone)
        {
            if (ReferenceEquals(curve, null)) curve = DefaultCurve;
            CheckCurve(curve);
            float step = 0;
            switch (fixedTime)
            {
                case true:
                    if (timeOrSpeed.Equals(0))
                    {
                        variable(targetValue); 
                        onDone?.Invoke();
                        break;
                    }
                    if (!startValue.Equals(targetValue))
                    {
                        while (step <= 1)
                        {
                            float interpolatedValue =
                                Mathf.LerpUnclamped(startValue, targetValue, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime / timeOrSpeed;
                            await UniTask.WaitForFixedUpdate();
                        }
                    }
                    variable(targetValue);
                    onDone?.Invoke();
                    break;
                
                case false:
                    CheckSpeed(timeOrSpeed);
                    if (!startValue.Equals(targetValue))
                    {
                        while (step <= 1)
                        {
                            float interpolatedValue =
                                Mathf.LerpUnclamped(startValue, targetValue, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime * timeOrSpeed;
                            
                            await UniTask.WaitForFixedUpdate();
                        }
                    }
                    variable(targetValue);
                    onDone?.Invoke();
                    break;
            }
        }

        private static async void LerpVectorByCurve(Action<Vector3> variable, [CanBeNull] AnimationCurve curve,
            Vector3 startVector, Vector3 targetVector, float timeOrSpeed, bool fixedTime, [CanBeNull] Action onDone)
        {
            if (ReferenceEquals(curve, null)) curve = DefaultCurve;
            CheckCurve(curve);
            float step = 0;
            switch (fixedTime)
            {
                case true:
                    if (timeOrSpeed.Equals(0))
                    {
                        variable(targetVector);
                        onDone?.Invoke();
                        break;
                    }

                    if (!Equals(startVector, targetVector))
                    {
                        while (step <= 1)
                        {
                            Vector3 interpolatedValue =
                                Vector3.LerpUnclamped(startVector, targetVector, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime / timeOrSpeed;

                            await UniTask.WaitForFixedUpdate();
                        }
                    }

                    variable(targetVector);
                    onDone?.Invoke();
                    break;

                case false:
                    CheckSpeed(timeOrSpeed);
                    if (!Equals(startVector, targetVector))
                    {
                        while (step <= 1)
                        {
                            Vector3 interpolatedValue =
                                Vector3.LerpUnclamped(startVector, targetVector, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime * timeOrSpeed;

                            await UniTask.WaitForFixedUpdate();
                        }
                    }

                    variable(targetVector);
                    onDone?.Invoke();
                    break;
            }
        }


        private static async void LerpVectorAsRotationByCurve(Action<Vector3> variable,
            [CanBeNull] AnimationCurve curve,
            Vector3 startVector, Vector3 targetVector, float timeOrSpeed, bool fixedTime, [CanBeNull] Action onDone)
        {
            if (ReferenceEquals(curve, null)) curve = DefaultCurve;
            CheckCurve(curve);
            float step = 0;
            switch (fixedTime)
            {
                case true:
                    if (timeOrSpeed.Equals(0))
                    {
                        variable(targetVector);
                        onDone?.Invoke();
                        break;
                    }

                    if (!Equals(startVector, targetVector))
                    {
                        while (step <= 1)
                        {
                            Vector3 interpolatedValue =
                                Vector3.LerpUnclamped(startVector, targetVector, curve.Evaluate(step));
                            variable(ClampVector(interpolatedValue));
                            step += Time.deltaTime / timeOrSpeed;
                            await UniTask.WaitForFixedUpdate();
                        }
                    }

                    variable(targetVector);
                    onDone?.Invoke();
                    break;

                case false:
                    CheckSpeed(timeOrSpeed);
                    if (!Equals(startVector, targetVector))
                    {
                        while (step <= 1)
                        {
                            Vector3 interpolatedValue =
                                Vector3.LerpUnclamped(startVector, targetVector, curve.Evaluate(step));
                            variable(ClampVector(interpolatedValue));
                            step += Time.deltaTime * timeOrSpeed;
                            await UniTask.WaitForFixedUpdate();
                        }
                    }

                    variable(targetVector);
                    onDone?.Invoke();
                    break;
            }

            Vector3 ClampVector(Vector3 vector)
            {
                return new Vector3(ClampFullRotation(vector.x), ClampFullRotation(vector.y),
                    ClampFullRotation(vector.z));
            }

            float ClampFullRotation(float value)
            {
                if (value > 360)
                {
                    value -= 360;
                    ClampFullRotation(value);
                }
                else if (value < -360)
                {
                    value += 360;
                    ClampFullRotation(value);
                }

                return value;
            }
        }
        
        private static async void LerpQuarternionByCurve(Action<Quaternion> variable, [CanBeNull] AnimationCurve curve,
            Quaternion startQuaternion, Quaternion targetQuaternion, float timeOrSpeed, bool fixedTime, [CanBeNull] Action onDone)
        {
            if (ReferenceEquals(curve, null)) curve = DefaultCurve;
            CheckCurve(curve);
            float step = 0;
            switch (fixedTime)
            {
                case true:
                    if (timeOrSpeed.Equals(0))
                    {
                        variable(targetQuaternion);
                        onDone?.Invoke();
                        break;
                    }
                    if (!Equals(startQuaternion, targetQuaternion))
                    {
                        while (step <= 1)
                        {
                            Quaternion interpolatedValue =
                                Quaternion.LerpUnclamped(startQuaternion, targetQuaternion, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime / timeOrSpeed;
                            await UniTask.WaitForFixedUpdate();
                        }
                    }
                    variable(targetQuaternion);
                    onDone?.Invoke();
                    break;
                
                case false:
                    CheckSpeed(timeOrSpeed);
                    if (!Equals(startQuaternion, targetQuaternion))
                    {
                        while (step <= 1)
                        {
                            Quaternion interpolatedValue =
                                Quaternion.LerpUnclamped(startQuaternion, targetQuaternion, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime * timeOrSpeed;
                            await UniTask.WaitForFixedUpdate();
                        }
                    }
                    variable(targetQuaternion);
                    onDone?.Invoke();
                    break;
            }
        }

        private static async void LerpColorByCurve(Action<Color> variable, [CanBeNull] AnimationCurve curve,
            Color startColor,
            Color targetColor, float timeOrSpeed, bool fixedTime, [CanBeNull] Action onDone)
        {
            if (ReferenceEquals(curve, null)) curve = DefaultCurve;
            CheckCurve(curve);
            float step = 0;
            switch (fixedTime)
            {
                case true:
                    if (timeOrSpeed.Equals(0))
                    {
                        variable(targetColor);
                        onDone?.Invoke();  
                        break;
                    }
                    if (!Equals(startColor, targetColor))
                    {
                        while (step <= 1)
                        {
                            Color interpolatedValue =
                                Color.LerpUnclamped(startColor, targetColor, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime / timeOrSpeed;
                            await UniTask.WaitForFixedUpdate();
                        }
                    }
                    variable(targetColor);
                    onDone?.Invoke();
                    break;
                case false:
                    CheckSpeed(timeOrSpeed);
                    if (!Equals(startColor, targetColor))
                    {
                        while (step <= 1)
                        {
                            Color interpolatedValue =
                                Color.LerpUnclamped(startColor, targetColor, curve.Evaluate(step));
                            variable(interpolatedValue);
                            step += Time.deltaTime * timeOrSpeed;
                            await UniTask.WaitForFixedUpdate();
                        }
                    }
                    variable(targetColor);
                    onDone?.Invoke();
                   break;
            }
        }

        #endregion

        #region Private Inner Methods And Fields

        private static readonly AnimationCurve DefaultCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        /// <summary>
        /// Checks whether the curve fits or not.
        /// Fitting curve must start with (0, 0) end end with (1, 1).
        /// </summary>
        private static void CheckCurve([NotNull] AnimationCurve curve)
        {
            if (curve == null) throw new ArgumentNullException(nameof(curve));
            if (curve[0].time > 0 || curve[0].value > 0)
            {
                throw new ArgumentException("[CurveAnimatorHelper] Curve must start with (0, 0) end end with (1, 1)!");
            }

            if (curve[curve.length - 1].time > 1 || curve[curve.length - 1].time < 1 ||
                curve[curve.length - 1].value > 1 ||
                curve[curve.length - 1].time < 1)
            {
                throw new ArgumentException("[CurveAnimatorHelper] Curve must start with (0, 0) end end with (1, 1)!");
            }
        }

        private static void CheckSpeed(float speed)
        {
            if (speed <= 0)
                throw new ArgumentOutOfRangeException(nameof(speed),
                    new ArgumentException(
                        "[CurveAnimatorHelper] Process will be endless because speed is equal to zero."));
        }

        #endregion
    }
}