using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
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


    [Header("NavMeshLink Traversal")]
    [SerializeField] private AnimationCurve _ventEnterCurve;
    private float _ventEnterDuration => _ventEnterCurve.keys[_ventEnterCurve.length - 1].time;
    
    [SerializeField] private AnimationCurve _ventExitCurve;
    private float _ventExitDuration => _ventExitCurve.keys[_ventExitCurve.length - 1].time;
    
    private bool _onNavMeshLink;


    [Header("GFX")]
    [SerializeField] private Transform _gfx;
    [SerializeField] private float _defaultHeight = 1.0f;
    private Vector3 _defaultScale;
    private Vector3 _defaultPos;

    [Space(5)]
    [SerializeField] private float _crouchHeight = 0.5f;
    private Vector3 _crouchScale;
    private Vector3 _crouchPos;

    [Space(5)]
    [SerializeField] private AnimationCurve _crouchHeightChangeCurve;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoTraverseOffMeshLink = false;
        _onNavMeshLink = false;

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

        if (_agent.isOnOffMeshLink && !_onNavMeshLink)
        {
            // We've just entered a NavMeshLink.
            StartNavMeshLinkMovement();
        }

        DetermineCurrentLayers();

        UpdateSpeed();
        UpdateGFX();
    }


    #region NavMeshLink Movement
    // Ref: 'https://github.com/SunnyValleyStudio/Diablo-Like-Movement-in-Unity-using-AI-Navigation-package/blob/main/AgentMover.cs'.

    private void StartNavMeshLinkMovement()
    {
        _onNavMeshLink = true;

        NavMeshLink link = (NavMeshLink)_agent.navMeshOwner;
        PerformLinkMovement(link);
    }

    private void PerformLinkMovement(NavMeshLink link)
    {
        bool reverseDirection = CheckIfExitingVent(link);
        Vector3 targetPos = reverseDirection ? link.gameObject.transform.TransformPoint(link.startPoint) : link.gameObject.transform.TransformPoint(link.endPoint);

        Debug.Log("Exiting?: " + reverseDirection);
        StartCoroutine(MoveOnOffMeshLink(targetPos, reverseDirection));
    }
    /// <summary>
    ///     Determine whether the agent is closer to the start or end of a NavMeshLink, and therefore whether we are travelling from the Start to End or vice versa.
    /// </summary>
    /// <param name="link">The NavMeshLink that we are testing against.</param>
    /// <returns> True if we are moving from the End to the Start of the NavMeshLink. False if otherwise.</returns>
    private bool CheckIfExitingVent(NavMeshLink link)
    {
        Vector3 startPos = link.gameObject.transform.TransformPoint(link.startPoint);
        Vector3 endPos = link.gameObject.transform.TransformPoint(link.endPoint);

        float distanceAgentToStart = Vector3.Distance(_agent.transform.position, startPos);
        float distanceAgentToEnd = Vector3.Distance(_agent.transform.position, endPos);

        return distanceAgentToStart > distanceAgentToEnd;
    }

    private IEnumerator MoveOnOffMeshLink(Vector3 targetPos, bool reverseDirection)
    {
        float currentTime = 0.0f;
        float duration = reverseDirection ? _ventExitDuration : _ventEnterDuration;

        Vector3 agentStartPosition = _agent.transform.position;

        while (currentTime < duration)
        {
            // Calculate our lerpTime.
            currentTime += Time.deltaTime;
            float lerpTime = Mathf.Clamp01(currentTime / _ventEnterDuration);

            // Handle our position change.
            float positionLerpValue = reverseDirection ? _ventExitCurve.Evaluate(lerpTime) : _ventEnterCurve.Evaluate(lerpTime);
            _agent.transform.position = Vector3.Lerp(agentStartPosition, targetPos, positionLerpValue);

            // Ensure that our Y-position is always at the desired level.
            //_agent.transform.position = new Vector3(_agent.transform.position.x, targetPos.y, _agent.transform.position.z);


            // Handle our GFX Changes (Would be in our animation for the Mimic proper).
            float gfxLerpValue = _crouchHeightChangeCurve.Evaluate(reverseDirection ? 1.0f - lerpTime : lerpTime);
            _gfx.localScale = Vector3.Lerp(_defaultScale, _crouchScale, gfxLerpValue);
            _gfx.localPosition = Vector3.Lerp(_defaultPos, _crouchPos, gfxLerpValue);

            yield return null;
        }

        // Finish our movement.
        _agent.CompleteOffMeshLink();

        // Ensure that our scale successfully reached our desired values.
        _gfx.localScale = reverseDirection ? _defaultScale : _crouchScale;
        _gfx.localPosition = reverseDirection ? _defaultPos : _crouchPos;

        // Allow ourself to enter a new NavMeshLink after a short delay.
        yield return new WaitForSeconds(0.1f);
        _onNavMeshLink = false;
    }

    #endregion


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
            //_gfx.localScale = _crouchHeight;
            //_gfx.localPosition = _crouchPos;
            return;
        }

        // We are not crouching.
        //_gfx.localScale = _defaultScale;
        //_gfx.localPosition = _defaultPos;
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
