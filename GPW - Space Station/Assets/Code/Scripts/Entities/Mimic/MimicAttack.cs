using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Mimic;
using Entities.Mimic.States;

public class MimicAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent _agent;
    private ChaseState _chaseState;
    private ChaseMimic _chaseMimic;
    private PlayerHealth _playerHealth;

    private GameObject _player;

    [Header("Settings")]
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _mimicSpeedAfterAttack;

    [HideInInspector] public bool _isAttacking = false;


    void Start()
    {
        // Get PlayerHealth script from "Player" game object
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerHealth = _player.GetComponent<PlayerHealth>();

        // Check if script is attached to general or chase mimic
        if (gameObject.name == "GeneralMimic")
        {
            _chaseState = GetComponent<ChaseState>();
        }
        else if (gameObject.name == "ChaseMimic")
        {
            _chaseMimic = GetComponent<ChaseMimic>();
        }
    }

    void Update()
    {
        // Start attack if general/chase mimic catches player and isnt currently attacking
        if (_chaseState != null && _chaseState._hasCaughtPlayer && !_isAttacking)
        {
            StartCoroutine(Attack());
        }
        else if (_chaseMimic != null && _chaseMimic._hasCaughtPlayer && !_isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        _isAttacking = true;

        // Damage Player
        if (_playerHealth != null)
        {
            _playerHealth.PlayerTakeDamage(25);
        }

        // Temporarily reduce mimic speed after attack
        float originalSpeed = _agent.speed;
        _agent.speed = _mimicSpeedAfterAttack;

        yield return new WaitForSeconds(_attackCooldown);

        // Restore mimic state
        _agent.speed = originalSpeed;
        _isAttacking = false;

        if (_chaseState != null)
        {
            _chaseState._hasCaughtPlayer = false;
        }
        else if (_chaseMimic != null)
        {
            _chaseMimic._hasCaughtPlayer = false;
        }
    }
}
