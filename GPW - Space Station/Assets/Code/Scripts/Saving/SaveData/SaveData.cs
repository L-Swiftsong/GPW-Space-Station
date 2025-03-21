using UnityEngine;
using System.Linq;

namespace Saving
{
    [System.Serializable]
    public class SaveData
    {
        public bool Exists;
        public float SaveTime;

        public int[] LoadedSceneIndices;
        public int ActiveSceneIndex;

        public PlayerData PlayerData;
        public InventorySaveData ItemSaveData;
    }
}
