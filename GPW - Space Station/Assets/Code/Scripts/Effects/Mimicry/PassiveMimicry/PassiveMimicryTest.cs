using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects.Mimicry.PassiveMimicry.Testing
{
    [RequireComponent(typeof(PassiveMimicryController))]
    public class PassiveMimicryTest : MonoBehaviour
    {
        private PassiveMimicryController _passiveMimicryController;

        [SerializeField] private float _minStrengthMimicryDistance;
        [SerializeField] private float _maxStrengthMimicryDistance;
        [SerializeField] private AnimationCurve _mimicryStrengthCurve;


        private void Awake() => _passiveMimicryController = GetComponent<PassiveMimicryController>();
        private void Update()
        {
            if (PlayerManager.Exists)
            {
                UpdateMimicryStrength();
            }
        }


        private void UpdateMimicryStrength()
        {
            // Calculate the percentage distance of the player between the defined minimum and maximum distances.
            float playerDistance = Vector3.Distance(PlayerManager.Instance.Player.position, transform.position);
            float playerPercentageDistance = Mathf.Clamp01((playerDistance - _minStrengthMimicryDistance) / (_maxStrengthMimicryDistance - _minStrengthMimicryDistance));

            // Using our percentage distance, calculate our desired mimicry strength.
            float newMimicryStrength = _mimicryStrengthCurve.Evaluate(playerPercentageDistance);
            _passiveMimicryController.SetMimicryStrengthTarget(newMimicryStrength);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _minStrengthMimicryDistance);
            Gizmos.DrawWireSphere(transform.position, _maxStrengthMimicryDistance);
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_minStrengthMimicryDistance > _maxStrengthMimicryDistance)
            {
                Debug.LogError("ERROR: The minimum strength mimicry distance cannot be smaller than the maximum mimicry strength distance.");
            }
        }
#endif
    }
}
