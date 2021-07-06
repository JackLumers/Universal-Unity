using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.Dialog
{
    public class NewUIDialog : BaseUiElement
    {
        [Header("Dialog Fields")]
        [SerializeField] public BaseUiElement dialog;
        [SerializeField] protected BaseTextUiElement headerTextElement;
        [SerializeField] protected BaseTextUiElement messageTextElement;
        [SerializeField] protected BaseInteractableUiElement acceptButton;
        [SerializeField] protected BaseInteractableUiElement declineButton;
        [SerializeField] protected BaseInteractableUiElement backgroundButton;

        public void SetHeaderText(string text)
        {
            headerTextElement.Text = text;
        }

        public void SetText(string text)
        {
            messageTextElement.Text = text;
        }

        public void AddBackgroundAction(Action action)
        {
            backgroundButton.OnClick += action.Invoke;
        }

        public void RemoveBackgroundAction(Action action)
        {
            backgroundButton.OnClick -= action.Invoke;
        }

        public void EnableBackground(bool enable)
        {
            backgroundButton.Enable(enable);
        }
        
        public void EnableDialog(bool enable)
        {
            dialog.Enable(enable);
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

        public async UniTask EnableAcceptButton(bool enable)
        {
            await acceptButton.Enable(enable);
        }
        
        public async UniTask EnableDeclineButton(bool enable)
        {
            await declineButton.Enable(enable);
        }

        public void ClearActions()
        {
            declineButton.ClearOnClickEvents();
            acceptButton.ClearOnClickEvents();
            backgroundButton.ClearOnClickEvents();
        }
    }
}