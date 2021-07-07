﻿using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.UI.CommonPatterns.FillableElement;

namespace UniversalUnity.Helpers.UI.CommonPatterns.Timers
{
    public class UiFillableTimer : BaseUiTimer
    {
        [SerializeField] protected UiFillableLine uiFillableLine;
        
        protected override async UniTask UiHandleTimer(float durationInMillis)
        {
            uiFillableLine.ForceFill(0);
            UniTask.Run(() => Enable());
            await uiFillableLine.Fill(100,uiFillableLine.maxAmount / (durationInMillis / 1000));
        }
    }
}