/* CONTEXT
 * This script handles the behavior of a low gravity zone. It applies low gravity effects
 * to objects with the "Gravity" tag and to the player when they enter the trigger zone.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.GravityZone
{
    public class AlteredGravityZone : MonoBehaviour, ITriggerable
    {
        [SerializeField] private bool _shouldStartEnabled = true;
        
        private bool m_isEnabled = true;
        private bool _isEnabled
        {
            get => m_isEnabled;
            set
            {
                if (m_isEnabled == value)
                {
                    // Our value hasn't changed.
                    return;
                }

                m_isEnabled = value;
                if (value == true)
                {
                    // The zone has been enabled.
                    // Notify the gravity objects currently occupying this zone so that they experience the gravity change.
                    AddToAllGravityObjects();
                }
                else
                {
                    // The zone has been disabled.
                    // Notify the gravity objects currently occupying this zone so that they no longer experience the gravity change.
                    RemoveFromAllGravityObjects();
                }
            }
        }

        private List<GravityObject> _gravityObjectsInBounds = new List<GravityObject>();


        [Header("Gravity Settings")]
        [SerializeField] private float _gravityScaleMultiplier = 0.2f; 


        [Header("Drift Settings")]
        [SerializeField] private float _dragStrength = 0.2f;


        #region Properties

        public float GravityMultiplier => _gravityScaleMultiplier;
        public float DragStrength => _dragStrength;

        #endregion


        private void Awake() => _isEnabled = _shouldStartEnabled;


        #region ITriggerable Methods

        public void Trigger() => _isEnabled = !_isEnabled;
        public void Activate() => _isEnabled = true;
        public void Deactivate() => _isEnabled = false;

        #endregion


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GravityObject>(out GravityObject gravityObject))
            {
                _gravityObjectsInBounds.Add(gravityObject);

                if (_isEnabled)
                {
                    // This zone is active, and so we should immediately notify the Gravity Object that it has entered.
                    gravityObject.AddToGravityZone(this);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<GravityObject>(out GravityObject gravityObject))
            {
                _gravityObjectsInBounds.Remove(gravityObject);

                if (_isEnabled)
                {
                    gravityObject.RemoveFromGravityZone(this);
                }
            }
        }

        
        private void AddToAllGravityObjects()
        {
            for(int i = 0; i < _gravityObjectsInBounds.Count; ++i)
            {
                _gravityObjectsInBounds[i].AddToGravityZone(this);
            }
        }
        private void RemoveFromAllGravityObjects()
        {
            for (int i = 0; i < _gravityObjectsInBounds.Count; ++i)
            {
                _gravityObjectsInBounds[i].RemoveFromGravityZone(this);
            }
        }
    }
}