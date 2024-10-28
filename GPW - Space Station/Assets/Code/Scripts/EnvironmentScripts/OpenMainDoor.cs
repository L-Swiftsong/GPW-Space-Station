using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMainDoor : MonoBehaviour
{
    [SerializeField]
    private float openHeight = 5f;

    [SerializeField]
    private float openSpeed = 2f;

    [SerializeField]
    private AudioClip openSound;

    [SerializeField]
    private AudioClip closeSound;

    [SerializeField]
    private float triggerDistance = 3f;

    [SerializeField]
    private float closeDelay = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private bool isClosing = false;
    private AudioSource audioSource;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = new Vector3(closedPosition.x, closedPosition.y + openHeight, closedPosition.z);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(PlayerManager.Instance.Player.position, transform.position);

        if (distanceToPlayer <= triggerDistance && !isOpening && !isClosing)
        {
            StartCoroutine(OpenDoor());
        }
        else if (distanceToPlayer > triggerDistance && !isClosing && !isOpening)
        {
            StartCoroutine(CloseDoorAfterDelay());
        }
    }

    IEnumerator OpenDoor()
    {
        isOpening = true;
        isClosing = false;

        if (openSound != null && transform.position != openPosition)
        {
            audioSource.clip = openSound;
            audioSource.Play();
        }

        while (transform.position.y < openPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, openSpeed * Time.deltaTime);
            yield return null;
        }

        isOpening = false;
    }

    IEnumerator CloseDoorAfterDelay()
    {
        isClosing = true;

        yield return new WaitForSeconds(closeDelay);

        if (closeSound != null && transform.position != closedPosition)
        {
            audioSource.clip = closeSound;
            audioSource.Play();
        }

        while (transform.position.y > closedPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, openSpeed * Time.deltaTime);
            yield return null;
        }

        isClosing = false;
    }
}
