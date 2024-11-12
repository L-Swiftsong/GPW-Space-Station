using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public abstract class InventoryItem : MonoBehaviour
    {
        [SerializeField] private Vector3 _localPosition = Vector3.zero;
        [SerializeField] private Vector3 _localEulerAngles = Vector3.zero;

        /// <summary> Initialise this item for the passed inventory.</summary>
        public virtual void Initialise(PlayerInventory inventory)
        {
            transform.localPosition = _localPosition;
            transform.localEulerAngles = _localEulerAngles;
        }


        /// <summary> Equip this item.</summary>
        public virtual void Equip() => gameObject.SetActive(true);
        /// <summary> Unequip this item.</summary>
        public virtual void Unequip() => gameObject.SetActive(false);


        /// <summary> Start performing the primary use of this item.</summary>
        public virtual void StartUse() { }
        /// <summary> Stop performing the primary use of this item.</summary>
        public virtual void StopUse() { }

        /// <summary> Start performing the alternate use of this item.</summary>
        public virtual void StartAltUse() { }
        /// <summary> Stop performing the alternate use of this item.</summary>
        public virtual void StopAltUse() { }


        /// <summary> Get this item's name.</summary>
        public abstract string GetItemName();
    }
}