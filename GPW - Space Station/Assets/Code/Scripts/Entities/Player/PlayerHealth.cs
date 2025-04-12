using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _maxHealth = 4;
    [SerializeField, ReadOnly] public int _currentHealth;
    public bool _isDead;


    [Header("Damage")]
    [SerializeField] private float _damageCooldown = 0.25f;
    private bool _isOnDamageCooldown = false;


    [Header("VFX")]
    [SerializeField] private HealthValueToVFX[] _healthValueToVFXReferences;
    private bool _updatHealthUI = true;

    private bool _hasTriggeredDeathCutscene = false;

    [System.Serializable]
    private struct HealthValueToVFX
    {
        public int HealthValue;
        public GameObject HealthVisorUI;
    }


    [Header("Healing")]
    [SerializeField, ReadOnly] private bool _isHealing = false;
    private Coroutine _healingCoroutine;

    public bool IsHealing => _isHealing;
    public event System.Action OnUsedHealthKit;

    void Awake()
    {
        // Set health to max at start of the game.
        _currentHealth = _maxHealth;

        _isDead = false;
        _isOnDamageCooldown = false;
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }

        //Updates health UI to accurately reflect current health/damage taken
        UpdateHealthUI();


        if (_currentHealth > _maxHealth)
        {
            // Ensure the player doesn't overheal.
            _currentHealth = _maxHealth;
        }
        else if (_currentHealth <= 0 && !_isDead)
        {
            // We have just died.
            _isDead = true;
        }
    }

    //Function to call player damage from external scripts
    public bool TakeDamage(int damage)
    {
        if (_isOnDamageCooldown) //Only take damage if enemies attack cooldown is finished
        {
            return false;
        }
        
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        _isOnDamageCooldown = true;
        StartCoroutine(StartDamageCooldown());

        if (_currentHealth <= 0 && !_isDead)
        {
            _isDead = true;
            return true; // Player just died
        }

        return false; // Player is still alive
    }

    // (Temp Implementation) Show the game over UI once the player dies.
    private void Die() => UI.GameOver.GameOverUI.Instance.ShowGameOverUI();

    void UpdateHealthUI()
    {
        if (_updatHealthUI)
        {
            // Changes visor state depending on current health.
            for (int i = 0; i < _healthValueToVFXReferences.Length; ++i)
            {
                _healthValueToVFXReferences[i].HealthVisorUI.SetActive(_healthValueToVFXReferences[i].HealthValue == _currentHealth);
            }
        }
    }

    IEnumerator StartDamageCooldown()
    {
        //Timer for when the enemy can damage the player again
        yield return new WaitForSeconds(_damageCooldown);
        _isOnDamageCooldown = false;
    }


    /// <summary> Start healing the player from a Medkit.</summary>
    public void StartHealing(int healingAmount, float healingDelay)
    {
        if (_healingCoroutine != null)
        {
            StopCoroutine(_healingCoroutine);
        }

        _healingCoroutine = StartCoroutine(HealPlayerAfterDelay(healingAmount, healingDelay));
    }
    /// <summary> Stop the current healing process.</summary>
    public void CancelHealing()
    {
        if (_healingCoroutine != null)
        {
            StopCoroutine(_healingCoroutine);
        }

        _isHealing = false;
    }

    // Healing process functionality.
    IEnumerator HealPlayerAfterDelay(int healingAmount, float healingDelay)
    {
        // Start healing.
        _isHealing = true;

        // Wait a few seconds before player is healed and health pack disappears.
        yield return new WaitForSeconds(healingDelay);

        // Finish healing.
        _isHealing = false;
        _currentHealth += healingAmount;
        OnUsedHealthKit?.Invoke();
    }

    public IEnumerator DeathCutscene(GameObject mimic)
    {
        if (_hasTriggeredDeathCutscene)
            yield break;

        _hasTriggeredDeathCutscene = true;

        // Get necessary references from the Mimic object.
        MimicAttack mimicAttack = mimic.GetComponent<MimicAttack>();
        Transform mimicLookAt = mimic.transform.Find("MimicLookAt");

        if (mimicAttack.SkipVisorDamageOnKill)
        {
            _updatHealthUI = false;
            DisableHealthVisors();
        }

        // Trigger focus on the Mimic that killed the player.
        if (mimicLookAt != null)
        {
            CameraFocusLook.TriggerFocusLookStatic(mimicLookAt.gameObject, 3f, 7.5f, PlayerInput.ActionTypes.Movement | PlayerInput.ActionTypes.Camera);
        }

        // Play death jumpscare audio.
        mimicAttack.StartCoroutine(mimicAttack.PlaySound(mimicAttack._deathSoundClip, mimicAttack._deathSoundDelay, mimicAttack._deathSoundVolume));

        // Wait before applying other effects.
        yield return new WaitForSeconds(1f);

        // Play final audio cue.
        mimicAttack.StartCoroutine(mimicAttack.PlaySound(mimicAttack._biteSoundClip, mimicAttack._biteSoundDelay, mimicAttack._biteSoundVolume));

        //delay to sync audio.
        yield return new WaitForSeconds(0.5f);

        //Move mimic towrds player.
        yield return mimicAttack.StartCoroutine(mimicAttack.JumpScare());

        // Disable health visors.
        DisableHealthVisors();

        // Show Game Over Menu.
        UI.GameOver.GameOverUI.Instance.ShowGameOverUI();
    }

    private void DisableHealthVisors()
    {
        // Disable the health visors (for UI effects)
        for(int i = 0; i < _healthValueToVFXReferences.Length; ++i)
            _healthValueToVFXReferences[i].HealthVisorUI.SetActive(false);
    }
}
