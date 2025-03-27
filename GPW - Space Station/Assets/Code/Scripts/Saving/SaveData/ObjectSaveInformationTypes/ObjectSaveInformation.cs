using UnityEngine;

namespace Saving.LevelData
{
    [System.Serializable]
    public class ObjectSaveInformation
    {
        [HideInInspector][SerializeField] protected bool[] BoolValues;
        [HideInInspector][SerializeField] protected int[] IntValues;

        private ObjectSaveInformation() { }
        protected ObjectSaveInformation(int boolValuesCount = 0, int intValuesCount = 0)
        {
            BoolValues = new bool[boolValuesCount];
            IntValues = new int[intValuesCount];
        }
    }
}