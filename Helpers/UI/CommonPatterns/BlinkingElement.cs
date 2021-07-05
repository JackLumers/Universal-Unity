using System.Collections;
using UnityEngine;
using UniversalUnity.Helpers.Coroutines;
using UniversalUnity.Helpers.Tweeks.CurveAnimationHelper;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns
{
    public class BlinkingElement : BaseUiElement
    {
        [Header("BlinkingElement fields")] [SerializeField]
        private float blinkingAnimationTime = 0.5f;

        private Coroutine _blinkingCoroutine;

        public void Blink()
        {
            Enable();
            CoroutineHelper.RestartCoroutine(ref _blinkingCoroutine, BlinkingProcess(), this);
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
        }

        protected virtual IEnumerator BlinkingProcess()
        {
            Vector3 scale = new Vector3(1.3f, 1.3f, 1f);

            yield return CurveAnimationHelper.Scale((RectTransform) transform, scale,
                speedOrTime: blinkingAnimationTime / 2);
            
            yield return CurveAnimationHelper.Scale((RectTransform) transform, Vector3.one,
                speedOrTime: blinkingAnimationTime / 2);
        }
    }
}