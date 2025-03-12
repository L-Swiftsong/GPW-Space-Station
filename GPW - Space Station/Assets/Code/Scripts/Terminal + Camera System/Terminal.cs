using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Interaction;
using Entities.Player;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

[System.Serializable]
public class FileStory
{
    public string title;
    [TextArea(3, 10)]
    public string content;
}

public class Terminal : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    [SerializeField] private GameObject terminalUI;
    [SerializeField] private TextMeshProUGUI terminalText;

    [Header("Input Field ++ Keyboard")]
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject virtualKeyboardPanel;
    [SerializeField] private Button defaultVirtualKeyboardButton; // For controller navigation

    [Header("Main Options")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button unlockDoorButton;
    [SerializeField] private Button shipCameraButton;
    [SerializeField] private Button readFilesButton;
    [SerializeField] private Button lockDoorButton;
    [SerializeField] private Button exitTerminalButton;

    [Header("Files UI")]
    [SerializeField] private GameObject filesPanel;
    [SerializeField] private Button file1Button;
    [SerializeField] private Button file2Button;
    [SerializeField] private Button file3Button;
    [SerializeField] private Button returnButton;

    [Header("References to Player Movement")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInput playerInput;

    [Header("Reference to Camera Terminal")]
    [SerializeField] private CameraSystem cameraTerminal;

    [Header("Terminal Camera")]
    [Tooltip("Camera to activate while using the terminal.")]
    [SerializeField] private Camera terminalCamera;

    [Header("ID Settings")]
    [SerializeField] private string correctPassword = "NCT0094";
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private float lockoutDuration = 10f;

    [Header("UI Settings")]
    [Tooltip("Typing speed between each character in typed lines. makes it look more real")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float inputFontSize = 7f;

    [Header("Files Settings")]
    [SerializeField] private FileStory[] files;


    #region IInteractable Properties & Events

    private int _previousLayer;

    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    #endregion


    private bool _isUsingTerminal = false;
    private PlayerInteraction _currentInteractingScript;
    private int attemptsLeft;
    private bool _lockedOut = false;

    // Updated prefix
    private const string prefix = "User ID> ";

    // Once authenticated, skip password on subsequent uses.
    private bool hasAuthenticated = false;

    // For preventing rapid virtual keyboard input.
    private bool isVirtualKeyProcessing = false;
    private IEnumerator ResetVirtualKeyProcessing()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        isVirtualKeyProcessing = false;
    }

    // Prevent door sequences from running multiple times
    private bool isDoorSequenceRunning = false;

    // Renamed the field to _isTyping to avoid ambiguity
    private bool _isTyping = false;

    private void Start()
    {
        if (terminalUI != null)
            terminalUI.SetActive(false);
        if (virtualKeyboardPanel != null)
            virtualKeyboardPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (filesPanel != null)
            filesPanel.SetActive(false);

        if (terminalText != null)
        {
            terminalText.text = "";
            terminalText.richText = true;
        }

        if (passwordInputField != null)
        {
            ConfigurePasswordInputField();
        }

        attemptsLeft = maxAttempts;

        if (unlockDoorButton != null)
            unlockDoorButton.onClick.AddListener(OnUnlockDoorClicked);
        if (shipCameraButton != null)
            shipCameraButton.onClick.AddListener(OnShipCameraClicked);
        if (readFilesButton != null)
            readFilesButton.onClick.AddListener(OnReadFilesClicked);

        if (lockDoorButton != null)
        {
            lockDoorButton.gameObject.SetActive(false);
            lockDoorButton.onClick.AddListener(OnLockDoorClicked);
        }
        if (exitTerminalButton != null)
            exitTerminalButton.onClick.AddListener(OnExitTerminalClicked);

        if (file1Button != null)
            file1Button.onClick.AddListener(() => StartCoroutine(OnFileClicked(0)));
        if (file2Button != null)
            file2Button.onClick.AddListener(() => StartCoroutine(OnFileClicked(1)));
        if (file3Button != null)
            file3Button.onClick.AddListener(() => StartCoroutine(OnFileClicked(2)));
        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnButtonClicked);

        if (terminalCamera != null)
            terminalCamera.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (_isUsingTerminal)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (_isUsingTerminal && Gamepad.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null && defaultVirtualKeyboardButton != null)
            {
                EventSystem.current.SetSelectedGameObject(defaultVirtualKeyboardButton.gameObject);
            }
        }
    }

    private void ConfigurePasswordInputField()
    {
        if (passwordInputField.textComponent != null)
        {
            passwordInputField.textComponent.fontSize = inputFontSize;
            passwordInputField.textComponent.color = Color.green;
        }
        if (passwordInputField.placeholder is TextMeshProUGUI placeholderTMP)
        {
            placeholderTMP.fontSize = inputFontSize;
            placeholderTMP.color = Color.green;
        }
        Navigation nav = passwordInputField.navigation;
        nav.mode = Navigation.Mode.None;
        passwordInputField.navigation = nav;
        passwordInputField.readOnly = true;
        passwordInputField.text = prefix;
        passwordInputField.caretPosition = passwordInputField.text.Length;
        passwordInputField.selectionAnchorPosition = passwordInputField.text.Length;
        passwordInputField.selectionFocusPosition = passwordInputField.text.Length;
        passwordInputField.selectionColor = new Color(0, 0, 0, 0);
        passwordInputField.caretColor = Color.green;
    }

    // ================================
    // IInteractable 
    // ================================
    public void Interact(PlayerInteraction interactingScript)
    {
        if (!_isUsingTerminal)
        {
            _isUsingTerminal = true;
            _currentInteractingScript = interactingScript;
            OnSuccessfulInteraction?.Invoke();

            // Prevent all actions s 
            PlayerInput.PreventAllActions(typeof(Terminal), disableGlobalMaps: true);

            interactingScript.SetCurrentInteractableOverride(this);
            OpenTerminalUI();
        }
    }
    public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
    public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);


    private void OpenTerminalUI()
    {
        if (terminalUI != null)
            terminalUI.SetActive(true);

        if (terminalCamera != null)
            terminalCamera.gameObject.SetActive(true);

        // Subscribe to input events.
        PlayerInput.OnUICancelPerformed += CloseTerminal;

        if (hasAuthenticated)
        {
            if (passwordInputField != null)
                passwordInputField.gameObject.SetActive(false);
            if (virtualKeyboardPanel != null)
                virtualKeyboardPanel.SetActive(false);
            if (terminalText != null)
                terminalText.text = "Choose an option below:";
            if (optionsPanel != null)
                optionsPanel.SetActive(true);
            return;
        }

        if (passwordInputField != null)
        {
            passwordInputField.gameObject.SetActive(true);
            if (!passwordInputField.text.StartsWith(prefix))
                passwordInputField.text = prefix;
        }
        if (virtualKeyboardPanel != null)
            virtualKeyboardPanel.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (Gamepad.current != null && defaultVirtualKeyboardButton != null)
            EventSystem.current.SetSelectedGameObject(defaultVirtualKeyboardButton.gameObject);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (terminalText != null)
            terminalText.text = "";
        StartCoroutine(TypeIntroLines());
    }

    private IEnumerator TypeIntroLines()
    {
        yield return StartCoroutine(TypeLine("SPACE STATION-023 (TM) CONSOLE"));
        yield return StartCoroutine(TypeLine("PLEASE ENTER USER ID"));
    }

    private void OnInputSubmitted(string typedText)
    {
        if (_lockedOut)
        {
            StartCoroutine(TypeLine("You are currently locked out. Please wait."));
            ClearInputFieldButKeepPrefix();
            return;
        }

        string actualInput = RemovePrefix(typedText).Trim().ToUpper();
        if (actualInput == "EXIT")
        {
            CloseTerminal();
            return;
        }

        if (actualInput == correctPassword)
        {
            StartCoroutine(IDVerificationSequence());
        }
        else
        {
            attemptsLeft--;
            if (attemptsLeft > 0)
            {
                StartCoroutine(TypeLine($"INCORRECT ID. {attemptsLeft} attempt(s) left."));
            }
            else
            {
                StartCoroutine(TypeLine($"NO ATTEMPTS LEFT! LOCKING OUT FOR {lockoutDuration}s..."));
                StartCoroutine(LockoutCoroutine(lockoutDuration));
            }
        }
        ClearInputFieldButKeepPrefix();
    }

    private IEnumerator IDVerificationSequence()
    {
        if (terminalText != null)
            terminalText.text = "";
        if (passwordInputField != null)
            passwordInputField.gameObject.SetActive(false);
        if (virtualKeyboardPanel != null)
            virtualKeyboardPanel.SetActive(false);

        yield return StartCoroutine(TypeLine("ID Verification in progress."));
        yield return StartCoroutine(TypeLine("Please wait"));
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(TypeLine("."));
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(TypeLine("."));
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(TypeLine("."));
        yield return new WaitForSecondsRealtime(0.3f);
        yield return StartCoroutine(TypeLine("ID Verification has been confirmed."));
        yield return new WaitForSecondsRealtime(1f);

        if (terminalText != null)
            terminalText.text = "Welcome back, please choose an option below:";
        if (optionsPanel != null)
            optionsPanel.SetActive(true);

        hasAuthenticated = true;
    }

    private void ClearInputFieldButKeepPrefix()
    {
        if (passwordInputField == null)
            return;
        passwordInputField.text = prefix;
        passwordInputField.Select();
        passwordInputField.ActivateInputField();
    }

    private string RemovePrefix(string fullText)
    {
        if (fullText.StartsWith(prefix))
            return fullText.Substring(prefix.Length);
        return fullText;
    }

    private IEnumerator LockoutCoroutine(float duration)
    {
        _lockedOut = true;
        if (passwordInputField != null)
            passwordInputField.interactable = false;
        yield return new WaitForSecondsRealtime(duration);
        attemptsLeft = maxAttempts;
        _lockedOut = false;
        if (passwordInputField != null)
            passwordInputField.interactable = true;
        StartCoroutine(TypeLine($"Lockout ended. You have {attemptsLeft} attempts again."));
    }

    private void ClearInputField()
    {
        if (passwordInputField != null)
        {
            passwordInputField.text = prefix;
            passwordInputField.Select();
            passwordInputField.ActivateInputField();
        }
    }

    // Type out text using unscaled time. Blocks UI via CanvasGroup.
    private IEnumerator TypeLine(string line, bool reEnableUIAfter = false)
    {
        if (terminalText == null) yield break;

        // Mark that we are typing
        _isTyping = true;

        string finalLine = line + "\n";
        for (int i = 0; i < finalLine.Length; i++)
        {
            terminalText.text += finalLine[i];
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        // Finished typing
        _isTyping = false;

    }

    // ===== Virtual Keyboard =====
    public void OnCharacterButtonClicked(string character)
    {
        if (isVirtualKeyProcessing || _isTyping) return;
        isVirtualKeyProcessing = true;
        string currentText = passwordInputField.text;
        if (!currentText.StartsWith(prefix))
            currentText = prefix;
        currentText += character;
        passwordInputField.text = currentText;
        passwordInputField.Select();
        passwordInputField.ActivateInputField();
        StartCoroutine(ResetVirtualKeyProcessing());
    }

    public void OnBackspaceButtonClicked()
    {
        if (isVirtualKeyProcessing || _isTyping) return;
        isVirtualKeyProcessing = true;
        string currentText = passwordInputField.text;
        if (currentText.Length > prefix.Length)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            passwordInputField.text = currentText;
            passwordInputField.Select();
            passwordInputField.ActivateInputField();
        }
        StartCoroutine(ResetVirtualKeyProcessing());
    }

    public void OnSubmitButtonClicked()
    {
        if (isVirtualKeyProcessing || _isTyping) return;
        isVirtualKeyProcessing = true;
        OnInputSubmitted(passwordInputField.text);
        StartCoroutine(ResetVirtualKeyProcessing());
    }

    public void OnEscButtonClicked()
    {
        if (isVirtualKeyProcessing || _isTyping) return;
        isVirtualKeyProcessing = true;
        OnInputSubmitted("EXIT");
        StartCoroutine(ResetVirtualKeyProcessing());
    }

    // ===== Terminal Menu UI: Door, Camera, Files, Exit etc =====
    public void OnUnlockDoorClicked()
    {
        // If we are typing, do nothing
        if (_isTyping) return;

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (!isDoorSequenceRunning)
            StartCoroutine(UnlockDoorSequence());
    }

    private IEnumerator UnlockDoorSequence()
    {
        isDoorSequenceRunning = true;
        if (terminalText != null)
            terminalText.text = "";

        yield return StartCoroutine(TypeLine("Unlocking Door, please stand by..."));
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(TypeLine("Door unlocked."));

        if (unlockDoorButton != null)
            unlockDoorButton.gameObject.SetActive(false);
        if (lockDoorButton != null)
            lockDoorButton.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
        if (terminalText != null)
            terminalText.text = "Choose an option below:";

        isDoorSequenceRunning = false;
    }

    public void OnLockDoorClicked()
    {
        if (_isTyping) return;

        if (lockDoorButton != null)
            lockDoorButton.gameObject.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (!isDoorSequenceRunning)
            StartCoroutine(LockDoorSequence());
    }

    private IEnumerator LockDoorSequence()
    {
        isDoorSequenceRunning = true;
        if (terminalText != null)
            terminalText.text = "";

        yield return StartCoroutine(TypeLine("Locking Door, please stand by..."));
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(TypeLine("Door locked."));

        if (lockDoorButton != null)
            lockDoorButton.gameObject.SetActive(false);
        if (unlockDoorButton != null)
            unlockDoorButton.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
        if (terminalText != null)
            terminalText.text = "Choose an option below:";

        isDoorSequenceRunning = false;
    }

    public void OnShipCameraClicked()
    {
        if (_isTyping) return;

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (terminalText != null)
            terminalText.text = "";
        StartCoroutine(ShipCameraSequence());
    }

    private IEnumerator ShipCameraSequence()
    {

        yield return StartCoroutine(TypeLine("Accessing camera feed..."));
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(TypeLine("Camera feed activated."));

        if (cameraTerminal != null)
        {
            cameraTerminal.Interact(_currentInteractingScript);
        }
        CloseTerminal();
    }

    public void OnReadFilesClicked()
    {
        if (_isTyping) return;

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (terminalText != null)
            terminalText.text = "";
        if (filesPanel != null)
            filesPanel.SetActive(true);
    }

    public IEnumerator OnFileClicked(int index)
    {
        if (_isTyping) yield break;

        if (file1Button != null) file1Button.gameObject.SetActive(false);
        if (file2Button != null) file2Button.gameObject.SetActive(false);
        if (file3Button != null) file3Button.gameObject.SetActive(false);

        // Also hide the Return button until the file text has finished.
        if (returnButton != null) returnButton.gameObject.SetActive(false);

        if (terminalText != null)
            terminalText.text = "";

        if (files != null && index >= 0 && index < files.Length)
        {
            yield return StartCoroutine(TypeLine(files[index].title));
            yield return new WaitForSecondsRealtime(0.3f);
            yield return StartCoroutine(TypeLine(files[index].content));
        }

        // Enable Return button once done
        if (returnButton != null) returnButton.gameObject.SetActive(true);
    }

    public void OnReturnButtonClicked()
    {
        if (_isTyping) return;

        if (terminalText != null)
            terminalText.text = "";
        if (filesPanel != null)
            filesPanel.SetActive(false);

        if (file1Button != null) file1Button.gameObject.SetActive(true);
        if (file2Button != null) file2Button.gameObject.SetActive(true);
        if (file3Button != null) file3Button.gameObject.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
        if (terminalText != null)
            terminalText.text = "Welcome back, please choose an option below:";
    }

    public void OnExitTerminalClicked()
    {
        if (_isTyping) return;
        CloseTerminal();
    }

    public void CloseTerminal()
    {
        if (terminalUI != null)
            terminalUI.SetActive(false);
        _isUsingTerminal = false;

        if (terminalCamera != null)
            terminalCamera.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Unsubscribe from Input Events.
        PlayerInput.OnUICancelPerformed -= CloseTerminal;


        _currentInteractingScript?.ResetCurrentInteractableOverride();
        _currentInteractingScript = null;

        PlayerInput.RemoveAllActionPrevention(typeof(Terminal));
    }
}
