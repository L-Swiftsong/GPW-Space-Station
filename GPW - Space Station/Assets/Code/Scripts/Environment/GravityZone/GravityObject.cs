using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Environment.GravityZone
{
    public class GravityObject : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private CharacterController _characterController;

        
        [Header("Settings")]
        [SerializeField] private float _defaultGravityScale = -19.61f;
        [SerializeField] private float _defaultDrag = 1.0f;

        [Space(5)]
        [SerializeField] private float _enterLowGravityForceMagnitude = 1.0f;
        private List<AlteredGravityZone> _currentGravityZones = new List<AlteredGravityZone>();


        #region Properties

        private float _gravityScale => _currentGravityZones.Count > 0 ? _currentGravityZones[0].GravityMultiplier : 1.0f;
        private Vector3 _gravityForce => Vector3.up * _defaultGravityScale * _gravityScale;

        #endregion


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _characterController = GetComponent<CharacterController>();
        }


        public void AddToGravityZone(AlteredGravityZone zone)
        {
            _currentGravityZones.Add(zone);
            SortGravityZones();
            UpdateGravitySettings();
        }
        public void RemoveFromGravityZone(AlteredGravityZone zone)
        {
            _currentGravityZones.Remove(zone);
            UpdateGravitySettings();
        }

        private void SortGravityZones()
        {
            // Order our currentGravityZones list so that they are ordered in descending order by their gravity strength.
            // This lets us take the first element (The one with the largest Gravity Multiplier) to use as our active zone.
            _currentGravityZones = _currentGravityZones.OrderByDescending(t => t.GravityMultiplier).ToList();
        }
        private void UpdateGravitySettings()
        {
            if (_rigidbody != null)
            {
                // Using a Rigidbody.
                _rigidbody.useGravity = false;

                if (_gravityScale < 1.0f)
                {
                    // Our current zone is one with lower gravity than default.
                    // We apply an initial force so that the object visually starts floating upon entering the zone.
                    _rigidbody.AddForce(Vector3.up * _enterLowGravityForceMagnitude, ForceMode.VelocityChange);
                }
                _rigidbody.drag = _currentGravityZones.Count > 0 ? _currentGravityZones[0].DragStrength : _defaultDrag;
            }
            else
            {
                // Using a CharacterController.
                throw new System.NotImplementedException();
            }
        }


        private void FixedUpdate()
        {
            if (_rigidbody != null)
            {
                ApplyRigidbodyGravity();
                ApplyRigidbodyDrift();
            }
            else
            {
                ApplyRigidbodyGravity();
                ApplyRigidbodyDrift();
            }
        }


        #region Rigidbody

        private void ApplyRigidbodyGravity()
        {
            // Apply the force of gravity to the Rigidbody.
            _rigidbody.AddForce(_gravityForce, ForceMode.Force);
        }
        private void ApplyRigidbodyDrift()
        {
            
        }

        #endregion

        #region Character Controller

        private void ApplyCharacterControllerGravity()
        {
            throw new System.NotImplementedException();
        }
        private void ApplyCharacterControllerDrift()
        {
            throw new System.NotImplementedException();
        }

        #endregion


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponent<Rigidbody>() == null && GetComponent<CharacterController>() == null)
            {
                Debug.LogError($"Error: The GravityObject instance on {this.name} requires it to have either a Rigidbody or CharacterController component");
            }
        }
#endif
    }
}