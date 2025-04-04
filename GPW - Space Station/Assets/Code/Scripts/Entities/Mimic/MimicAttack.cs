using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Player;

public class MimicAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent _navMeshAgent;
    private PlayerHealth _playerHealth;

    [Header("Settings")]
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _attackRadius = 1.0f;
    [SerializeField] private float _mimicKnockbackStrength = 2f;
    [SerializeField] private float _mimicKnockbackDuration = 0.5f;


    [HideInInspector] public bool _isAttacking = false;
    private bool _canAttack = true;


    public event System.Action OnAttackPerformed;

    private bool playerDied = false;

    private void Start()
    {
        if (PlayerManager.Instance.Player.TryGetComponent<PlayerHealth>(out _playerHealth) == false)
        {
            // Failed to get PlayerHealth reference.
            Debug.LogError("Error: Chase Mimic initialised without PlayerHealth reference");
        }

        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (_isAttacking || !_canAttack)
        {
            _navMeshAgent.isStopped = true;
            return;
        }
        else
        {
            _navMeshAgent.isStopped = false;
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

        // Damage the Player and check if they died
        playerDied = _playerHealth.TakeDamage(1);

        OnAttackPerformed?.Invoke();

        if (playerDied) // If Player is dead play cutscene
        {
            _playerHealth.StartCoroutine(_playerHealth.DeathCutscene(gameObject));
        }

        // Attack Recovery.
        StartCoroutine(AttackCooldown());
        StartCoroutine(PerformKnockback());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);
        _isAttacking = false;
    }

    private IEnumerator PerformKnockback()
    {
        if (playerDied)
        {
            _mimicKnockbackStrength = _mimicKnockbackStrength / 2;
        }

        // Calculate the mimic knockback after attack
        Vector3 knockbackDirection = (transform.position - PlayerManager.Instance.Player.position).normalized;
        Vector3 knockbackAmount = knockbackDirection * _mimicKnockbackStrength;

        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + knockbackAmount;

        while (elapsedTime < _mimicKnockbackDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / _mimicKnockbackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }


    public void SetCanAttack(bool newValue) => _canAttack = newValue;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}
