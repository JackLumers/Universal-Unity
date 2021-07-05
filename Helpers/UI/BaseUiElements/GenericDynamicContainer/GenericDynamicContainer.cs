// By Ilyas Kharisov, Fair Games, 2020.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Helpers.Coroutines;
using Common.Helpers.Pooling.SimplePool;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Helpers.UI.BaseUiElements.GenericDynamicContainer
{
    /// <summary>
    /// Container for dynamic changing game objects, like items in scroll view and e.t.c.
    /// Objects RE_instantiation optimized by pooling.
    /// 
    /// Note that all instantiated items will be in memory, if not cleared MANUALLY using
    /// <see cref="ForceClear"/> or <see cref="AwaitClear"/> or <see cref="AwaitClearInOrder"/>.
    /// 
    /// For scroll view optimization with multiple entries you will need to use your own scroll view optimization.
    /// </summary>
    public abstract class GenericDynamicContainer<TElementType, TElementData> : MonoBehaviour
        where TElementType : BaseDynamicUiElement<TElementType, TElementData>
        where TElementData : BaseDynamicUiElement<TElementType, TElementData>.DynamicUiElementData
    {
        [Header("GenericDynamicContainer Fileds")]
        [SerializeField] private TElementType prefab = null;
        [SerializeField] private LayoutGroup layoutToContainItems = null;

        [NotNull] protected abstract string PoolName { get; }

        /// <summary>
        /// key - instanceId of the item. (See <see cref="UnityEngine.Object.GetInstanceID"/>);
        /// <para></para>
        /// value - item of <see cref="TElementType"/>
        /// </summary>
        private readonly Dictionary<int, TElementType> _itemsInContainer =
            new Dictionary<int, TElementType>();

        /// <summary>
        /// key - instanceId of the item. (See <see cref="UnityEngine.Object.GetInstanceID"/>);
        /// <para></para>
        /// value - item's data
        /// </summary>
        private readonly Dictionary<int, TElementData> _itemsDataDictionary =
            new Dictionary<int, TElementData>();

        protected Coroutine UpdateProcess;
        protected Coroutine ClearInOrderProcess;
        protected Coroutine ClearProcess;

        private void OnDestroy()
        {
            ClearAndDestroyPool();
            InheritOnDestroy();
        }

        protected virtual void InheritOnDestroy()
        {

        }

        public bool HasItem(TElementType item)
        {
            if (!ReferenceEquals(item, null))
            {
                return _itemsInContainer.ContainsKey(item.GetInstanceID());
            }

            throw new NullReferenceException(nameof(item));
        }

        public bool HasItemWithData(TElementData data)
        {
            if (ReferenceEquals(data, null)) return false;
            return !ReferenceEquals(data.Item, null) && _itemsDataDictionary.ContainsKey(data.Item.GetInstanceID());
        }

        /// <summary>
        /// Updates all items in the container.
        /// </summary>
        /// <param name="data">List of parameters for items</param>
        /// <param name="numberOfElementsPerStep">How many elements will be spawn at the same time</param>
        /// <param name="reverse">Add items from bottom of the enumerable</param>
        /// <typeparam name="TElementType"><see cref="BaseDynamicUiElement{TElementType, TElementData}"/> - item with dynamic parameters initialization</typeparam>
        public Coroutine UpdateItems(IEnumerable<TElementData> data, int numberOfElementsPerStep = 1,
            bool reverse = false)
        {
            if (!SimplePool.IsPoolForObjectsExist(PoolName))
                SimplePool.CreatePool(PoolName);
            ForceClear();
            gameObject.SetActive(true);
            return CoroutineHelper.RestartCoroutine(ref UpdateProcess,
                UpdateItemsProcess(data, numberOfElementsPerStep, reverse), this);
        }

        /// <summary>
        /// Adds 1 item in the container.
        /// </summary>
        /// <param name="data">Parameters for item</param>
        /// <param name="siblingIndex">Index of this item in container (-1 by default). 0 - first; -1 - last.</param>
        /// <typeparam name="TElementType"><see cref="BaseDynamicUiElement{TElementType, TElementData}"/> - item with dynamic parameters initialization</typeparam>
        public Coroutine AddItem(TElementData data, int siblingIndex = -1)
        {
            if (!HasItemWithData(data))
            {
                if (!SimplePool.IsPoolForObjectsExist(PoolName))
                    SimplePool.CreatePool(PoolName);

                return StartCoroutine(AddItemProcess(data, siblingIndex));
            }

            throw new InvalidOperationException("There is already presented this data with linked item in this container!");
        }

        /// <summary>
        /// Removes specified item from the container and deletes it's data.
        /// </summary>
        public Coroutine RemoveItem(TElementType item)
        {
            if (HasItem(item))
            {
                return StartCoroutine(RemoveItemProcess(item));
            }

            throw new InvalidOperationException("There is no such item in this container!");
        }

        /// <summary>
        /// Removes specified item from the container and deletes it's data.
        /// </summary>
        public Coroutine RemoveItem(TElementData itemData)
        {
            try
            {
                if (HasItemWithData(itemData))
                {
                    if (itemData.Item != null)
                        return StartCoroutine(RemoveItemProcess(_itemsInContainer[itemData.Item.GetInstanceID()]));
                }

                throw new NullReferenceException("There is no any item linked with this data!");
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("There is no item linked with this data in this container!");
            }
        }

        public Coroutine ReplaceItemData([NotNull] TElementType item, [NotNull] TElementData newData)
        {
            try
            {
                return StartCoroutine(ReplaceItemDataProcess(_itemsInContainer[item.GetInstanceID()], newData));
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError("[GenericDynamicContainer.ReplaceItemData] There is no such item in this container!");
                throw;
            }
        }

        /// <summary>
        /// Returns all items contained in this container by this exact time.
        /// </summary>
        /// <returns></returns>
        public TElementType[] GetItems()
        {
            return _itemsInContainer.Values.ToArray();
        }

        public TElementType GetItem(TElementData data)
        {
            return _itemsInContainer[data.Item.GetInstanceID()];
        }
            
        /// <summary>
        /// Clears all container form items with animation that is defined by items one by one.
        /// </summary>
        public Coroutine AwaitClearInOrder()
        {
            StopAllCoroutines();
            return CoroutineHelper.RestartCoroutine(ref ClearInOrderProcess, AwaitClearInOrderProcess(), this);
        }

        /// <summary>
        /// Clears all container from items with animation that is defined by items.
        /// </summary>
        public Coroutine AwaitClear()
        {
            StopAllCoroutines();
            return CoroutineHelper.RestartCoroutine(ref ClearProcess, AwaitClearProcess(), this);
        }

        /// <summary>
        /// Clears all container from items without animation.
        /// </summary>
        public void ForceClear()
        {
            StopAllCoroutines();
            foreach (var item in _itemsInContainer)
            {
                SimplePool.Return(item.Value, PoolName);
            }

            _itemsInContainer.Clear();
            _itemsDataDictionary.Clear();
        }

        public void ClearAndDestroyPool()
        {
            foreach (var item in _itemsInContainer)
            {
                Destroy(item.Value);
            }

            if (SimplePool.IsPoolForObjectsExist(PoolName)) SimplePool.RemovePool(PoolName);

            _itemsInContainer.Clear();
            _itemsDataDictionary.Clear();
        }

        protected IEnumerator UpdateItemsProcess(IEnumerable<TElementData> data,
            int numberOfElementsPerStep, bool reverse)
        {
            var objectsData = data.ToArray();
            
            if (numberOfElementsPerStep == 0 && objectsData.Length > 0)
            {
                throw new ArgumentException("Param numberOfElementsPerStep must be more that zero if there is data elements more than zero.");
            }

            int i = reverse ? (objectsData.Length - 1) : 0;
            bool isNotComplete = reverse ? (i >= 0) : (i < objectsData.Length);
            while (isNotComplete)
            {
                // if elements number in step more than items data left, then reduce step
                if (numberOfElementsPerStep > objectsData.Length - i)
                {
                    numberOfElementsPerStep = objectsData.Length - i;
                }

                /*        Step         */
                for (int j = 0; j < (numberOfElementsPerStep - 1) && (i < objectsData.Length); j++, i++)
                {
                    if (!HasItemWithData(objectsData[i]))
                    {
                        StartCoroutine(AddItemProcess(objectsData[i], -1));
                    }
                    else
                    {
                        throw new InvalidOperationException("There is already presented this data with linked item in this container!");
                    }
                }

                // Waiting enabling of the last element in step
                if (!HasItemWithData(objectsData[i]))
                {
                    yield return StartCoroutine(AddItemProcess(objectsData[i], -1));
                }
                else
                {
                    throw new InvalidOperationException("There is already presented this data with linked item in this container!");
                }
                
                /*       Step end       */

                i += reverse ? -1 : 1;
                isNotComplete = reverse ? (i >= 0) : (i < objectsData.Length);
            }
        }

        protected IEnumerator AddItemProcess(TElementData data, int siblingIndex)
        {
            var item = SimplePool.Get(prefab, layoutToContainItems.transform, PoolName);

            if (siblingIndex != -1)
            {
                item.transform.SetSiblingIndex(siblingIndex);
            }

            item.SetAlphaImmediately(0);
            item.Init(data);
            _itemsInContainer.Add(item.GetInstanceID(), item);
            _itemsDataDictionary.Add(item.GetInstanceID(), data);
            yield return item.Enable();
        }

        protected IEnumerator RemoveItemProcess(TElementType item)
        {
            yield return item.Disable();
            SimplePool.Return(item, PoolName);
            _itemsDataDictionary.Remove(item.GetInstanceID());
            _itemsInContainer.Remove(item.GetInstanceID());
        }

        protected IEnumerator ReplaceItemDataProcess(TElementType item, TElementData newData)
        {
            yield return item.Disable();
            _itemsDataDictionary[item.GetInstanceID()] = newData;
            item.Init(newData);
            yield return item.Enable();
        }

        protected IEnumerator AwaitClearInOrderProcess()
        {
            foreach (var item in _itemsInContainer)
            {
                yield return item.Value.Disable();
                SimplePool.Return(item.Value, PoolName);
            }

            _itemsInContainer.Clear();
            _itemsDataDictionary.Clear();
        }

        protected IEnumerator AwaitClearProcess()
        {
            if (_itemsInContainer.Count == 0) yield break;

            var last = _itemsInContainer.Last().Value;
            foreach (var item in _itemsInContainer.Where(item => !ReferenceEquals(last, item.Value)))
            {
                item.Value.Disable();
            }

            yield return last.Disable();

            foreach (var item in _itemsInContainer)
            {
                SimplePool.Return(item.Value, PoolName);
            }

            _itemsInContainer.Clear();
            _itemsDataDictionary.Clear();
        }
    }

    /// <summary>
    /// UI item that can be initialized with different parameters.
    /// </summary>
    /// <example>
    /// In ScrollView, if you want to fill content in runtime, and objects in this content needs to be initialized with different parameters each time they are loaded,
    /// you can inherit scripts of your items from this class and use <see cref="GenericDynamicContainer{TElementType, TElementData}"/> to update this items.
    /// </example>
    /// <seealso cref="BaseInteractableDynamicUiElement{TItem,TData}"/>
    public abstract class BaseDynamicUiElement<TElementType, TElementData> : BaseUiElement
        where TElementType : BaseDynamicUiElement<TElementType, TElementData>
        where TElementData : BaseDynamicUiElement<TElementType, TElementData>.DynamicUiElementData
    {
        public bool IsDataInitialized => !ReferenceEquals(Data, null);

        [CanBeNull] public TElementData Data { get; private set; }

        /// <summary>
        /// Call to put parameters in this item and link data with this item
        /// </summary>
        protected internal void Init(TElementData data)
        {
            data.Init(this);
            Data = data;
            ImplementInit(data);
        }

        /// <summary>
        /// TODO: CALL ON EVERY CLEAR OPERATION!!!!!
        /// </summary>
        private protected void DeInit()
        {
            Data?.DeInit();
        }

        /// <summary>
        /// <inheritdoc cref="Init"/>
        /// <para>
        /// Called when item added in container.
        /// </para>
        /// </summary>
        protected abstract void ImplementInit([NotNull] TElementData data);

        public abstract class DynamicUiElementData
        {
            [CanBeNull] public BaseDynamicUiElement<TElementType, TElementData> Item { get; private set; }

            protected internal void Init(BaseDynamicUiElement<TElementType, TElementData> relativeItem)
            {
                Item = relativeItem;
            }

            /// <summary>
            /// TODO: CALL ON EVERY CLEAR OPERATION!!!!!
            /// </summary>
            protected internal void DeInit()
            {
                Item = null;
            }
        }
    }
}