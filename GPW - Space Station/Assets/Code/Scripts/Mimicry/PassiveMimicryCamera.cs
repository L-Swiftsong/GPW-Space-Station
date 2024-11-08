using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mimicry
{
    public class PassiveMimicryCamera : MonoBehaviour
    {
        private Camera _mimicryCamera;
        private Camera _playerCamera;
        [SerializeField] private Transform _mimicryObject;

        [Space(5)]
        [SerializeField] private bool _test;
        [SerializeField] private float _renderThreshold;
        [SerializeField] private Collider _mimicryObjectCollider;


        private void Awake()
        {
            _mimicryCamera = GetComponent<Camera>();
            _playerCamera = PlayerManager.Instance.GetPlayerCamera();
            _renderThreshold = _mimicryObjectCollider != null ? _mimicryObjectCollider.bounds.extents.magnitude : 1.0f;
        }

        private void Update()
        {
            if (_test)
            {
                
            }
        }


        
    }
}