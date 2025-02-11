using System.Collections;
using UnityEngine;
using Interaction;

public class Lock : MonoBehaviour, IInteractable
{
    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    private Vector3 originPosition;
    private Quaternion originRotation;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool isMoving = false;
    private float moveSpeed = 3f;

    public bool lockInteraction = false;

    [Header("Code Settings")]
    [SerializeField] private int correctFirstDigit;
    [SerializeField] private int correctSecondDigit;
    [SerializeField] private int correctThirdDigit;
    [SerializeField] private int correctFourthDigit;

    private GameObject wheel1;
    private GameObject wheel2;
    private GameObject wheel3;
    private GameObject wheel4;

    public GameObject connectedDoor;

    void Start()
    {
        // stores locks orignial position so that it can return when not interacting
        originPosition = transform.position;
        originRotation = transform.rotation;

        // wheel references
        wheel1 = transform.Find("Wheel1").gameObject;
        wheel2 = transform.Find("Wheel2").gameObject;
        wheel3 = transform.Find("Wheel3").gameObject;
        wheel4 = transform.Find("Wheel4").gameObject;
    }

    void Update()
    {
        if (isMoving && lockInteraction)
        {
            // moves lock towards camera
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);

            // Stop moving when close enough to camera
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                isMoving = false;
            }
        }

        // movement keys stop interaction with lock
        if (lockInteraction)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                ResetLockPosition();
            }

            // checks if wheels on lock are being clicked and if so rotates them
            DetectWheelClick();
            DetectCompletion();
        }
    }

    public void Interact(PlayerInteraction player)
    {
        // gets position infront of camera for lock to move to
        SetTargetPosition();

        isMoving = true;
        lockInteraction = true;

        Cursor.lockState = CursorLockMode.Confined;

        PlayerInput.PreventAllActions(typeof(Lock));

        OnSuccessfulInteraction?.Invoke();
    }

    public void SetTargetPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // offsets lock infront of camera
            targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 0.75f;

            // little offsets as lock wasnt in centre of camera
            targetPosition += mainCamera.transform.right * 0.1f;
            targetPosition += mainCamera.transform.up * -0.1f;

            // lock faces camera
            targetRotation = Quaternion.LookRotation(-mainCamera.transform.forward);
        }
    }

    public void ResetLockPosition()
    {
        lockInteraction = false;

        transform.position = originPosition;
        transform.rotation = originRotation;

        Cursor.lockState = CursorLockMode.Locked;

        PlayerInput.RemoveAllActionPrevention(typeof(Lock));
    }

    private void DetectWheelClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Wheel"))
                {
                    RotateWheel(hit.transform);
                }
            }
        }
    }

    private void RotateWheel(Transform wheel)
    {
        wheel.Rotate(0, 36, 0);
    }

    // checks if wheel digits match the correct digits
    private void DetectCompletion()
    {
        if (wheel1.GetComponent<LockWheel>().wheelDigit == correctFirstDigit)
        {
            if (wheel2.GetComponent<LockWheel>().wheelDigit == correctSecondDigit)
            {
                if (wheel3.GetComponent<LockWheel>().wheelDigit == correctThirdDigit)
                {
                    if (wheel4.GetComponent<LockWheel>().wheelDigit == correctFourthDigit)
                    {
                        ResetLockPosition();

                        connectedDoor.GetComponent<LockerDoor>().ToggleDoor();

                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }
}
