using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Mimic.States;

namespace Entities.Mimic
{
    public class MimicAnimator : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField] private Animator[] _animators;

        private static readonly int MOVEMENT_SPEED_HASH = Animator.StringToHash("MovementSpeed");
        private static readonly int IS_CRAWLING_HASH = Animator.StringToHash("IsCrawling");

        private static readonly int DETECTED_NOISE_HASH = Animator.StringToHash("DetectedNoise");
        private static readonly int PERFORMED_ATTACK_HASH = Animator.StringToHash("PerformedAttack");
        private static readonly int FLASHLIGHT_STAGGERED_HASH = Animator.StringToHash("FlashlightStaggered");
        private static readonly int DETECTED_PLAYER_HASH = Animator.StringToHash("DetectedPlayer");


        [Header("Mimic Script References")]
        [SerializeField] private GeneralMimic _generalMimic;
        [SerializeField] private EntityMovement _entityMovement;
        [SerializeField] private MimicAttack _mimicAttack;
        [SerializeField] private NavMeshAgent _agent;


        private void OnEnable()
        {
            if (_generalMimic != null)
                _generalMimic.OnStateChanged += GeneralMimic_OnStateChanged;
            if (_mimicAttack != null)
                _mimicAttack.OnAttackPerformed += MimicAttack_OnAttackPerformed;
        }
        private void OnDestroy()
        {
            if (_generalMimic != null)
                _generalMimic.OnStateChanged -= GeneralMimic_OnStateChanged;
            if (_mimicAttack != null)
                _mimicAttack.OnAttackPerformed -= MimicAttack_OnAttackPerformed;
        }


        private void Update()
        {
            UpdateAnimators();
        }

        private void UpdateAnimators()
        {
            // Get values.
            bool isCrawling;
            float agentSpeed = _agent.velocity.magnitude;
            if (_entityMovement != null)
            {
                isCrawling = _entityMovement.GetCurrentMovementState() == EntityMovement.MovementState.Crawling;
            }
            else
            {
                isCrawling = false;
            }

            // Set animator values.
            for (int i = 0; i < _animators.Length; ++i)
            {
                _animators[i].SetFloat(MOVEMENT_SPEED_HASH, agentSpeed);
                _animators[i].SetBool(IS_CRAWLING_HASH, isCrawling);
            }
        }


        public void GeneralMimic_OnStateChanged(State newState)
        {
            if (newState.GetType() == typeof(PreparingToChaseState)) { OnDetectedPlayer(); }
            else if (newState.GetType() == typeof(StunnedState)) { OnFlashlightStaggered(); }
            else if (newState.GetType() == typeof(SearchState)) { OnDetectedNoise(); }
        }
        private void MimicAttack_OnAttackPerformed() => OnPerformedAttack();


        public void OnDetectedNoise() => SetTrigger(DETECTED_NOISE_HASH);
        public void OnPerformedAttack() => SetTrigger(PERFORMED_ATTACK_HASH);
        public void OnFlashlightStaggered() => SetTrigger(FLASHLIGHT_STAGGERED_HASH);
        public void OnDetectedPlayer() => SetTrigger(DETECTED_PLAYER_HASH);


        private void SetTrigger(int id)
        {
            for(int i = 0; i < _animators.Length; ++i)
            {
                _animators[i].SetTrigger(id);
            }
        }
    }
}
