using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chase;
using Environment.Doors;

public class CloseDoorOnMimic : MonoBehaviour
{
    private ExternalInputDoor externalInputDoor;
    private ChaseEndTrigger chaseEndTrigger;

    [SerializeField] private GameObject _door;
    private GameObject _mimic;

    private bool _chaseEnded = false;

    void Start()
    {
        // Script References
        externalInputDoor = _door.GetComponent<ExternalInputDoor>();

        _mimic = GameObject.Find("ChaseMimic");
    }

    void Update()
    {
        if (_chaseEnded)
        {
            externalInputDoor.Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _mimic)
        {
            EndChase();
            StartCoroutine(DestroyMimicAfterDoorClosed());
        }
    }

    private void EndChase()
    {
        _chaseEnded = true;
    }

    IEnumerator DestroyMimicAfterDoorClosed()
    {
        yield return new WaitForSeconds(1f);
        Destroy(_mimic);
    }
}
