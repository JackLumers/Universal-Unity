using System;
using System.Collections;
using Common.Helpers.Coroutines;
using Common.Helpers.Tweeks.CurveAnimationHelper;
using Common.Helpers.UI.BaseUiElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Helpers.UI.CommonPatterns
{
    /// <summary>
    /// Realisation of "tabs" design pattern
    /// </summary>
    [RequireComponent(typeof(LayoutGroup))]
    public class TabsButtons : BaseUiElement
    {
        [Serializable]
        private class Tab
        {
            [SerializeField] public BaseInteractableUiElement button = null;
            [SerializeField] public UnityEvent onClick = null;
        }
        
        [SerializeField] private BaseUiElement tabSlider = null;
        [SerializeField] private int firstSelectedTab = 0;
        [SerializeField] private Tab[] tabs = null;
        
        public int TabsCount => tabs.Length;
        
        /// <summary>
        /// Current selected tab. -1 if not any.
        /// </summary>
        public int CurrentTab { get; private set; } = 0;
        public bool ChangingTabLocked { get; private set; } = false;

        private Coroutine _tabChangingCoroutine;

        protected override void InheritInitComponents()
        {
            var i = 0;
            foreach (var tab in tabs)
            {
                var i1 = i;
                tab.button.OnClick += () =>
                {
                    if (!ChangingTabLocked && CurrentTab != i1)
                    {
                        tab.onClick.Invoke();
                        ChangeTab(i1);
                    }
                };
                i++;
            }
        }

        private void OnPreRender()
        {
            tabSlider.transform.localPosition = tabs[firstSelectedTab].button.transform.localPosition;
            CurrentTab = firstSelectedTab;
        }

        /// <summary>
        /// Deselects any selected tab.
        /// </summary>
        public void DeselectAny()
        {
            CurrentTab = -1;
            tabSlider.Disable();
        }
        
        public void ChangeTab(int index)
        {
            if (index < 0 || index > TabsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is less than 0 or greater than tabs count!");
            }

            if (CurrentTab == -1) tabSlider.Enable();
            
            CurrentTab = index;
            Vector3 target = tabs[index].button.transform.localPosition;
            CoroutineHelper.RestartCoroutine(ref _tabChangingCoroutine, MovingProcess(target), this);
        }
        
        private IEnumerator MovingProcess(Vector3 target)
        {
            ChangingTabLocked = true;
            yield return CurveAnimationHelper.Move(tabSlider.transform, target, speedOrTime: 3.5f, isLocalPosition: true);
            ChangingTabLocked = false;
        }
    }
}