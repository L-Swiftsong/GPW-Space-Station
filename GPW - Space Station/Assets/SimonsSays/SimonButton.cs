using System.Collections;
using UnityEngine;
using Interaction;  // Ensure this matches the namespace where IInteractable is declared
using System;

public class SimonButton : MonoBehaviour, IInteractable
{
    [Header("Button Settings")]
    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private Color originalColor = Color.white;

    // Reference to the SimonSays manager (assign in Inspector)
    [SerializeField] private SimonSays simonSays;

    private Renderer _renderer;

    [field: SerializeField] public bool IsInteractable { get; set; } = true;

    public event Action OnSuccessfulInteraction;
    public event Action OnFailedInteraction;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
            Debug.LogError($"{gameObject.name} is missing a Renderer component!");
    }

    /// <summary>
    /// Flashes the button's original color briefly then turns it off.
    /// </summary>
    public IEnumerator FlashOriginalColor()
    {
        if (_renderer != null)
        {
            _renderer.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
            _renderer.material.color = Color.black;
        }
    }

    /// <summary>
    /// Flashes the button with a given color briefly then turns it off.
    /// </summary>
    public IEnumerator FlashColor(Color colorToFlash)
    {
        if (_renderer != null)
        {
            _renderer.material.color = colorToFlash;
            yield return new WaitForSeconds(flashDuration);
            _renderer.material.color = Color.black;
        }
    }

    /// <summary>
    /// Immediately sets the button’s color.
    /// </summary>
    public void SetColor(Color color)
    {
        if (_renderer != null)
        {
            _renderer.material.color = color;
        }
    }

    // --- IInteractable Implementation ---

    public void Interact(PlayerInteraction player)
    {
        if (simonSays != null && simonSays.IsFlashingSequence)
        {
            return;
        }
        if (simonSays != null)
        {
            simonSays.OnPlayerInput(gameObject);
            Debug.Log($"Interacted with {gameObject.name} via IInteractable");
        }
    }

    public void Highlight()
    {
        if (simonSays != null && simonSays.IsFlashingSequence)
        {
            return;
        }
        if (_renderer != null)
        {
            _renderer.material.color = originalColor;
        }
    }

    public void StopHighlighting()
    {
        if (_renderer != null)
        {
            _renderer.material.color = Color.black;
        }
    }
}
