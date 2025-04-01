using System;
using System.Collections.Generic;
using System.Linq;

namespace Items.Collectables
{
    public static class CollectableManager
    {
        private static Dictionary<Type, CollectableDataList> _obtainedCollectableData = new Dictionary<Type, CollectableDataList>();


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
                _obtainedCollectableData.Add(dataType, new CollectableDataList(collectableData));
            }
        }

        /// <remarks>
        ///     May be unsafe.
        /// </remarks>
        public static List<CollectableData> GetCollectablesOfType(CollectableDataType collectableDataType)
        {
            if (_obtainedCollectableData.ContainsKey(collectableDataType.ToSystemType()))
            {
                return _obtainedCollectableData[collectableDataType.ToSystemType()].AsList();
            }
            else
            {
                return new List<CollectableData>();
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


        public static void PrepareForLoad() => _obtainedCollectableData.Clear();
        public static void LoadObtainedCollectables(CollectableDataType type, bool[] collectableObtainedStates) => LoadObtainedCollectables(type.ToSystemType(), collectableObtainedStates);
        public static void LoadObtainedCollectables(System.Type type, bool[] collectableObtainedStates)
        {
            if (collectableObtainedStates.Any(t => t == true) == false)
            {
                // We haven't obtained any collectables of this type.
                return;
            }

            // Construct a list of our obtained collectable instances.
            int collectableObtainedStatesCount = collectableObtainedStates.Length;
            List<CollectableData> obtainedCollectables = new List<CollectableData>(collectableObtainedStatesCount);
            for (int i = 0; i < collectableObtainedStatesCount; ++i)
            {
                if (collectableObtainedStates[i] == true)
                {
                    obtainedCollectables.Add(CollectableDataOrderManager.s_AllCollectableOrdersList[type].GetDataAtIndex(i));
                }
            }

            // Our obtainedCollectables list SHOULD be all collectables that the player has for this type.
            _obtainedCollectableData.Add(type, new CollectableDataList(obtainedCollectables));
        }
        public static bool[] GetObtainedStateArrayForType(System.Type type)
        {
            if (type.BaseType != typeof(CollectableData))
            {
                throw new ArgumentException($"{type.Name} does not inherit from CollectableData.");
            }

            if (_obtainedCollectableData.TryGetValue(type, out CollectableDataList data))
            {
                // We have an instance.
                return data.AsObtainedArray();
            }
            else
            {
                return new bool[CollectableDataOrderManager.s_AllCollectableOrdersList[type].Count];
            }
        }


        public class CollectableDataList
        {
            private List<CollectableData> _list;
            private CollectableDataOrderSO _orderData;


            public CollectableDataList(CollectableData firstItem)
            {
                // Create our list with our first item.
                _list = new List<CollectableData>() { firstItem };


                // Get a reference to our CollectableOrderData instance, throwing an error if one doesn't exist for our firstItem's type.
                try
                {
                    _orderData = CollectableDataOrderManager.s_AllCollectableOrdersList[firstItem.GetType()];
                }
                catch
                {
                    UnityEngine.Debug.LogError($"Error: No CollectableDataOrderSO for type: {firstItem.GetType()}.");
                }
            }
            public CollectableDataList(List<CollectableData> list)
            {
                _list = list;

                // Get a reference to our CollectableOrderData instance using our first element, throwing an error if one doesn't exist for our first element's type.
                try
                {
                    _orderData = CollectableDataOrderManager.s_AllCollectableOrdersList[list[0].GetType()];
                }
                catch
                {
                    UnityEngine.Debug.LogError($"Error: No CollectableDataOrderSO for type: {list[0].GetType()}.");
                }
            }
            

            public void Add(CollectableData item)
            {
                int newItemIndex = _orderData.GetDataIndex(item);

                if (newItemIndex != -1)
                {
                    for (int i = 0; i < _list.Count; ++i)
                    {
                        if (_orderData.GetDataIndex(_list[i]) > newItemIndex)
                        {
                            // This element should follow the new item in the list.
                            _list.Insert(i, item);
                            return;
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Warning: {item.name.ToString()} doesn't have a place in the CollectableDataOrderSO for the type {_orderData.SystemType.ToString()}");
                }

                // All existing elements should precede the new item (Or our new item hasn't had a index set.
                // Add the new item to the end of the list.
                _list.Add(item);
            }
            public List<CollectableData> AsList() => _list;
            public bool[] AsObtainedArray()
            {
                // Create an array of all existing (Though not neccessarily obtained) data instances for this type. (Default bool value is false).
                bool[] dataObtainedStateArray = new bool[_orderData.Count];

                // Mark all obtained data instances as true.
                for(int i = 0; i < _list.Count; ++i)
                {
                    dataObtainedStateArray[_orderData.GetDataIndex(_list[i])] = true;
                }

                // Return our array.
                return dataObtainedStateArray;
            }
        }
    }
}
