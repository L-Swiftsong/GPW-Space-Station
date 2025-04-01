using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Effects.Mimicry.PassiveMimicry;
using Entities.Mimic.States;

namespace Entities.Mimic
{
    [RequireComponent(typeof(NavMeshAgent), typeof(EntitySenses))]
    public class GeneralMimic : BaseMimic
    {
        // References.
        private NavMeshAgent _agent;
        private EntitySenses _entitySenses;
        private EntityMovement _entityMovement;
        private FlashlightStunnable _flashlightStunnableScript;
        private PassiveMimicryController _passiveMimicryController;
        private MimicAttack _mimicAttack;


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


        public event System.Action<State> OnStateChanged;



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

            // Start in the wander state.
            SetActiveState(_wanderState);
        }


        private void Update()
        {
            HandleStateTransitions();
            _currentState.OnLogic();

            _currentStatePath = _currentState.Name;
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
                    _wanderState.AttemptingDecision();

                    // We should attempt to exit the Wander State.
                    float _rndBehaviourDecision = Random.Range(0.0f, 1.0f);

                    if (_rndBehaviourDecision <= _wanderState.SetTrapChance && _setTrapState.CanEnter())
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

            OnStateChanged?.Invoke(_currentState);
            UpdateSaveableState();
        }


        protected override Saving.LevelData.MimicSavableState GetSavableState()
        {
            if (_currentState.GetType() == typeof(ChaseState) || _currentState.GetType() == typeof(PreparingToChaseState))
            {
                return Saving.LevelData.MimicSavableState.Chasing;
            }
            else
            {
                return Saving.LevelData.MimicSavableState.Idle;
            }
        }
    }
}