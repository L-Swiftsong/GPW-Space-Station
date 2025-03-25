using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Mimic
{
    public abstract class BaseMimic : MonoBehaviour
    {
        public virtual void Activate() => this.gameObject.SetActive(true);
        public virtual void Deactivate() => this.gameObject.SetActive(true);

        public virtual void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
