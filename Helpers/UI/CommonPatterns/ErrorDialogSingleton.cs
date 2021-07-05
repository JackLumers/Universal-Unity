using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.Pooling.SimplePool;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns
{
    public class ErrorDialogSingleton : GenericSingleton<ErrorDialogSingleton>
    {
        [SerializeField] private UiDialog uiDialogPrefab;
        [SerializeField] private BaseUiElement raycastBlockElement;

        private const string PoolName = "ErrorDialogsPool";

        private static Dictionary<string, UiDialog> _openedDialogs = new Dictionary<string, UiDialog>();
        
        protected override void InheritAwake()
        {
            base.InheritAwake();
            SimplePool.CreatePool(PoolName);
        }
        
        public static void OpenNewDialog(string dialogId, [CanBeNull] string text, 
            bool canBeAccepted, bool canBeDeclined, 
            [CanBeNull] Action onAccept, [CanBeNull] Action onDecline, [CanBeNull] string headerText = "Ошибка.")
        {
            if (!_openedDialogs.ContainsKey(dialogId))
            {
                // Todo: localize
                var dialog = SimplePool.Get(Instance.uiDialogPrefab, Instance.transform, PoolName);
                _openedDialogs.Add(dialogId, dialog);
                dialog.Init(headerText, text, canBeAccepted, canBeDeclined, onAccept, onDecline);
                dialog.Enable();
            }
            else
            {
                LogHelper.LogWarning($"Dialog with id '{dialogId}' already opened.", 
                    nameof(OpenNewDialog));
            }
            
            Instance.raycastBlockElement.Enable();
        }

        public static void OpenNewClosableDialog([CanBeNull] string text, [CanBeNull] string headerText)
        {
            string dialogId = Guid.NewGuid().ToString();
            if (!_openedDialogs.ContainsKey(dialogId))
            {
                // Todo: localize
                var dialog = SimplePool.Get(Instance.uiDialogPrefab, Instance.transform, PoolName);
                _openedDialogs.Add(dialogId, dialog);
                dialog.Init(headerText, text, true, false, 
                    () => CloseDialog(dialogId), 
                    null);
                dialog.Enable();
            }
            else
            {
                LogHelper.LogError($"Dialog with id '{dialogId}' already opened.", 
                    nameof(OpenNewClosableDialog));
            }
            
            Instance.raycastBlockElement.Enable();
        }
        
        public static void CloseDialog(string dialogId)
        {
            if (_openedDialogs.ContainsKey(dialogId))
            {
                _openedDialogs[dialogId].Disable();
                _openedDialogs.Remove(dialogId);
            }

            if (_openedDialogs.Count == 0)
            {
                Instance.raycastBlockElement.Disable();
            }
        }
        
        public static void CloseAllDialogs()
        {
            foreach (var uiDialog in _openedDialogs)
            {
                uiDialog.Value.Disable(() => SimplePool.Return(uiDialog.Value, PoolName));
            }
            _openedDialogs.Clear();
            Instance.raycastBlockElement.Disable();
        }
    }
}