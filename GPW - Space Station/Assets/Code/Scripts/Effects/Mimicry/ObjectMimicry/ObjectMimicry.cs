using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Effects.Mimicry.ObjectMimicry
{
    public class ObjectMimicry : MonoBehaviour
    {
        [Header("Target Detection")]
        [SerializeField] private float _maxMimicryRadius;
        [SerializeField] private LayerMask _mimicableLayers;
        private MimicableObject _selectedMimicTarget = null;


        [Header("Mimicry")]
        [SerializeField] private Transform _defaultGFXParent;
        [SerializeField] private Transform _mimicryGFXParent;

        [Space(5)]
        private Rigidbody _rigidbody;
        private RigidbodyInformation _defaultRigidbodyInformation;


        [Header("Effects")]
        [SerializeField] private ParticleSystem _mimicryOccuredPS;


        [Header("Testing")]
        [SerializeField] private bool _attemptMimicry = true;
        [SerializeField] private float _timeBetweenMimicryAttempts = 3.0f;
        private float _timeSinceLastMimicry = 0.0f;
        [SerializeField] private bool _stopMimicry = false;


        [Header("Debug")]
        private List<MimicableObject> _allMimicTargets = new List<MimicableObject>();


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _defaultRigidbodyInformation = RigidbodyInformation.CreateFromRigidbody(_rigidbody);

            _timeSinceLastMimicry = 0.0f;
        }
        private void Update()
        {
            if (_attemptMimicry)
            {
                _timeSinceLastMimicry += Time.deltaTime;

                if (_timeSinceLastMimicry > _timeBetweenMimicryAttempts)
                {
                    _timeSinceLastMimicry %= _timeBetweenMimicryAttempts;
                    AttemptMimicry();
                }
            }

            if (_stopMimicry)
            {
                _attemptMimicry = false;
                _stopMimicry = false;
                _timeSinceLastMimicry = 0.0f;

                StopMimicry();
            }
        }


        private void AttemptMimicry()
        {
            if (!CanMimic())
            {
                // We cannot mimic right now.
                return;
            }

            if (DetermineMimicTarget())
            {
                PerformMimicry();
            }
        }

        private bool CanMimic() => true;
        private bool DetermineMimicTarget()
        {
            List<MimicableObject> mimicTargets = new List<MimicableObject>();
            _allMimicTargets.Clear();
            foreach (Collider potentialMimicTarget in Physics.OverlapSphere(transform.position, _maxMimicryRadius, _mimicableLayers))
            {
                if (potentialMimicTarget.TryGetComponentThroughParents<MimicableObject>(out MimicableObject mimicableObject))
                {
                    _allMimicTargets.Add(mimicableObject);


                    if (mimicableObject == _selectedMimicTarget)
                    {
                        continue;
                    }
                    
                    mimicTargets.Add(mimicableObject);
                }
            }

            if (mimicTargets.Count <= 0)
            {
                _selectedMimicTarget = null;
                return false;
            }

            _selectedMimicTarget = mimicTargets[Random.Range(0, mimicTargets.Count)];
            return true;
        }


        private void PerformMimicry()
        {
            RemoveExistingMimicGraphics();
            
            // Disable our default graphics.
            _defaultGFXParent.gameObject.SetActive(false);


            // Instantiate the mimicked object's graphics.
            Transform mimickedGraphics = Instantiate(_selectedMimicTarget.GetGraphicsParent(), _mimicryGFXParent, false);

            if (_selectedMimicTarget.HasRigidbody())
            {
                // Update our Rigidbody to match that of the mimicked object.
                _selectedMimicTarget.GetRigidbodyInformation().ApplyToRigidbody(_rigidbody);
            }


            // Play Effects.
            if (_mimicryOccuredPS != null)
            {
                _mimicryOccuredPS.Play();
            }
        }
        private void RemoveExistingMimicGraphics()
        {
            // Remove the mimicked graphics.
            foreach (Transform child in _mimicryGFXParent)
            {
                Destroy(child.gameObject);
            }
        }
        private void StopMimicry()
        {
            RemoveExistingMimicGraphics();

            // Show the default gfx.
            _defaultGFXParent.gameObject.SetActive(true);

            // Revert our rigidbody to its default values.
            _defaultRigidbodyInformation.ApplyToRigidbody(_rigidbody);


            // Play Effects.
            _mimicryOccuredPS.Play();
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _maxMimicryRadius);
            
            foreach (MimicableObject mimicableObject in _allMimicTargets)
            {
                Gizmos.color = mimicableObject == _selectedMimicTarget ? Color.green : Color.red;
                Gizmos.DrawSphere(mimicableObject.transform.position, 0.3f);
            }
        }
    }
}