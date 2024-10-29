using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private Transform _targetObject;
    [SerializeField] private Camera _renderCamera;

    private void Update()
    {
        Vector2 cutoutPosition = _renderCamera.WorldToViewportPoint(_targetObject.position);
        cutoutPosition.y /= (Screen.width / Screen.height);

        foreach(Transform child in transform)
        {
            foreach(Material material in child.GetComponent<Renderer>().materials)
            {
                material.SetVector("_Position", cutoutPosition);
            }
        }
    }
}
