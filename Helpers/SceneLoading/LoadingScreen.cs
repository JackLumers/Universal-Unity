using System;
using System.Collections;
using Common.Helpers.Coroutines;
using Common.Helpers.MonoBehaviourExtenders;
using Common.Helpers.UI.BaseUiElements;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Helpers.SceneLoading
{
    // Must be in not destroyable parent
    public class LoadingScreen : GenericSingleton<LoadingScreen>
    {
        [SerializeField] private BaseUiElement ui;
        [SerializeField] private BaseTextUiElement textElement;

        private Coroutine _textChangingCoroutine;

        protected override void InheritAwake()
        {
            //ui.gameObject.SetActive(false);
        }
        
        public Coroutine Enable([CanBeNull] Action onEnabled = null)
        {
            return ui.Enable(onEnabled);
        }

        public void ForceEnable()
        {
            ui.ForceEnable();
        }

        public Coroutine Disable([CanBeNull] Action onDisabled = null)
        {
            return ui.Disable(onDisabled);
        }
        
        public void ForceDisable()
        {
            ui.ForceDisable();
        }

        public Coroutine ShowText(string text, [CanBeNull] Action onSet = null)
        {
            ui.Enable();
            return CoroutineHelper.RestartCoroutine(ref _textChangingCoroutine, TextChanging(text, onSet), this);
        }

        public void SetText(string text)
        {
            textElement.gameObject.SetActive(true);
            textElement.Text = text;
        }

        private IEnumerator TextChanging(string text, [CanBeNull] Action onChanged)
        {
            if(!ui.IsEnabled) yield return ui.Enable();
            if(textElement.IsEnabled) yield return textElement.Disable();
            textElement.Text = text;
            yield return textElement.Enable();
            onChanged?.Invoke();
        }
    }
}