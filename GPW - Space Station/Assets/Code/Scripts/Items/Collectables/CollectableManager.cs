using System;
using System.Collections.Generic;

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
                    UnityEngine.Debug.LogWarning($"Warning: {item.name.ToString()} doesn't have a place in the CollectableDataOrderSO for the type {_orderData.Type.ToString()}");
                }

                // All existing elements should precede the new item (Or our new item hasn't had a index set.
                // Add the new item to the end of the list.
                _list.Add(item);
            }
            public List<CollectableData> AsList() => _list;
        }
    }
}
