using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UniversalUnity.Helpers.UI.BaseUiElements.BaseElements
{
    public class BaseTextUiElement : BaseUiElement
    {
        [Header("= BaseTextUiElement Fields =")]
        [SerializeField] public Text textComponent = null;

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
            await Disable();
            Text = text;
            await Enable();
        }
        
        public void ForceShowText(string text)
        {
            ForceEnable();
            textComponent.text = text;
        }
    }
}