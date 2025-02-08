using System;
using System.Collections;
using System.Collections.Generic;

namespace Items.Collectables
{
    public static class CollectableManager
    {
        private static Dictionary<Type, SortedList<CollectableData>> _obtainedCollectableData = new Dictionary<Type, SortedList<CollectableData>>();


        public static void AddCollectable(CollectableData collectableData)
        {
            // Get the type of the given collectable data (E.g. CodexData).
            Type dataType = collectableData.GetType();

            if (_obtainedCollectableData.ContainsKey(dataType))
            {
                // We have an entry for this collectableType.
                // Add our obtained collectable to the existing list.
                _obtainedCollectableData[dataType].Add(collectableData);
            }
            else
            {
                // We don't have an entry for this collectableType.
                // Create a new entry for this collectableType and add our obtained collectable as the first value.
                _obtainedCollectableData.Add(dataType, new SortedList<CollectableData>() { collectableData });
            }
        }

        public static List<T> GetCollectablesOfType<T>() where T : CollectableData
        {
            if (_obtainedCollectableData.ContainsKey(typeof(T)))
            {
                return _obtainedCollectableData[typeof(T)].AsList().ConvertAll(x => (T)x);
            }
            else
            {
                return new List<T>();
            }
        }


        public class SortedList<T> : IEnumerable<T> where T : IComparable<T>
        {
            private List<T> _list;


            public SortedList() 
            {
                _list = new List<T>();
            }
            

            public void Add(T item)
            {
                for(int i = 0; i < _list.Count; ++i)
                {
                    if (_list[i].CompareTo(item) > 0)
                    {
                        // This element should follow the new item in the list.
                        _list.Insert(i, item);
                        return;
                    }
                }

                // All existing elements should precede the new item.
                // Add the new item to the end of the list.
                _list.Add(item);
            }
            public List<T> AsList() => _list;


            public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
