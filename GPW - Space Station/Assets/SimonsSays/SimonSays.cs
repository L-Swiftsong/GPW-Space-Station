using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSays : MonoBehaviour
{
    public GameObject[] buttons; // Assign terminals here (e.g., Red, Green, Blue)
    public float flashTime = 0.5f;
    public float playerTimeout = 5f;
    public int totalPhases = 7; // Total number of phases

    [Header("Audio Settings")]
    public AudioSource puzzleAudioSource; // AudioSource on the SimonSays puzzle GameObject
    public AudioClip[] flashClips;        // Array of audio clips for each button's flash (order must match buttons)
    public AudioClip failClip;            // Audio clip to play when flashing red (failure)
    public AudioClip successClip;         // Audio clip to play when flashing green (puzzle complete)

    // Keycard fields (if needed for further actions)
    public GameObject keycard;         // Assign the keycard GameObject in the Inspector
    public Transform keycardTarget;    // Target location for keycard once puzzle completes

    private int _currentPhase = 1;
    private List<int> _sequence = new List<int>();
    private int _playerInputIndex = 0;
    private bool _isPlayerTurn = false;
    private bool _isFlashingSequence = false;
    private bool _isGameActive = false;
    private float _timeSinceLastInput = 0f;
    private Dictionary<GameObject, Color> _buttonColors = new Dictionary<GameObject, Color>();
    private float _currentPlayerTimeout;

    public bool puzzleCompleted = false; // Indicates if the puzzle is completed

    // Expose the flashing state so buttons can check if they are allowed to highlight or interact.
    public bool IsFlashingSequence
    {
        get { return _isFlashingSequence; }
    }

    void Start()
    {
        InitializeButtonColors();
    }

    private void InitializeButtonColors()
    {
        foreach (GameObject button in buttons)
        {
            Renderer renderer = button.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Store the original color before creating a new material
                Color originalColor = renderer.material.color;
                _buttonColors[button] = originalColor;

                // Create a unique material instance for this button
                renderer.material = new Material(renderer.material);

                // Set the button's color to black (off)
                renderer.material.color = Color.black;
            }
            else
            {
                Debug.LogError($"Button {button.name} is missing a Renderer component!");
            }
        }
    }

    public void StartPuzzle()
    {
        Debug.Log("Simon Says puzzle started!");
        _isGameActive = true;
        _currentPhase = 1;
        _sequence.Clear();
        puzzleCompleted = false;
        GenerateSequence(); // Generate initial sequence
        StartCoroutine(PlayPhase());
    }

    private IEnumerator PlayPhase()
    {
        while (_currentPhase <= totalPhases && _isGameActive)
        {
            Debug.Log($"Starting Phase {_currentPhase}");

            // Generate the next element in the sequence if not the first phase
            if (_currentPhase > 1)
            {
                GenerateSequence();
            }

            // Play the sequence
            yield return StartCoroutine(PlaySequence());

            // Player's turn
            _isPlayerTurn = true;
            _playerInputIndex = 0;
            _timeSinceLastInput = 0f;
            _currentPlayerTimeout = playerTimeout;

            Debug.Log("Your turn! Repeat the sequence.");

            // Wait until player input is completed or timeout occurs
            while (_isPlayerTurn)
            {
                _timeSinceLastInput += Time.deltaTime;
                if (_timeSinceLastInput >= _currentPlayerTimeout)
                {
                    Debug.Log("You took too long!");
                    ResetPuzzle();
                    yield break;
                }
                yield return null;
            }

            // Proceed to next phase if player was correct
            _currentPhase++;
            yield return new WaitForSeconds(1f);
        }

        if (_isGameActive)
        {
            // Puzzle completed successfully
            StartCoroutine(FlashGreenSuccess(3));
        }
    }

    private void GenerateSequence()
    {
        int nextColorIndex;
        do
        {
            nextColorIndex = Random.Range(0, buttons.Length);
        }
        while (_sequence.Count > 0 && nextColorIndex == _sequence[_sequence.Count - 1]);

        _sequence.Add(nextColorIndex);
        Debug.Log("Generated Sequence: " + string.Join(", ", _sequence));
    }

    private IEnumerator PlaySequence()
    {
        _isFlashingSequence = true;
        Debug.Log("Watch the sequence!");

        foreach (int index in _sequence)
        {
            Debug.Log($"Flashing {buttons[index].name}");
            // Play the flash sound for this button's color from the puzzle's AudioSource
            if (puzzleAudioSource != null && flashClips != null && flashClips.Length > index && flashClips[index] != null)
            {
                puzzleAudioSource.PlayOneShot(flashClips[index]);
            }
            yield return StartCoroutine(FlashButton(index));
            yield return new WaitForSeconds(0.2f);
        }
        _isFlashingSequence = false;
    }

    private IEnumerator FlashButton(int index)
    {
        SimonButton buttonScript = buttons[index].GetComponent<SimonButton>();
        if (buttonScript != null)
        {
            yield return buttonScript.FlashOriginalColor();
        }
        else
        {
            Debug.LogError($"SimonButton script not found on {buttons[index].name}");
        }
    }

    public void OnPlayerInput(GameObject button)
    {
        if (!_isPlayerTurn || _isFlashingSequence || !_isGameActive || puzzleCompleted)
            return;

        int buttonIndex = System.Array.IndexOf(buttons, button);
        if (buttonIndex == -1)
        {
            Debug.Log($"Unknown button pressed: {button.name}");
            return;
        }

        Debug.Log($"Expected: {buttons[_sequence[_playerInputIndex]].name}, Received: {button.name}");

        if (buttonIndex == _sequence[_playerInputIndex])
        {
            Debug.Log($"Correct! Pressed: {button.name}");
            _playerInputIndex++;
            _timeSinceLastInput = 0f;
            if (_playerInputIndex >= _sequence.Count)
            {
                _isPlayerTurn = false;
                Debug.Log("Good job! Sequence completed.");
            }
        }
        else
        {
            Debug.Log($"Wrong Button! Pressed: {button.name}");
            // Play the fail sound from the puzzle's AudioSource
            if (puzzleAudioSource != null && failClip != null)
            {
                puzzleAudioSource.PlayOneShot(failClip);
            }
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
    {
        StopAllCoroutines();
        Debug.Log("Incorrect! Resetting puzzle...");
        _currentPhase = 1;
        _sequence.Clear();
        _isGameActive = false;
        _isPlayerTurn = false;
        _isFlashingSequence = false;
        puzzleCompleted = false;

        // Reset buttons to black (off)
        foreach (GameObject button in buttons)
        {
            SimonButton buttonScript = button.GetComponent<SimonButton>();
            if (buttonScript != null)
            {
                buttonScript.SetColor(Color.black);
            }
        }
        StartCoroutine(ResetSequenceWithFlashes());
    }

    private IEnumerator ResetSequenceWithFlashes()
    {
        _isFlashingSequence = true;
        // Flash red three times. For each flash, play the failClip.
        for (int i = 0; i < 3; i++)
        {
            foreach (GameObject button in buttons)
            {
                if (puzzleAudioSource != null && failClip != null)
                {
                    puzzleAudioSource.PlayOneShot(failClip);
                }
                SimonButton buttonScript = button.GetComponent<SimonButton>();
                if (buttonScript != null)
                {
                    StartCoroutine(buttonScript.FlashColor(Color.red));
                }
            }
            yield return new WaitForSeconds(flashTime + 0.2f);
        }
        // Flash each terminal one by one with its original color.
        foreach (GameObject button in buttons)
        {
            SimonButton buttonScript = button.GetComponent<SimonButton>();
            if (buttonScript != null)
            {
                yield return buttonScript.FlashOriginalColor();
                yield return new WaitForSeconds(0.2f);
            }
        }
        Debug.Log("Resetting puzzle to initial state.");
        _isGameActive = true;
        _sequence.Clear();
        _currentPhase = 1;
        GenerateSequence();
        StartCoroutine(PlayPhase());
    }

    private IEnumerator FlashGreenSuccess(int flashes)
    {
        _isGameActive = false;
        _isPlayerTurn = false;
        _isFlashingSequence = true;
        puzzleCompleted = true;

        Debug.Log("Puzzle completed successfully!");

        for (int i = 0; i < flashes; i++)
        {
            foreach (GameObject button in buttons)
            {
                if (puzzleAudioSource != null && successClip != null)
                {
                    puzzleAudioSource.PlayOneShot(successClip);
                }
                SimonButton buttonScript = button.GetComponent<SimonButton>();
                if (buttonScript != null)
                {
                    StartCoroutine(buttonScript.FlashColor(Color.green));
                }
            }
            yield return new WaitForSeconds(flashTime + 0.2f);
        }
        // Set the buttons to a final color (e.g., grey)
        foreach (GameObject button in buttons)
        {
            SimonButton buttonScript = button.GetComponent<SimonButton>();
            if (buttonScript != null)
            {
                buttonScript.SetColor(Color.grey);
            }
        }
        // Once the puzzle is complete, move the keycard to the target location.
        MoveKeycard();
    }

    /// <summary>
    /// Moves the keycard GameObject to the designated target location.
    /// </summary>
    private void MoveKeycard()
    {
        if (keycard != null && keycardTarget != null)
        {
            keycard.transform.position = keycardTarget.position;
            keycard.transform.rotation = keycardTarget.rotation;
            Debug.Log("Keycard moved to target location.");
        }
        else
        {
            Debug.LogWarning("Keycard or keycardTarget is not assigned in the Inspector.");
        }
    }
}
