using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.Coroutines;

namespace UniversalUnity.Helpers.Utils
{
    /// <summary>
    /// NOT TESTED
    /// 
    /// Used for handling events (from main or separate thread) in the order.
    /// Order is FIFO (First in first out).
    ///
    /// </summary>
    public class ActionDispatcher : ScriptableObject
    {
        private static readonly Queue<Action> ActionsQueue = new Queue<Action>();
        private static readonly Queue<ConditionAction> ConditionActionsQueue = new Queue<ConditionAction>();

        private readonly struct ConditionAction
        {
            internal readonly Action Action;
            internal readonly Func<bool> Predicate;

            public ConditionAction(Action action, Func<bool> predicate)
            {
                Action = action;
                Predicate = predicate;
            }
        }

        private CancellationTokenSource _conditionEventSchedulerCts;
        private CancellationTokenSource _eventSchedulerCts;
        
        public bool IsRunning { get; private set; }
        
        public static void EnqueueAction(Action action)
        {
            ActionsQueue.Enqueue(action);
        }
        
        public static void EnqueueConditionAction(Action action, Func<bool> predicate)
        {
            ConditionActionsQueue.Enqueue(new ConditionAction(action, predicate));
        }
        
        private void Run()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                
                var actionDispatcherInnerObject = Instantiate(new GameObject());
                actionDispatcherInnerObject.name = $"{nameof(ActionDispatcher)}InnerObject";
                
                _conditionEventSchedulerCts.CancelAndLinkToDestroy(actionDispatcherInnerObject);
                _eventSchedulerCts.CancelAndLinkToDestroy(actionDispatcherInnerObject);
                
                RunScheduler().Forget();
                RunConditionScheduler().Forget();
            }
        }

        private async UniTaskVoid RunScheduler()
        {
            while (!_eventSchedulerCts.IsCancellationRequested)
            {
                if (ActionsQueue.Count > 0)
                {
                    try
                    {
                        ActionsQueue.Dequeue()?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Action is invoked with error. Message: {e.Message}. Stacktrace: {e.StackTrace}");
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        
        private async UniTaskVoid RunConditionScheduler()
        {
            while (!_conditionEventSchedulerCts.IsCancellationRequested)
            {
                if (ConditionActionsQueue.Count > 0)
                {
                    if (ConditionActionsQueue.Peek().Predicate.Invoke())
                    {
                        try
                        {
                            ConditionActionsQueue.Dequeue().Action.Invoke();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Action is invoked with error. Message: {e.Message}. Stacktrace: {e.StackTrace}");
                        }
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
    }
}