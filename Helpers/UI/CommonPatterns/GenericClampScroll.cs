using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.UI.BaseUiElements;
using UniversalUnity.Helpers.UI.BaseUiElements.GenericDynamicContainer;

namespace UniversalUnity.Helpers.UI.CommonPatterns
{
    public abstract class GenericClampScroll<TElementType, TElementData> : 
        GenericDynamicContainer<TElementType, TElementData> 
        where TElementType : BaseDynamicUiElement<TElementType, TElementData> 
        where TElementData : BaseDynamicUiElement<TElementType, TElementData>.DynamicUiElementData
    {
        [Header("GenericClampScroll Fileds")]
        [SerializeField] protected int pageSize = 6;
        [SerializeField] protected int elementsPerScroll = 6;
        [SerializeField] [CanBeNull] protected BaseInteractableUiElement nextPageButton;
        [SerializeField] [CanBeNull] protected BaseInteractableUiElement previousPageButton;

        protected Dictionary<string, TElementData> Data = new Dictionary<string,TElementData>();
        
        /// <summary>
        /// Index of the last data on page
        /// </summary>
        protected int CurrentDataIndex = 0;
        
        public int CurrentPage { get; protected set; }

        private void Awake()
        {
            if (nextPageButton != null) nextPageButton.OnClick += () => Scroll(true);
            if (previousPageButton != null) previousPageButton.OnClick += () => Scroll(false);
        }

        public void InitData(Dictionary<string, TElementData> data)
        {
            Data = data;
            CurrentDataIndex = 0;
            ProtectedInitData(data);
        }

        public void ReplaceItemData(string dataId, TElementData data)
        {
            Data[dataId] = data;
            OpenPage(CurrentDataIndex);
        }
        
        protected virtual void ProtectedInitData(Dictionary<string, TElementData> data)
        {
            
        }

        [CanBeNull] 
        public Coroutine Scroll(bool next)
        {
            var pageData = new List<TElementData>();
            var dataAsList = Data.Values.ToList();
            int dataIndex;
            
            // If go to next page
            if (next)
            {
                dataIndex = CurrentDataIndex + 1;
                Debug.Log("_currentDataIndex before next: " + CurrentDataIndex);
                Debug.Log("dataAsList.Count: " + dataAsList.Count);
                Debug.Log("dataIndex: " + dataIndex);

                // if we already on last data element
                if (dataAsList.Count - 1 == CurrentDataIndex) return null;
                
                CurrentDataIndex += elementsPerScroll;
                
                // If current element index after adding elements will be bigger than data count
                if (CurrentDataIndex >= dataAsList.Count)
                {
                    CurrentDataIndex = dataAsList.Count - 1;
                }
                Debug.Log("_currentDataIndex: " + CurrentDataIndex);
            }
            // If go to previous page
            else
            {
                // If we are already on the first page
                if (pageSize - 1 >= CurrentDataIndex) return null;

                Debug.Log(CurrentDataIndex);
                CurrentDataIndex = pageSize * ((CurrentDataIndex - elementsPerScroll) / pageSize + 1) - 1;
                dataIndex = pageSize - CurrentDataIndex - 1;
                Debug.Log("_currentDataIndex: " + CurrentDataIndex);
                Debug.Log("dataIndex: " + dataIndex);
            }
            
            for (int i = dataIndex; i < dataAsList.Count && i < dataIndex + pageSize; i++)
            {
                pageData.Add(dataAsList[i]);
            }

            foreach (var cardData in pageData)
            {
                cardData.DeInit();
            }

            return UpdateItems(pageData, pageData.Count);
        }
        
        public Coroutine OpenPage(int page)
        {
            var dataAsList = Data.Values.ToList();
            var pageData = new List<TElementData>();

            // If there is no such page -> go to last
            if (page * pageSize >= dataAsList.Count)
            {
                page = dataAsList.Count / pageSize;
            }
            
            int dataIndex = page * pageSize;
            
            for (int i = dataIndex; i < dataAsList.Count && i < dataIndex + pageSize; i++)
            {
                pageData.Add(dataAsList[i]);
            }
            
            foreach (var cardData in pageData)
            {
                cardData.DeInit();
            }

            CurrentPage = page;
            CurrentDataIndex = dataIndex + pageData.Count - 1;
            return UpdateItems(pageData, pageData.Count);
        }
    }
}