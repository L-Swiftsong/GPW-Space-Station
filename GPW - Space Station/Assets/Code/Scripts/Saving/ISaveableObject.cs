using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving.LevelData
{
    public interface ISaveableObject
    {
        SerializableInstanceGuid ID { get; set; }

        public void BindExisting(ObjectSaveData saveData);
        public ObjectSaveData BindNew();
        public void InitialiseID();
    }
}