using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndCreditsTestScript : MonoBehaviour
{
    [SerializeField] private CameraAnimation[] _cameraAnimations;
    [SerializeField] private int _currentAnimationPreview;
    [SerializeField, Range(0.0f, 1.0f)] private float _previewLerpValue;


    private void Update()
    {
        if (_cameraAnimations != null && _currentAnimationPreview < _cameraAnimations.Length)
        {
            PreviewAnimation(_cameraAnimations[_currentAnimationPreview]);
        }
    }
    private void PreviewAnimation(CameraAnimation cameraAnimation)
    {
        transform.position = Vector3.Lerp(cameraAnimation.StartPos, cameraAnimation.EndPos, _previewLerpValue);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(cameraAnimation.StartEulerAngles), Quaternion.Euler(cameraAnimation.EndEulerAngles), _previewLerpValue);
    }



    [System.Serializable]
    private struct CameraAnimation
    {
        public string Name;

        [Space(5)]
        public Vector3 StartPos;
        public Vector3 StartEulerAngles;

        [Space(5)]
        public Vector3 EndPos;
        public Vector3 EndEulerAngles;


        [Space(5)]
        public float Duration;
    }
}
