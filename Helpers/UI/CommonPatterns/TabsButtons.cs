using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniversalUnity.Helpers.UI.BaseUiElements;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns
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

        private CancellationTokenSource _tabChangingCancellationTokenSource;

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
                        ChangeTab(i1).Forget();
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
            tabSlider.Disable().Forget();
        }
        
        public async UniTask ChangeTab(int index)
        {
            if (index < 0 || index > TabsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is less than 0 or greater than tabs count!");
            }
            
            _tabChangingCancellationTokenSource?.Cancel();
            _tabChangingCancellationTokenSource?.Dispose();
            _tabChangingCancellationTokenSource = new CancellationTokenSource();
            
            if (CurrentTab == -1) tabSlider.Enable().Forget();
            
            CurrentTab = index;
            Vector3 target = tabs[index].button.transform.localPosition;
            
            ChangingTabLocked = true;
            await tabSlider.transform.DOLocalMove(target, 3.5f)
                .WithCancellation(_tabChangingCancellationTokenSource.Token)
                .SuppressCancellationThrow();
            ChangingTabLocked = false;
        }
    }
}