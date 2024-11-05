using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Testing.Mimicry
{
    public class MimicryTest : MonoBehaviour
    {
        private Transform _player;

        [SerializeField] private Renderer _renderer;
        private Material _mimicryMaterial;

        private const string PASSIVE_MIMICRY_RAMP_IDENTIFIER = "_PassiveMimicryStrength";


        [Header("Strength Settings")]
        [SerializeField] private float _minMimicryDistance = 1.5f;
        [SerializeField] private float _maxMimicryDistance = 3.0f;
        [SerializeField] private AnimationCurve _mimicryStrengthCurve;


        [Header("Debug")]
        [SerializeField] private bool _drawGizmos = false;
        [SerializeField] private Color _minDistanceColour = Color.red;
        [SerializeField] private Color _maxDistanceColour = Color.green;


        private void Awake() => _mimicryMaterial = _renderer.material;
        private void Start() => _player = PlayerManager.Instance.Player;

        private void Update()
        {
            if (_player == null)
            {
                return;
            }
            
            // Determine the percentage distance between this object and the player, clamped between min and max mimicry distance.
            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            float percentageDistance = Mathf.Clamp01((distanceToPlayer - _minMimicryDistance) / (_maxMimicryDistance - _minMimicryDistance));

            // Determine the percentage strength to apply for our mimicry.
            float mimicryPercentageStrength = _mimicryStrengthCurve.Evaluate(percentageDistance);

            // Set our controller's passiveMimicryRamp value to the percentage strength
            _mimicryMaterial.SetFloat(PASSIVE_MIMICRY_RAMP_IDENTIFIER, mimicryPercentageStrength);
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