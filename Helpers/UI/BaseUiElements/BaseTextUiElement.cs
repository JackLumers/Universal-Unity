using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UniversalUnity.Helpers.Coroutines;

namespace UniversalUnity.Helpers.UI.BaseUiElements
{
    public class BaseTextUiElement : BaseUiElement
    {
        [Header("= BaseTextUiElement Fields =")]
        [SerializeField] public Text textComponent = null;

        private Coroutine _textChangingCoroutine;

        public string Text
        {
            get
            {
                if(!IsInitialized) InitComponents();
                return textComponent.text;
            }
            set
            {
                if(!IsInitialized) InitComponents();
                textComponent.text = value;
            }
        }

        public Coroutine ShowText(string text, [CanBeNull] Action onShown = null)
        {
            gameObject.SetActive(true);
            Enable();
            return CoroutineHelper.RestartCoroutine(ref _textChangingCoroutine, 
                TextChanging(text, onShown), this);
        }
        
        public void ForceShowText(string text)
        {
            ForceEnable();
            textComponent.text = text;
        }

        private IEnumerator TextChanging(string text, [CanBeNull] Action onChanged)
        {
            yield return ChangeAlpha(0f, enableAnimationTime);
            Text = text;
            yield return Enable(onChanged);
        }
    }
}