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


        [Header("Mimicry")]
        [SerializeField] private Transform _defaultGFXParent;
        [SerializeField] private Transform _mimicryGFXParent;


        [Header("Testing")]
        [SerializeField] private bool _attemptMimicry;
        [SerializeField] private bool _stopMimicry;


        [Header("Debug")]
        private List<MimicableObject> _allMimicTargets;
        private MimicableObject _selectedMimicTarget;


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

            if (TryGetMimicTarget(out MimicableObject mimicTarget))
            {
                _selectedMimicTarget = mimicTarget;
                PerformMimicry(mimicTarget);
            }
        }

        private bool CanMimic() => true;
        private bool TryGetMimicTarget(out MimicableObject mimicTarget)
        {
            List<MimicableObject> mimicTargets = new List<MimicableObject>();
            foreach (Collider potentialMimicTarget in Physics.OverlapSphere(transform.position, _maxMimicryRadius, _mimicableLayers))
            {
                if (potentialMimicTarget.TryGetComponentThroughParents<MimicableObject>(out MimicableObject mimicableObject))
                {
                    mimicTargets.Add(mimicableObject);
                }
            }

            _allMimicTargets = mimicTargets;

            if (mimicTargets.Count <= 0)
            {
                mimicTarget = null;
                return false;
            }


            // Cache found targets for gizmos.
            mimicTarget = mimicTargets[Random.Range(0, mimicTargets.Count)];
            return true;
        }

        private void PerformMimicry(MimicableObject mimicTarget)
        {
            // Hide our default gfx.
            _defaultGFXParent.gameObject.SetActive(false);


            // Instantiate the mimicked object's gfx.
            Transform mimickedGFX = Instantiate(mimicTarget.MimicGFXPrefab, _mimicryGFXParent, false);
        }
        private void StopMimicry()
        {
            // Hide the mimicked gfx.
            foreach (Transform child in _mimicryGFXParent)
            {
                Destroy(child.gameObject);
            }

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
                Gizmos.DrawSphere(mimicableObject.transform.position, 0.2f);
            }
        }
    }
}