using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private PlayerController playerController;

    [Header("Timers")]
    public float transitionTime = 0.5f;
    private float elapsedTime;

    private Coroutine _hidingCoroutine;


    [Header("Bools")]
    public bool isHiding = false;
    public bool isTransitioning = false;

    [Header("Offsets")]
    public float heightOffset = 1.25f;
    public float exitDistance = 2f;

    [Header("Scales")]
    public Vector3 hidingScale = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 originalScale;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        originalScale = transform.localScale;
    }


    public void StartHiding(Vector3 hidePosition)
    {
        // Stop the current hiding coroutine.
        if (_hidingCoroutine != null)
        {
            StopCoroutine(_hidingCoroutine);
        }

        // Start hiding.
        _hidingCoroutine = StartCoroutine(HideCoroutine(hidePosition));
    }
    public void StopHiding()
    {
        // Stop the current hiding coroutine.
        if (_hidingCoroutine != null)
        {
            StopCoroutine(_hidingCoroutine);
        }

        // Stop hiding.
        _hidingCoroutine = StartCoroutine(ExitHidingCoroutine());
    }



    public IEnumerator HideCoroutine(Vector3 hidePosition)
    {
        elapsedTime = 0f;

        isTransitioning = true;
        playerController.SetHiding(true); 
        transform.localScale = hidingScale;

        Vector3 startPosition = transform.position;
        //Vector3 endPosition = hidePosition.position + new Vector3(0, -heightOffset, 0);
        Vector3 endPosition = hidePosition;

        yield return SmoothMoveToPosition(startPosition, endPosition);

        transform.position = endPosition;
        isHiding = true;
        isTransitioning = false;
        Debug.Log("Hiding under the table.");
    }

    public IEnumerator ExitHidingCoroutine()
    {
        elapsedTime = 0f;

        isTransitioning = true;

        Vector3 startPosition = transform.position;
        Vector3 exitDirection = transform.forward;

        Debug.DrawRay(startPosition, exitDirection * exitDistance, Color.red, 2f);

        Vector3 endPosition = startPosition + exitDirection * exitDistance;

        transform. localScale = originalScale;

        yield return SmoothMoveToPosition(startPosition, endPosition);

        transform.position = endPosition;

        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
        }

        isHiding = false;
        isTransitioning = false;
        playerController.SetHiding(false); 
        Debug.Log("Exited hiding.");
    }

    private IEnumerator SmoothMoveToPosition(Vector3 start, Vector3 end)
    {
        while (elapsedTime < transitionTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }

}
