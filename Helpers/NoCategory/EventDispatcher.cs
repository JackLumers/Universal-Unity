using System;
using System.Collections;
using System.Collections.Generic;
using Common.Helpers.Coroutines;
using Common.Helpers.MonoBehaviourExtenders;
using UnityEngine;

namespace Common.Helpers
{
    public class EventDispatcher : GenericSingleton<EventDispatcher>
    {
        private static readonly Queue<Action> ActionsQueue = new Queue<Action>();
        private static readonly Queue<ConditionAction> ConditionActionsQueue = new Queue<ConditionAction>();

        private struct ConditionAction
        {
            internal readonly Action Action;
            internal readonly Func<bool> Predicate;

            public ConditionAction(Action action, Func<bool> predicate)
            {
                Action = action;
                Predicate = predicate;
            }
        }

        private Coroutine _conditionEventSchedulerCoroutine;
        private Coroutine _eventSchedulerCoroutine;
            
        protected override void InheritAwake()
        {
            CoroutineHelper.RestartCoroutine(ref _eventSchedulerCoroutine, EventScheduler(), this);
            CoroutineHelper.RestartCoroutine(ref _conditionEventSchedulerCoroutine, ConditionEventScheduler(), this);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                CoroutineHelper.RestartCoroutine(ref _eventSchedulerCoroutine, EventScheduler(), this);
                CoroutineHelper.RestartCoroutine(ref _conditionEventSchedulerCoroutine, ConditionEventScheduler(), this);
            }
        }

        public void EnqueueAction(Action action)
        {
            ActionsQueue.Enqueue(action);
        }
        
        public void EnqueueConditionAction(Action action, Func<bool> predicate)
        {
            ConditionActionsQueue.Enqueue(new ConditionAction(action, predicate));
        }

        private IEnumerator EventScheduler()
        {
            while (gameObject.activeInHierarchy)
            {
                if (ActionsQueue.Count > 0)
                {
                    ActionsQueue.Dequeue()?.Invoke();
                }

                yield return null;
            }
        }
        
        private IEnumerator ConditionEventScheduler()
        {
            while (gameObject.activeInHierarchy)
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
                    yield return null;
                }

                yield return null;
            }
        }
    }
}