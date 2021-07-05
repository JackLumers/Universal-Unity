using System;
using Common.Helpers.UI.BaseUiElements;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Helpers.UI.CommonPatterns
{
    /// <summary>
    /// TODO: change all references to a new UiDialog
    /// </summary>
    public class UiDialog : BaseUiElement
    {
        [Header("UiDialog Fields")]
        [SerializeField] public BaseTextUiElement headerTextElement;
        [SerializeField] public BaseInteractableUiElement acceptButton;
        [SerializeField] public BaseInteractableUiElement declineButton;
        [SerializeField] public BaseTextUiElement messageTextElement;

        protected bool CanBeAccepted;
        protected bool CanBeDeclined;
        
        [CanBeNull] protected event Action OnAccept;
        [CanBeNull] protected event Action OnDecline;

        public void Init
        (
            [CanBeNull] string headerText, [CanBeNull] string text, bool canBeAccepted, bool canBeDeclined,
            [CanBeNull] Action onAccept, [CanBeNull] Action onDecline
        )
        {
            CanBeAccepted = canBeAccepted;
            CanBeDeclined = canBeDeclined;
            OnAccept = onAccept;
            OnDecline = onDecline;

            if (CanBeAccepted)
            {
                acceptButton?.ForceEnable();
                acceptButton.ClearOnClickEvents();
                if (acceptButton != null && OnAccept != null)
                {
                    acceptButton.OnClick += OnAccept.Invoke;
                }
            }
            else
            {
                acceptButton.ForceDisable();
            }
            
            if (CanBeDeclined)
            {
                declineButton.ForceEnable();
                declineButton.ClearOnClickEvents();
                if (declineButton != null && OnDecline != null)
                {
                    declineButton.OnClick += OnDecline.Invoke;
                }
            }
            else
            {
                declineButton.ForceDisable();
            }

            if (text != null)
            {
                messageTextElement.Text = text;
            }

            if (headerText != null)
            {
                headerTextElement.Text = headerText;
            }
        }

        public void SetText(string text)
        {
            if (messageTextElement is null) return;
            
            messageTextElement.Text = text;
            messageTextElement.gameObject.SetActive(true);
        }
    }
}