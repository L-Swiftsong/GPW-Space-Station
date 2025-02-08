using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Collectables
{
    [CreateAssetMenu(menuName = "Collectables/New Order List", fileName = "CollectableOrderList", order = -1)]
    public class CollectableDataOrderSO : ScriptableObject
    {
        [SerializeField] private CollectableDataType _type;
        [SerializeField] private List<CollectableData> _collectableData;


        public System.Type Type => _type switch
        {
            CollectableDataType.Codex => typeof(CodexData),

            _ => throw new System.NotImplementedException($"Conversion to System.Type is not implemented for CollectableDataType: {_type.ToString()}"),
        };
        
        public int GetDataIndex(CollectableData data) => _collectableData.IndexOf(data);


        [System.Serializable]
        private enum CollectableDataType
        {
            Codex
        }
    }

    public static class CollectableDataOrderManager
    {
        public static Dictionary<System.Type, CollectableDataOrderSO> s_AllCollectableOrdersList;
        private const string COLLECTABLES_FOLDER_PATH = "Collectables/";

        static CollectableDataOrderManager()
        {
            s_AllCollectableOrdersList = new Dictionary<System.Type, CollectableDataOrderSO>();

            foreach (CollectableDataOrderSO collectableDataOrderSO in Resources.LoadAll(COLLECTABLES_FOLDER_PATH, typeof(CollectableDataOrderSO)))
            {
                if (!s_AllCollectableOrdersList.TryAdd(collectableDataOrderSO.Type, collectableDataOrderSO))
                {
                    // Failed to add (We have duplicate instances).
                    Debug.LogError($"Error: We have multiple CollectableDataOrderSO instances for the Type: {collectableDataOrderSO.Type}");
                }
            }
        }
    }
}
