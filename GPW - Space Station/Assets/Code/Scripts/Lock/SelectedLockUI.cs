using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedLockUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] private Vector2[] _uiPositions;
    [SerializeField] private Lock _lock;

    [Space(5)]
    [SerializeField] private GameObject _root;


    #if UNITY_EDITOR

    [Space(5)]
    [SerializeField] private bool _drawGizmos;

    #endif


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _lock.OnSuccessfulInteraction += OnLockEnabled;
    }
    private void OnDestroy() => _lock.OnSuccessfulInteraction -= OnLockEnabled;
    
    private void Update()
    {
        if (!_lock.lockInteraction)
        {
            OnLockDisabled();
            return;
        }

        int uiPositionIndex = Mathf.Clamp(_lock.GetSelectedWheelIndex(), 0, _uiPositions.Length);
        _rectTransform.localPosition = new Vector3(_uiPositions[uiPositionIndex].x, _uiPositions[uiPositionIndex].y, _rectTransform.localPosition.z);
    }

    private void OnLockEnabled() => _root.SetActive(true);
    private void OnLockDisabled() => _root.SetActive(false);


#if UNITY_EDITOR

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
            Vector3 position = rectTransform.TransformPoint(_uiPositions[i]);
            position.z = transform.position.z;
            Gizmos.DrawSphere(position, 0.01f);
        }
    }

#endif
}
