using System;
using UnityEngine;

namespace Items.Collectables
{
    public abstract class CollectableData : ScriptableObject, IComparable<CollectableData>
    {
        [Header("Collectable Data")]
        [SerializeField] private string _collectableName;
        [SerializeField] private int _sortingOrder;

        public string CollectableName => _collectableName;
        public int SortingOrder => _sortingOrder;



        public int CompareTo(CollectableData other)
        {
            if (this._sortingOrder < other._sortingOrder)
            {
                return -1;
            }
            else if (this._sortingOrder > other._sortingOrder)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
