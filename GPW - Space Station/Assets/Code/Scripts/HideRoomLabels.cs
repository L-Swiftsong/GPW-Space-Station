using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRoomLabels : MonoBehaviour
{
    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
