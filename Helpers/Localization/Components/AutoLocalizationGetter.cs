using UnityEngine;
using UnityEngine.UI;

namespace UniversalUnity.Helpers.Localization.Components
{
    [RequireComponent(typeof(Text))]
    public class AutoLocalizationGetter : MonoBehaviour
    {
        [SerializeField] private string textId = null;

        private Text _textComponent;
        private string _text;

        public void Awake()
        {
            _textComponent = GetComponent<Text>();
            if(LocalizationManager.IsParsed) SetLocalizedText();
            LocalizationManager.OnParsed += SetLocalizedText;
        }

        private void OnDestroy()
        {
            LocalizationManager.OnParsed -= SetLocalizedText;
        }

        private void SetLocalizedText()
        {
            _text = LocalizationManager.GetText(textId);
            _textComponent.text = _text;
        }
    }
}
