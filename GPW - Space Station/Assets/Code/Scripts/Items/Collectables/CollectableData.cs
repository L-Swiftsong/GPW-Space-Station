using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Collectables
{
    public abstract class CollectableData : ScriptableObject
    {
        [SerializeField] private string _collectableName;


        public string CollectableName => _collectableName;
    }
}
