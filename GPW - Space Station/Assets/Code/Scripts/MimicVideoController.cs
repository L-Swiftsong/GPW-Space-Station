using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Effects.Mimicry.PassiveMimicry;

[RequireComponent(typeof(NavMeshAgent), typeof(PassiveMimicryController))]
public class MimicVideoController : MonoBehaviour
{
    [System.Serializable] 
    private struct MoveControl
    {
        [SerializeField] public KeyCode KeyCode;
        [SerializeField] public Vector3 TargetPosition;

        [Space(5)]
        [SerializeField] public bool UseTargetRotation;
        [SerializeField] public Vector3 TargetEulerAngles;

        public MoveControl(KeyCode keyCode, Vector3 targetPosition)
        {
            this.KeyCode = keyCode;
            this.TargetPosition = targetPosition;


            this.UseTargetRotation = false;
            this.TargetEulerAngles = Vector3.zero;
        }
    }
    [System.Serializable]
    private struct MimicryControl
    {
        [SerializeField] public KeyCode KeyCode;
        [SerializeField] public float TargetStrength;

        public MimicryControl(KeyCode keyCode, float targetStrength)
        {
            this.KeyCode = keyCode;
            this.TargetStrength = targetStrength;
        }
    }


    [Header("References")]
    [SerializeField] private PassiveMimicryController _passiveMimicryController;
    [SerializeField] private NavMeshAgent _navMeshAgent;


    [Header("Controls")]
    [SerializeField] private MoveControl[] _movementControls = new MoveControl[] { new MoveControl(KeyCode.A, Vector3.zero), new MoveControl(KeyCode.D, Vector3.zero) };
    [SerializeField] private MimicryControl[] _mimicryControls = new MimicryControl[] { new MimicryControl(KeyCode.Q, 0.0f), new MimicryControl(KeyCode.E, 1.0f) };

    private Vector3? _currentTargetDirection = null;


    private void Update()
    {
        // Update current movement control.
        for(int i = 0; i < _movementControls.Length; ++i)
        {
            if (Input.GetKeyDown(_movementControls[i].KeyCode))
            {
                _navMeshAgent.updateRotation = true;
                _navMeshAgent.SetDestination(_movementControls[i].TargetPosition);
                _currentTargetDirection = _movementControls[i].UseTargetRotation ? _movementControls[i].TargetEulerAngles : null;
                break;
            }
        }

        // Update current mimicry control. 
        for(int i = 0; i < _mimicryControls.Length; ++i)
        {
            if (Input.GetKeyDown(_mimicryControls[i].KeyCode))
            {
                _passiveMimicryController.SetMimicryStrengthTarget(_mimicryControls[i].TargetStrength);
                break;
            }
        }


        // If we've reached our destination and have a desired direction, rotate to face the desired direction.
        if (_currentTargetDirection.HasValue && (_navMeshAgent.destination - transform.position).sqrMagnitude < 0.25f)
        {
            _navMeshAgent.updateRotation = false;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(_currentTargetDirection.Value), _navMeshAgent.angularSpeed * Time.deltaTime);
        }
    }
}
