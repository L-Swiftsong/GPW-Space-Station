using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Effects.Mimicry.PassiveMimicry;
using Entities.Mimic.States;


namespace Entities.Mimic
{
    [RequireComponent(typeof(NavMeshAgent), typeof(EntitySenses))]
    public class GeneralMimic : MonoBehaviour
    {
        // References.
        private NavMeshAgent _agent;
        private EntitySenses _entitySenses;
        private EntityMovement _entityMovement;
        private FlashlightStunnable _flashlightStunnableScript;
        private PassiveMimicryController _passiveMimicryController;
        private MimicAttack _mimicAttack;

        [Header("Sounds")]
        [SerializeField] private AudioClip _footstepSound1;
        [SerializeField] private AudioClip _footstepSound2;
        [SerializeField] private float _footstepInterval = 0.5f;
        [SerializeField] private AudioSource _audioSource;
        private float _footstepTimer = 0.0f;
        private bool _useFirstFootstep = true;


        [Header("States")]
        [SerializeField] [ReadOnly] private State _currentState;
        [SerializeField] [ReadOnly] private string _currentStatePath;

        private WanderState _wanderState;
        private ChaseState _chaseState;
        private PreparingToChaseState _preparingToChaseState;
        private SearchState _searchState;
        //private VentState _ventState;
        private SetTrapState _setTrapState;
        private StunnedState _stunnedState;



        private void Awake()
        {
            // Get Component References.
            _agent = GetComponent<NavMeshAgent>();
            _entitySenses = GetComponent<EntitySenses>();
            _entityMovement = GetComponent<EntityMovement>();
            _flashlightStunnableScript = GetComponent<FlashlightStunnable>();
            _passiveMimicryController = GetComponent<PassiveMimicryController>();
            _mimicAttack = GetComponent<MimicAttack>();

            _mimicAttack.SetCanAttack(false);
        }

        private void Start()
        {
            // Get State References.
            _wanderState = GetComponent<WanderState>();
            _chaseState = GetComponent<ChaseState>();
            _preparingToChaseState = GetComponent<PreparingToChaseState>();
            _searchState = GetComponent<SearchState>();
            //_ventState = GetComponent<VentState>();
            _setTrapState = GetComponent<SetTrapState>();
            _stunnedState = GetComponent<StunnedState>();


            _audioSource = gameObject.GetComponent<AudioSource>();
            _audioSource.clip = _footstepSound1;
            _audioSource.playOnAwake = false;

            // Start in the wander state.
            SetActiveState(_wanderState);
        }


        private void Update()
        {
            HandleStateTransitions();
            _currentState.OnLogic();

            _currentStatePath = _currentState.Name;

            HandleFootsteps();
        }


        private void HandleStateTransitions()
        {
            // ----- GLOBAL TRANSITIONS -----
            if (_flashlightStunnableScript.IsStunned && _currentState != _stunnedState) // Stunned State.
            {
                // We have been stunned.
                SetActiveState(_stunnedState);
                return;
            }

            // ----- LOCAL TRANSITIONS -----
            if (_currentState == _wanderState) // Transitions FROM WanderState.
            {
                if (_entitySenses.HasTarget)
                {
                    // We can see the player.
                    SetActiveState(_preparingToChaseState);
                    return;
                }
                if (_entitySenses.CurrentPointOfInterest.HasValue)
                {
                    // We have an active Point of Interest.
                    SetActiveState(_searchState);
                    return;
                }


                if (_wanderState.ShouldMakeNewDecision())
                {
                    // We should attempt to exit the Wander State.
                    float _rndBehaviourDecision = Random.Range(0.0f, 1.0f);

                    /*if (_rndBehaviourDecision <= _wanderState.VentChance && _ventState.CanEnter())
                    {
                        SetActiveState(_ventState);
                        return;
                    }
                    else */if (_rndBehaviourDecision <= (_wanderState.VentChance + _wanderState.SetTrapChance) && _setTrapState.CanEnter())
                    {
                        SetActiveState(_setTrapState);
                        return;
                    }
                }
            }
            else if (_currentState == _chaseState) // Transitions FROM ChaseState.
            {
                if (!_entitySenses.HasTarget && _entityMovement.HasReachedDestination() && _mimicAttack._isAttacking == false)
                {
                    // We can no longer see the player and have reached their last spotted position.
                    SetActiveState(_wanderState);
                    return;
                }
            }
            else if (_currentState == _searchState) // Transitions FROM SearchState.
            {
                if (_entitySenses.HasTarget)
                {
                    // We can see the player.
                    SetActiveState(_preparingToChaseState);
                    return;
                }

                if (_entitySenses.CurrentPointOfInterest.HasValue == false)
                {
                    // We no longer have a Point of Interest to travel to.
                    SetActiveState(_wanderState);
                    return;
                }
            }
            else if (_currentState == _preparingToChaseState) // Transitions FROM PreparingToChase.
            {
                if (_preparingToChaseState.CanStartChase())
                {
                    // Start the chase.
                    SetActiveState(_chaseState);
                    return;
                }
            }
            /*else if (_currentState == _ventState) // Transitions FROM VentState.
            {
                if (_entitySenses.HasTarget)
                {
                    // We can see the player.
                    SetActiveState(_preparingToChaseState);
                    return;
                }
                if (_entitySenses.CurrentPointOfInterest.HasValue)
                {
                    // We have an active Point of Interest.
                    SetActiveState(_searchState);
                    return;
                }
                if (_ventState.ShouldExitState())
                {
                    // We should exit this state (Either because we failed to enter it or we exited the vent).
                    SetActiveState(_wanderState);
                    return;
                }
            }*/
            else if (_currentState == _setTrapState) // Transitions FROM SetTrapState.
            {
                if (_entitySenses.HasTarget)
                {
                    // We can see the player.
                    SetActiveState(_preparingToChaseState);
                    return;
                }
                if (_entitySenses.CurrentPointOfInterest.HasValue)
                {
                    // We have an active Point of Interest.
                    SetActiveState(_searchState);
                    return;
                }
                if (_setTrapState.ShouldExitState())
                {
                    // We should exit this state (Either because we failed to enter it or we exited the vent).
                    SetActiveState(_wanderState);
                    return;
                }
            }
            else if (_currentState == _stunnedState)
            {
                if (!_flashlightStunnableScript.IsStunned)
                {
                    // We are no longer stunned.
                    SetActiveState(_wanderState);
                    return;
                }
            }
        }
        private void SetActiveState(State newState)
        {
            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _currentState = newState;
            _currentState.OnEnter();
        }

        private void HandleFootsteps()
        {
            if (_agent.velocity.magnitude > 0.1f)
            {
                _footstepTimer -= Time.deltaTime;

                if (_footstepTimer <= 0f)
                {
                    _footstepTimer = _footstepInterval;

                    // Alternate between the two footstep sounds
                    AudioClip footstepToPlay = _useFirstFootstep ? _footstepSound1 : _footstepSound2;
                    _useFirstFootstep = !_useFirstFootstep;

                    _audioSource.clip = footstepToPlay;
                    _audioSource.PlayOneShot(footstepToPlay);
                }
            }
        }
    }

}