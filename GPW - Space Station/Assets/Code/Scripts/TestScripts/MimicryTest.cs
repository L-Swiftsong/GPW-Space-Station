using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mimicry.PassiveMimicry;


namespace Testing.Mimicry
{
    [RequireComponent(typeof(PassiveMimicryController))]
    public class MimicryTest : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        private PassiveMimicryController _mimicryController;


        [Header("Strength Settings")]
        [SerializeField] private float _minMimicryDistance;
        [SerializeField] private float _maxMimicryDistance;
        [SerializeField] private AnimationCurve _mimicryStrengthCurve;


        [Header("Debug")]
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private Color _minDistanceColour;
        [SerializeField] private Color _maxDistanceColour;


        private void Awake() => _mimicryController = GetComponent<PassiveMimicryController>();
        private void Start() => _player = FindObjectOfType<PlayerController>().transform; // Replace ASAP.

        private void Update()
        {
            // Determine the percentage distance between this object and the player, clamped between min and max mimicry distance.
            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            float percentageDistance = Mathf.Clamp01((distanceToPlayer - _minMimicryDistance) / (_maxMimicryDistance - _minMimicryDistance));

            // Determine the percentage strength to apply for our mimicry.
            float mimicryPercentageStrength = _mimicryStrengthCurve.Evaluate(percentageDistance);
            
            // Set our controller's passiveMimicryRamp value to the percentage strength
            _mimicryController.SetPassiveMimicryRamp(mimicryPercentageStrength);
        }


        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
            {
                return;
            }

            Gizmos.color = _minDistanceColour;
            Gizmos.DrawWireSphere(transform.position, _minMimicryDistance);
            
            Gizmos.color = _maxDistanceColour;
            Gizmos.DrawWireSphere(transform.position, _maxMimicryDistance);
        }
    }
}