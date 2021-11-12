using TMPro;
using UnityEngine;

namespace UniversalUnity.Helpers.Localization.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TmpAutoLocalizationGetter : MonoBehaviour
    {
        [SerializeField] private string textId = null;

        private TextMeshProUGUI _textComponent;
        private string _text;

        public void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
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