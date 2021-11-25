using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.Dialog
{
    public class UiDialog : BaseUiElement
    {
        [Header("Dialog Fields")]
        [SerializeField] protected BaseTextUiElement headerTextElement;
        [SerializeField] protected BaseTextUiElement messageTextElement;
        [SerializeField] protected BaseInteractableUiElement acceptButton;
        [SerializeField] protected BaseInteractableUiElement declineButton;
        
        public async UniTask ChangeText(string text)
        {
            Enable().Forget();
            await messageTextElement.ShowText(text);
        }
        
        public async UniTask ChangeHeaderText(string text)
        {
            Enable().Forget();
            await headerTextElement.ShowText(text);
        }

        public void SetAcceptButton(bool enable)
        {
            acceptButton.ForceEnable(enable);
        }
        
        public void SetDeclineButton(bool enable)
        {
            declineButton.ForceEnable(enable);
        }
        
        public void SetHeaderText(string text)
        {
            headerTextElement.Text = text;
        }

        public void SetText(string text)
        {
            messageTextElement.Text = text;
        }

        public void SetAcceptButtonText(string text)
        {
            acceptButton.GetComponentInChildren<Text>().text = text;
        }
        
        public void SetDeclineButtonText(string text)
        {
            declineButton.GetComponentInChildren<Text>().text = text;
        }

        public void AddAcceptAction(Action action)
        {
            acceptButton.OnClick += action.Invoke;
        }

        public void RemoveAcceptAction(Action action)
        {
            acceptButton.OnClick -= action.Invoke;
        }
        
        public void AddDeclineAction(Action action)
        {
            declineButton.OnClick += action.Invoke;
        }
        
        public void RemoveDeclineAction(Action action)
        {
            declineButton.OnClick += action.Invoke;
        }
        
        public void ClearActions()
        {
            declineButton?.ClearOnClickEvents();
            acceptButton?.ClearOnClickEvents();
        }
        
        public UiDialog Instantiate(Transform parent = null)
        {
            var dialogInstance = Instantiate(this, parent);

            dialogInstance.AddAcceptAction(
                () =>
                {
                    dialogInstance
                        .Disable()
                        .ContinueWith(() => Destroy(dialogInstance));
                });
            
            dialogInstance.Enable().Forget();
            return dialogInstance;
        }
    }
}