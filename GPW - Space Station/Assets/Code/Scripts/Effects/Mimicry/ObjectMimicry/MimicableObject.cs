using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Effects.Mimicry.ObjectMimicry
{
    public class MimicableObject : MonoBehaviour
    {
        [SerializeField] private Transform _graphicsParent;
        public Transform GetGraphicsParent() => _graphicsParent;

        private Rigidbody _rigidbody;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }


        public bool HasRigidbody() => _rigidbody != null;
        public RigidbodyInformation GetRigidbodyInformation() => RigidbodyInformation.CreateFromRigidbody(_rigidbody);
    }
}