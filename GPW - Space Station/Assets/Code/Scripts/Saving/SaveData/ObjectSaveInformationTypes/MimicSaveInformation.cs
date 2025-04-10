using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving.LevelData
{
    [System.Serializable]
    public class MimicSaveInformation : ObjectSaveInformation
    {
        public MimicSavableState MimicSavableState { get => (MimicSavableState)this.ObjectSaveData.IntValues[0]; set => this.ObjectSaveData.IntValues[0] = (int)value; }


        public MimicSaveInformation(ObjectSaveData objectSaveData, DisabledState disabledState) : base(objectSaveData, disabledState, boolCount: 0, intCount: 1) { }
        public MimicSaveInformation(SerializableGuid id, DisabledState disabledState, MimicSavableState mimicSavableState) : base(id, disabledState, boolCount: 0, intCount: 1)
        {
            this.MimicSavableState = mimicSavableState;
        }
    }


    [System.Serializable]
    public enum MimicSavableState
    {
        Idle,
        Chasing
    }
}
