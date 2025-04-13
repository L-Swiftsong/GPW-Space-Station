using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Player;
using Entities.Mimic;
using Audio;

public class MimicAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent _navMeshAgent;
    private PlayerHealth _playerHealth;
    private PlayerController _playerController;
    private GeneralMimic _generalMimic;
    private MimicController _mimicController;

    [Header("Settings")]
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _attackRadius = 1.0f;
    [SerializeField] private float _mimicKnockbackStrength = 2f;
    [SerializeField] private float _mimicKnockbackDuration = 0.5f;
    [SerializeField] private float _mimicJumpScareDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip _attackRecoverySoundClip;
    [SerializeField] [Range(0f, 2f)] private float _attackRecoverySoundVolume = 1f;
    [SerializeField] private float _attackRecoverySoundDelay = 1f;
    [SerializeField] private AudioClip _attackSoundClip;
    [SerializeField] [Range(0f, 2f)] private float _attackSoundVolume = 1f;
    [SerializeField] private float _attackSoundDelay = 1f;
    [SerializeField] public AudioClip _deathSoundClip;
    [SerializeField] [Range(0f, 2f)] public float _deathSoundVolume = 1f;
    [SerializeField] public float _deathSoundDelay = 1f;
    [SerializeField] public AudioClip _biteSoundClip;
    [SerializeField] [Range(0f, 2f)] public float _biteSoundVolume = 1f;
    [SerializeField] public float _biteSoundDelay = 1f;

    [HideInInspector] public bool _isAttacking = false;
    private bool _canAttack = true;

    public event System.Action OnAttackPerformed;

    private bool _hasTriggeredJumpscare = false;

    public bool SkipVisorDamageOnKill = false;

    private void Start()
    {
        if (PlayerManager.Instance.Player.TryGetComponent<PlayerHealth>(out _playerHealth) == false)
        {
            Debug.LogError("Error: Chase Mimic initialised without PlayerHealth reference");
        }

        if (PlayerManager.Instance.Player.TryGetComponent<PlayerController>(out _playerController) == false)
        {
            Debug.LogError("Error: Mimic initialised without PlayerController reference");
        }

        TryGetComponent<GeneralMimic>(out _generalMimic);

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
            if (TryGetComponent<MimicController>(out _mimicController))
            {
                if (_mimicController._cancelOnAttack)
                {
                    _navMeshAgent.isStopped = true;
                    _mimicController.ReplaceMimic();
                    return;
                }
            }

            if (GetComponent<ChaseMimic>())
            {
                SetJumpscareSettings();
                return;
            }

            if (_generalMimic != null && _generalMimic.GetCurrentState() == _generalMimic.GetPreparingToChaseState())
            {
                return;
            }

            PerformAttack();
        }
    }


    private void PerformAttack()
    {
        if (_playerController.GetHiding() == true)
        {
            return;
        }

        if (_isAttacking)
            return;
        _isAttacking = true;

        // Player is dead, skip attack sequence & start death cutscene.
        if (_playerHealth._currentHealth <= 1)
        {
            SetJumpscareSettings();
            return;
        }

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // Trigger attack animation and audio
        OnAttackPerformed?.Invoke();
        StartCoroutine(PlaySound(_attackSoundClip, _attackSoundDelay, _attackSoundVolume));

        // Wait until the attack animation reaches the 'hit' part
        yield return new WaitForSeconds(1.2f);

        // Stop movement, deal damage, apply camera shake & push mimic away from player.
        HandleAttackImpact();

        // Trigger recovery audio.
        StartCoroutine(PlaySound(_attackRecoverySoundClip, _attackRecoverySoundDelay, _attackRecoverySoundVolume));

        // Wait for cooldown and reset attack state.
        yield return new WaitForSeconds(_attackCooldown);
        _navMeshAgent.isStopped = false;
        _isAttacking = false;
    }

    public IEnumerator PlaySound(AudioClip clip, float delay, float volume)
    {
        yield return new WaitForSeconds(delay);
        SFXManager.Instance.PlayClipAtPosition(clip, transform.position, minPitch: 1f, maxPitch: 1f, volume: volume, minDistance: 6.5f, maxDistance: 15f);
    }

    private void HandleAttackImpact()
    {
        _navMeshAgent.isStopped = true;

        if (_playerController.GetHiding())
        {
            return;
        }

        _playerHealth.TakeDamage(1);

        CameraShake.StartEventShake(intensity: 0.135f, speed: 26f, duration: 0.65f);
        StartCoroutine(PerformKnockback());
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

    public IEnumerator JumpScare()
    {
        var collider = GetComponent<CapsuleCollider>();
        collider.enabled = false;

        Transform cam = Camera.main.transform;

        // Move towards camera but keep mimic grounded
        Vector3 targetPosition = cam.position + cam.forward * -0.1f;
        targetPosition.y = transform.position.y;

        float elapsedTime = 0f;
        float duration = _mimicJumpScareDuration;

        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            Vector3 pos = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            pos.y = startPosition.y;
            transform.position = pos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private void SetJumpscareSettings()
    {
        if (_hasTriggeredJumpscare)
            return;

        _hasTriggeredJumpscare = true;

        _navMeshAgent.isStopped = true;
        _playerHealth.TakeDamage(1);
        _playerHealth.StartCoroutine(_playerHealth.DeathCutscene(gameObject));
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
