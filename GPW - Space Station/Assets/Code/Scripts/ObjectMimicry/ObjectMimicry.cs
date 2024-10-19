using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GPW.Tests.Mimicry
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


        [Header("Testing")]
        [SerializeField] private bool _attemptMimicry;
        [SerializeField] private bool _stopMimicry;


        [Header("Debug")]
        private List<MimicableObject> _allMimicTargets = new List<MimicableObject>();


        private void Update()
        {
            if (_attemptMimicry)
            {
                _attemptMimicry = false;
                AttemptMimicry();
            }

            if (_stopMimicry)
            {
                _stopMimicry = false;
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
                PerformMimicry(_selectedMimicTarget.GetGraphicsParent());
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


        private void PerformMimicry(Transform mimickedGraphicsPrefab)
        {
            RemoveExistingMimicGraphics();
            
            // Disable our default graphics.
            _defaultGFXParent.gameObject.SetActive(false);


            // Instantiate the mimicked object's graphics.
            Transform mimickedGraphics = Instantiate(mimickedGraphicsPrefab, _mimicryGFXParent, false);
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