using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Player;

public class MimicAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent _agent;
    private PlayerHealth _playerHealth;

    [Header("Settings")]
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _attackRadius = 1.0f;

    [Space(5)]
    [SerializeField] private float _mimicSpeedAfterAttack;

    [HideInInspector] public bool _isAttacking = false;
    private bool _canAttack = true;

    private void Start()
    {
        if (PlayerManager.Instance.Player.TryGetComponent<PlayerHealth>(out _playerHealth) == false)
        {
            // Failed to get PlayerHealth reference.
            Debug.LogError("Error: Chase Mimic initialised without PlayerHealth reference");
        }
    }
    void Update()
    {
        if (_isAttacking || !_canAttack)
        {
            return;
        }

        // Start attack if general/chase mimic catches player and isnt currently attacking.
        if ((transform.position - PlayerManager.Instance.Player.position).sqrMagnitude <= _attackRadius * _attackRadius)
        {
            PerformAttack();
        }
    }


    private void PerformAttack()
    {
        if (_isAttacking)
        {
            return;
        }
        _isAttacking = true;


        // Damage the Player.
        _playerHealth.TakeDamage(1);


        // Attack Recovery.
        StartCoroutine(AttackRecovery());
    }
    IEnumerator AttackRecovery()
    {
        // Temporarily reduce mimic speed after attack.
        float originalSpeed = _agent.speed;
        _agent.speed = _mimicSpeedAfterAttack;

        yield return new WaitForSeconds(_attackCooldown);

        // Restore mimic state.
        _agent.speed = originalSpeed;
        _isAttacking = false;
    }


    public void SetCanAttack(bool newValue) => _canAttack = newValue;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}
