using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving.LevelData
{
    public interface ISaveableObject
    {
        SerializableGuid ID { get; set; }

        public void BindExisting(ObjectSaveData saveData);
        public ObjectSaveData BindNew();
    }
}