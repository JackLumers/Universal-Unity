using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Common.Helpers
{
    public class ShuffledList<T>
    {
        private readonly IEnumerable<T> _originalList;
        private readonly Random _random;

        private List<T> _shuffledList;
        private int _currentIndex = 0;

        public ShuffledList(IEnumerable<T> originalList)
        {
            _random = new Random();

            _originalList = new List<T>(originalList);
            Reshuffle();
        }

        public int Count()
        {
            return _shuffledList.Count() - _currentIndex;
        }

        public void Reshuffle()
        {
            _shuffledList = _originalList.OrderBy(x => _random.Next()).ToList();
            _currentIndex = 0;
        }

        public T GetElement(bool reshuffleIfEnded = false)
        {
            if (_currentIndex > _originalList.Count() && reshuffleIfEnded)
            {
                Reshuffle();
            } else if (_currentIndex > _originalList.Count() && !reshuffleIfEnded)
            {
                throw new IndexOutOfRangeException("No elements left. You need to reshuffle " +
                                                   "list in order to get elements again. " +
                                                   "If you can't enable \"reshuffleIfEnded\" param use Count() to check" +
                                                   "if you can get an element.");
            }

            var element = _shuffledList[_currentIndex];
            _currentIndex++;

            return element;
        }

        public List<T> GetElements(int quantity, bool reshuffleIfEnded = false)
        {
            var elements = new List<T>();
            for (int i = 0; i < quantity; i++)
            {
                elements.Add(GetElement(reshuffleIfEnded));
            }

            return elements;
        }

        public IEnumerable<T> GetElements()
        {
            for (;;) yield return GetElement(true);
        }
    }
}