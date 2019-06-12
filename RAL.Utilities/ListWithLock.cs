using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TheColonel2688.Utilities
{

    public class ListWithLock<T> : IList<T>
    {
        public List<T> hidden;


        public ListWithLock()
        {
            hidden = new List<T>();
        }

        public int Count
        {
            get 
            {
                lock (hidden)
                {
                    return hidden.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                lock (hidden)
                {
                    return hidden[index];
                }
            }

            set
            {
                lock (hidden)
                {
                    hidden[index] = value;
                }
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            lock (hidden)
            {
                hidden.AddRange(items);
            }
        }

        public void Add(T item)
        {
            lock (hidden)
            {
                hidden.Add(item);
            }
        }

        public void Clear()
        {
            lock (hidden)
            {
                hidden.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (hidden)
            {
                return hidden.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (hidden)
            {
                hidden.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (hidden)
            { 
                return Clone().GetEnumerator();
            }
        }

        public Collection<T> Clone()
        {

            lock (hidden)
            {

                Collection<T> clonedList = new Collection<T>();
                foreach(var item in hidden)
                {
                    clonedList.Add(item);
                }
                return clonedList;
            }
        }

        public bool Remove(T item)
        {
            lock (hidden)
            {
                return hidden.Remove(item);
            }
        }

        public void RemoveRange(int index, int count)
        {
            lock (hidden)
            {
                hidden.RemoveRange(index, count);
            }
        }

        public bool RemoveMany(IEnumerable<T> items)
        {

            lock (hidden)
            {
                foreach (var item in items)
                {
                    if (!hidden.Remove(item))
                    {
                        throw new InvalidOperationException($"Count Not Remove item {item.ToString()} from {nameof(ListWithLock<T>)}'s hidden Collection");
                    }
                }
                return true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (hidden) { return Clone().GetEnumerator(); }
                
            
        }

        public int IndexOf(T item)
        {
            //** This is gross and deprecated don't use it.
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            lock (hidden)
            {
                hidden.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (hidden)
            {
                hidden.RemoveAt(index);
            }
        }


        /// <summary>
        /// Trims off "Front" of List leaving the back N items
        /// </summary>
        /// <param name=""></param>
        public void TrimToSize(int numberOfItemsToLeave)
        {
            lock (hidden)
            {
                hidden.RemoveRange(hidden.Count - numberOfItemsToLeave, numberOfItemsToLeave);
            }
        }
    }
}
