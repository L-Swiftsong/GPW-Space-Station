using UnityEngine;

namespace Saving.LevelData
{
    [System.Serializable]
    public class ObjectSaveData
    {
        [field: ReadOnly] [field: SerializeField] public SerializableGuid ID { get; set; }
        
        [ReadOnly] public Vector3 Position;
        [ReadOnly] public Quaternion Rotation;


        [ReadOnly] public bool Exists = false;
        [ReadOnly] public DisabledState DisabledState = DisabledState.None;
        [Space(5)]

        [ReadOnly] public bool[] BoolValues;
        [ReadOnly] public int[] IntValues;



        #region Rounding for Space Optimisation

        private float ROUNDING_FACTOR = 1000.0f;
        public void OnBeforeSave()
        {
            Position = new Vector3(Round(Position.x), Round(Position.y), Round(Position.z));
            Rotation = new Quaternion(Round(Rotation.x), Round(Rotation.y), Round(Rotation.z), Round(Rotation.w));
        }
        public void OnAfterSave() => TranslateSavedValues();
        public void OnBeforeLoad() => TranslateSavedValues();

        private void TranslateSavedValues()
        {
            Position = Position / ROUNDING_FACTOR;
            Rotation = new Quaternion(Rotation.x / ROUNDING_FACTOR, Rotation.y / ROUNDING_FACTOR, Rotation.z / ROUNDING_FACTOR, Rotation.w / ROUNDING_FACTOR);
        }
        private int Round(float value) => Mathf.RoundToInt(value * ROUNDING_FACTOR);

        #endregion
    }
}