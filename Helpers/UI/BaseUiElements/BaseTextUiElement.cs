using System;
using System.Collections;
using Cysharp.Threading.Tasks;
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

        public async UniTask ShowText(string text)
        {
            gameObject.SetActive(true);
            await TextChangingProcess(text);
        }
        
        public void ForceShowText(string text)
        {
            ForceEnable();
            textComponent.text = text;
        }

        private async UniTask TextChangingProcess(string text)
        {
            await Disable();
            Text = text;
            await Enable();
        }
    }
}