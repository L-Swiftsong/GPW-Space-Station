using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveToTarget : MonoBehaviour
{
    private NavMeshAgent _agent;
    private NavMeshLayers _currentLayers;
    [SerializeField] private float _defaultSpeed = 2.0f;

    
    [Header("Targets")]
    [SerializeField] private Transform[] _targets;
    private int _currentTargetIndex;
    [SerializeField] private bool _navigateToTarget;


    [SerializeField] private AnimationCurve _ventEnterCurve;


    [Header("GFX")]
    [SerializeField] private Transform _gfx;
    [SerializeField] private float _defaultHeight = 1.0f;
    private Vector3 _defaultScale;
    private Vector3 _defaultPos;

    [Space(5)]
    [SerializeField] private float _crouchHeight = 0.5f;
    private Vector3 _crouchScale;
    private Vector3 _crouchPos;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _currentTargetIndex = 0;
        
        _defaultScale = new Vector3(_gfx.localScale.x, _defaultHeight, _gfx.localScale.z);
        _defaultPos = _gfx.localPosition;
        _crouchScale = new Vector3(_gfx.localScale.x, _crouchHeight, _gfx.localScale.z);
        _crouchPos = new Vector3(_gfx.localPosition.x, _crouchScale.y, _gfx.localPosition.z);
    }

    private void Update()
    {
        if (_navigateToTarget && (_agent.path == null || _agent.remainingDistance <= 0.5f))
        {
            SelectNextTarget();
            _agent.SetDestination(_targets[_currentTargetIndex].position);
        }

        DetermineCurrentLayers();

        UpdateSpeed();
        UpdateGFX();
    }

    private void DetermineCurrentLayers()
    {
        if (!_agent.SamplePathPosition(_agent.areaMask, 1.0f, out NavMeshHit hit))
        {
            // SamplePathPosition() call succeeded.
            _currentLayers = (NavMeshLayers)hit.mask;
            Debug.Log(_currentLayers.ToString());
        }
        else
        {
            // SamplePathPosition() failed.
            _currentLayers = NavMeshLayers.None;
        }
    }

    private void UpdateSpeed()
    {
        _agent.speed = _defaultSpeed;

        if (_currentLayers.HasFlag(NavMeshLayers.Crawlable))
        {
            // We are crouching.
            _agent.speed *= 0.5f;
        }
    }
    private void UpdateGFX()
    {
        if (_currentLayers.HasFlag(NavMeshLayers.Crawlable))
        {
            // We are crouching.
            _gfx.localScale = _crouchScale;
            _gfx.localPosition = _crouchPos;
            return;
        }

        // We are not crouching.
        _gfx.localScale = _defaultScale;
        _gfx.localPosition = _defaultPos;
    }


    private void SelectNextTarget()
    {
        if (_currentTargetIndex < _targets.Length - 1)
        {
            _currentTargetIndex++;
        }
        else
        {
            _currentTargetIndex = 0;
        }
    }
}
