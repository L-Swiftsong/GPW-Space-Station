using UnityEngine;
using Interaction;
using Entities.Player;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraSystem : MonoBehaviour, IInteractable
{
    [Header("Security Cameras")]
    [SerializeField] private Camera[] stationCameras;

    [Header("Camera UI")]
    [SerializeField] private GameObject cameraUI;
    [SerializeField] private TextMeshProUGUI cameraLabel;

    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInput playerInput; 

    [Header("Per-Camera Clamp Settings")]
    [SerializeField] private float[] yawMinPerCamera;
    [SerializeField] private float[] yawMaxPerCamera;
    [SerializeField] private float[] pitchMinPerCamera;
    [SerializeField] private float[] pitchMaxPerCamera;
    [SerializeField] private float[] initialPitchOffsetPerCamera;

    [Header("Default Clamp Settings (Fallback)")]
    [SerializeField] private float yawMin = -30f;
    [SerializeField] private float yawMax = 30f;
    [SerializeField] private float pitchMin = -50f;
    [SerializeField] private float pitchMax = 50f;
    [SerializeField] private float initialPitchOffset = -15f;

    [Header("Zoom Settings")]
    [SerializeField] private float minFOV = 20f;
    [SerializeField] private float maxFOV = 60f;
    [SerializeField] private float zoomStep = 5f;

    [Header("Rec Indicator")]
    [SerializeField] private TextMeshProUGUI recIndicator;


    private bool isViewingCameras = false;
    private PlayerInteraction currentInteractingScript;
    private int activeCameraIndex = 0;
    private float cameraPitch = 0f;
    private float cameraYaw = 0f;
    private float[] originalFOVs;

    //stop the "REC" indicator animation when closing the camera UI.
    private Coroutine recIndicatorCoroutine;


    #region IInteractable Properties & Events

    private int _previousLayer;

    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    #endregion



    private void Awake()
    {
        if (stationCameras != null && stationCameras.Length > 0)
        {
            originalFOVs = new float[stationCameras.Length];
            for (int i = 0; i < stationCameras.Length; i++)
            {
                if (stationCameras[i] != null)
                {
                    originalFOVs[i] = stationCameras[i].fieldOfView;
                    stationCameras[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        if (isViewingCameras)
        {
            HandleKeyboardInput();
            HandleCameraRotation();
        }
    }

    public void Interact(PlayerInteraction interactingScript)
    {
        if (!isViewingCameras)
        {
            isViewingCameras = true;
            currentInteractingScript = interactingScript;
            OnSuccessfulInteraction?.Invoke();

            PlayerInput.PreventAllActions(typeof(CameraSystem));

            interactingScript.SetCurrentInteractableOverride(this);
            OpenCameraView();
        }
        else
        {
            CloseCameraView(currentInteractingScript);
        }
    }
    public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
    public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);


    private void OpenCameraView()
    {
        if (cameraUI != null)
            cameraUI.SetActive(true);

        activeCameraIndex = 0;
        cameraPitch = (initialPitchOffsetPerCamera != null && initialPitchOffsetPerCamera.Length > activeCameraIndex)
            ? initialPitchOffsetPerCamera[activeCameraIndex]
            : initialPitchOffset;
        cameraYaw = 0f;

        ShowCameraFeed(activeCameraIndex);

        // Start the rec indicator animation
        if (recIndicator != null)
        {
            recIndicatorCoroutine = StartCoroutine(AnimateRecIndicator());
        }
    }

    private void CloseCameraView(PlayerInteraction interactingScript)
    {
        if (cameraUI != null)
            cameraUI.SetActive(false);

        if (stationCameras != null)
        {
            for (int i = 0; i < stationCameras.Length; i++)
            {
                if (stationCameras[i] != null)
                {
                    stationCameras[i].gameObject.SetActive(false);
                    stationCameras[i].fieldOfView = originalFOVs[i];
                }
            }
        }

        isViewingCameras = false;
        currentInteractingScript = null;

        // Remove input prevention:
        PlayerInput.RemoveAllActionPrevention(typeof(CameraSystem));

        interactingScript.ResetCurrentInteractableOverride();

        // Stop the rec indicator animation
        if (recIndicatorCoroutine != null)
        {
            StopCoroutine(recIndicatorCoroutine);
            recIndicatorCoroutine = null;
        }
        // Clear the text
        if (recIndicator != null)
            recIndicator.text = "";
    }

    private void ShowCameraFeed(int index)
    {
        if (stationCameras == null || stationCameras.Length == 0)
            return;

        for (int i = 0; i < stationCameras.Length; i++)
        {
            if (stationCameras[i] != null)
                stationCameras[i].gameObject.SetActive(false);
        }

        if (stationCameras[index] != null)
            stationCameras[index].gameObject.SetActive(true);

        UpdateCameraLabel();
    }

    private void UpdateCameraLabel()
    {
        if (cameraLabel != null)
            cameraLabel.text = "Camera " + (activeCameraIndex + 1);
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SwitchCamera(1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            SwitchCamera(-1);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            ZoomIn();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            ZoomOut();

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseCameraView(currentInteractingScript);
    }

    private void SwitchCamera(int direction)
    {
        if (stationCameras == null || stationCameras.Length == 0)
            return;

        activeCameraIndex = (activeCameraIndex + direction + stationCameras.Length) % stationCameras.Length;
        cameraPitch = (initialPitchOffsetPerCamera != null && initialPitchOffsetPerCamera.Length > activeCameraIndex)
            ? initialPitchOffsetPerCamera[activeCameraIndex]
            : initialPitchOffset;
        cameraYaw = 0f;
        ShowCameraFeed(activeCameraIndex);
    }

    private void HandleCameraRotation()
    {
        if (stationCameras == null || stationCameras.Length == 0)
            return;

        Camera currentCam = stationCameras[activeCameraIndex];
        if (currentCam == null)
            return;

        Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))
                            * Time.deltaTime * 250f;

        cameraPitch -= lookInput.y;
        cameraYaw += lookInput.x;

        float currentPitchMin = (pitchMinPerCamera != null && pitchMinPerCamera.Length > activeCameraIndex)
                                ? pitchMinPerCamera[activeCameraIndex]
                                : pitchMin;
        float currentPitchMax = (pitchMaxPerCamera != null && pitchMaxPerCamera.Length > activeCameraIndex)
                                ? pitchMaxPerCamera[activeCameraIndex]
                                : pitchMax;
        float currentYawMin = (yawMinPerCamera != null && yawMinPerCamera.Length > activeCameraIndex)
                              ? yawMinPerCamera[activeCameraIndex]
                              : yawMin;
        float currentYawMax = (yawMaxPerCamera != null && yawMaxPerCamera.Length > activeCameraIndex)
                              ? yawMaxPerCamera[activeCameraIndex]
                              : yawMax;

        cameraPitch = Mathf.Clamp(cameraPitch, currentPitchMin, currentPitchMax);
        cameraYaw = Mathf.Clamp(cameraYaw, currentYawMin, currentYawMax);

        currentCam.transform.localRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }

    public void ZoomIn()
    {
        if (stationCameras == null || stationCameras.Length == 0)
            return;

        Camera currentCam = stationCameras[activeCameraIndex];
        if (currentCam != null)
        {
            currentCam.fieldOfView = Mathf.Max(currentCam.fieldOfView - zoomStep, minFOV);
        }
    }

    public void ZoomOut()
    {
        if (stationCameras == null || stationCameras.Length == 0)
            return;

        Camera currentCam = stationCameras[activeCameraIndex];
        if (currentCam != null)
        {
            currentCam.fieldOfView = Mathf.Min(currentCam.fieldOfView + zoomStep, maxFOV);
        }
    }

    // =====================================================
    // Animate the "REC" UI indicator . .. ...
    private IEnumerator AnimateRecIndicator()
    {
        while (isViewingCameras)
        {
            if (recIndicator != null)
            {
                recIndicator.text = "REC.";
                yield return new WaitForSecondsRealtime(0.5f);

                recIndicator.text = "REC..";
                yield return new WaitForSecondsRealtime(0.5f);

                recIndicator.text = "REC...";
                yield return new WaitForSecondsRealtime(0.5f);
            }
            // Then loop back to "REC." again
        }

        if (recIndicator != null)
            recIndicator.text = "";
    }
    // =====================================================
}
