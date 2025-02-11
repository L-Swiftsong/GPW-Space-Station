using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CameraFocusLook : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject focusLookTarget;

    public float lookStrength = 3f;
    public float resistanceStrength = 0.5f;

    [SerializeField] private bool isFocusLookActive;
    public float focusLookDuration;
    public float focusLookTimer;

	private void Start()
	{
		playerCamera = Camera.main;
        isFocusLookActive = false;
	}

	// Update is called once per frame
	void Update()
    {
		if (isFocusLookActive)
        {
            Debug.Log("Focus look is active, handling focus...");
            HandleFocusLook();
        }
    }

    private void HandleFocusLook()
    {
        focusLookTimer += Time.deltaTime;

        Vector3 directionToTarget = focusLookTarget.transform.position - playerCamera.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, Time.deltaTime * lookStrength);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector2 resistance = new Vector2(-mouseY, mouseX);
        playerCamera.transform.Rotate(resistance * resistanceStrength);

        if (focusLookTimer >= focusLookDuration)
        {
			isFocusLookActive = false;
        }
    }

    public static void TriggerFocusLookStatic(GameObject target, float duration = 3f, float strength = 3f) => Entities.Player.PlayerManager.Instance.Player.GetComponent<CameraFocusLook>().TriggerFocusLook(target, duration, strength);
    public void TriggerFocusLook(GameObject target, float duration = 3f, float strength = 3f)
    {
		focusLookTarget = target;
        Debug.Log(target);
		focusLookDuration = duration;
        lookStrength = strength;

        focusLookTimer = 0f;
        isFocusLookActive = true;
        Debug.Log(isFocusLookActive);
    }

    public bool IsFocusLookActive()
    {
        return isFocusLookActive;
    }
}
