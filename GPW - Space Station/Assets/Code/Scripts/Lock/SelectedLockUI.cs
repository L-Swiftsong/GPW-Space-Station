using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedLockUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] private Vector2[] _uiPositions;
    [SerializeField] private Lock _lock;


    #if UNITY_EDITOR

    [Space(5)]
    [SerializeField] private bool _drawGizmos;

    #endif


    private void Awake() => _rectTransform = GetComponent<RectTransform>();
    private void Update()
    {
        int uiPositionIndex = Mathf.Clamp(_lock.GetSelectedWheelIndex(), 0, _uiPositions.Length);
        _rectTransform.localPosition = _uiPositions[uiPositionIndex];
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
        {
            return;
        }

        RectTransform rectTransform = transform.parent.GetComponent<RectTransform>();

        Gizmos.color = Color.green;
        for (int i = 0; i < _uiPositions.Length; ++i)
        {
            Gizmos.DrawSphere(rectTransform.TransformPoint(_uiPositions[i]), 0.01f);
        }
    }
}
