using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NPOTests
{
    class DynamicArray<T> : IEnumerable<T>
    {
        private int defaultInitialSize = 10;
        private T[] array;
        private int currentIndex = 0;

        public T this[int index] => Items[index];
        public int Count => currentIndex;
        public T[] Items
        {
            get
            {
                T[] result = new T[currentIndex];
                Array.Copy(array, 0, result, 0, currentIndex);
                return result;
            }
        }

        public DynamicArray()
        {
            array = new T[defaultInitialSize];
        }
        public DynamicArray(int initialSize)
        {

            if (initialSize > 0)
            {
                array = new T[initialSize];
            }
            else
            {
                throw new ArgumentException("Incorrect array size");
            }
        }
        public void Add(T value)
        {
            if (currentIndex == array.Length)
            {
                var newSize = array.Length * 2;
                var newArray = new T[newSize];
                array.CopyTo(newArray, 0);
                array = newArray;
            }
            array[currentIndex++] = value;
        }
        public bool Remove(T value)
        {
            int index = Array.IndexOf<T>(array, value, 0, currentIndex); ;
            if (index < 0)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }
        public void RemoveAt(int index)
        {
            if (index >= currentIndex)
                throw new IndexOutOfRangeException();
            var copy = new T[array.Length];
            array.CopyTo(copy, 0);
            Array.Copy(array, index + 1, copy,
                index, array.Length - index - 1);
            array = copy;
            currentIndex--;
        }
        public void Clear()
        {
            currentIndex = 0;
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var value in Items)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
