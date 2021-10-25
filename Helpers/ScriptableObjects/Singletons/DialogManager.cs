﻿using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UniversalUnity.Helpers.UI.CommonPatterns.Dialog;

namespace UniversalUnity.Helpers.ScriptableObjects.Singletons
{
    [CreateAssetMenu(fileName = "DialogManager", menuName = "ScriptableObjects/Singletons/DialogManager")]
    public class DialogManager : ScriptableSingleton<DialogManager>
    {
        [SerializeField] private UiDialog dialogPrefab;
        
        public UiDialog CreateDialogInstance(string header, string message, bool enable = true, [CanBeNull] Transform parent = null)
        {
            var dialogInstance = Instantiate(dialogPrefab, parent);

            dialogInstance.AddAcceptAction(
                () =>
                {
                    dialogInstance
                        .Disable()
                        .ContinueWith(() => Destroy(dialogInstance));
                });
            
            dialogInstance.AddDeclineAction(
                () =>
                {
                    dialogInstance
                        .Disable()
                        .ContinueWith(() => Destroy(dialogInstance));
                });
            
            dialogInstance.SetHeaderText(header);
            dialogInstance.SetText(message);
            
            if (enable)
            {
                dialogInstance.Enable().Forget();
            }
            
            return dialogInstance;
        }
    }
}