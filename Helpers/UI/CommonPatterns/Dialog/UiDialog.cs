using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.Dialog
{
    public class UiDialog : BaseUiElement
    {
        [Header("Dialog Fields")]
        [SerializeField] public BaseUiElement dialog;
        [SerializeField] protected BaseTextUiElement headerTextElement;
        [SerializeField] protected BaseTextUiElement messageTextElement;
        [SerializeField] protected BaseInteractableUiElement acceptButton;
        [SerializeField] protected BaseInteractableUiElement declineButton;
        [SerializeField] protected BaseInteractableUiElement backgroundButton;

        public async UniTask ChangeText(string text)
        {
            dialog.Enable().Forget();
            await messageTextElement.ShowText(text);
        }
        
        public async UniTask ChangeHeaderText(string text)
        {
            dialog.Enable().Forget();
            await headerTextElement.ShowText(text);
        }

        public async UniTask EnableBackground(bool enable)
        {
            await backgroundButton.Enable(enable);
        }
        
        public async UniTask EnableDialog(bool enable)
        {
            await dialog.Enable(enable);
        }

        public async UniTask EnableAcceptButton(bool enable)
        {
            await acceptButton.Enable(enable);
        }
        
        public async UniTask EnableDeclineButton(bool enable)
        {
            await declineButton.Enable(enable);
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
            acceptButton.GetComponent<Text>().text = text;
        }
        
        public void AddBackgroundAction(Action action)
        {
            backgroundButton.OnClick += action.Invoke;
        }

        public void RemoveBackgroundAction(Action action)
        {
            backgroundButton.OnClick -= action.Invoke;
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
            declineButton.ClearOnClickEvents();
            acceptButton.ClearOnClickEvents();
            backgroundButton.ClearOnClickEvents();
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