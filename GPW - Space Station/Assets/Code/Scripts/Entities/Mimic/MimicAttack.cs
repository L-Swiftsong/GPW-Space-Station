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
        if (_isAttacking)
        {
            FacePlayer();
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
            return;
        _isAttacking = true;

        // Player is dead, skip attack sequence & start death cutscene.
        if (_playerHealth._currentHealth <= 1)
        {
            _playerHealth.TakeDamage(1);
            _playerHealth.StartCoroutine(_playerHealth.DeathCutscene(gameObject));
            return;
        }

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // trigger animation.
        OnAttackPerformed?.Invoke();

        // Continue chasing player until 'hit' part of animation happens.
        yield return new WaitForSeconds(1.2f);

        // Stop movement.
        _navMeshAgent.isStopped = true;

        // Deal damage
        _playerHealth.TakeDamage(1);

        // Perform knockback
        yield return StartCoroutine(PerformKnockback());

        

        // Finish cooldown
        yield return new WaitForSeconds(_attackCooldown);

        // Return movement & attack to mimic.
        _navMeshAgent.isStopped = false;
        _isAttacking = false;
    }

    private IEnumerator PerformKnockback()
    {
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

    private void FacePlayer()
    {
        Vector3 direction = PlayerManager.Instance.Player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    public void SetCanAttack(bool newValue) => _canAttack = newValue;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}
