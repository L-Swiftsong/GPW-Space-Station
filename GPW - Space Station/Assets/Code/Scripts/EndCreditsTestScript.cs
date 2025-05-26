using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;


public class EndCreditsTestScript : MonoBehaviour
{
    [SerializeField] private bool _previewFullAnimation;
    private Coroutine _fullPreviewCoroutine;
    private bool _isPlayingCutscene;

    [Space(5)]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    private const float FADE_IN_DURATION = 0.25f;

    [Space(5)]
    [SerializeField] private CameraAnimation[] _cameraAnimations;
    [SerializeField] private int _currentAnimationPreview;
    [SerializeField, Range(0.0f, 1.0f)] private float _previewLerpValue;


    private void Update()
    {
        if (_isPlayingCutscene)
        {
            if (_previewFullAnimation)
            {
                _previewFullAnimation = false;

                if (_fullPreviewCoroutine != null)
                    StopCoroutine(_fullPreviewCoroutine);
            }

            return;
        }


        if (_cameraAnimations != null && _currentAnimationPreview < _cameraAnimations.Length)
        {
            PreviewAnimation(_cameraAnimations[_currentAnimationPreview]);
        }


        if (_previewFullAnimation)
        {
            _previewFullAnimation = false;

            if (_fullPreviewCoroutine != null)
                StopCoroutine(_fullPreviewCoroutine);
            _fullPreviewCoroutine = StartCoroutine(PreviewFullAnimation());
        }
    }
    private void PreviewAnimation(CameraAnimation cameraAnimation)
    {
        transform.position = Vector3.Lerp(cameraAnimation.StartPos, cameraAnimation.EndPos, _previewLerpValue);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(cameraAnimation.StartEulerAngles), Quaternion.Euler(cameraAnimation.EndEulerAngles), _previewLerpValue);
    }
    private IEnumerator PreviewFullAnimation()
    {
        if (CutsceneUI.HasInstance)
        {
            CutsceneUI.Instance.ShowCutsceneBars(0.0f);
        }


        float lerpRate, lerpTime, fadeInEndTime, fadeStartTime, fadeEndTime;
        foreach(CameraAnimation animation in _cameraAnimations)
        {
            // Calculate lerp time & fade times.
            lerpRate = 1.0f / animation.Duration;
            
            fadeInEndTime = _fadeCanvasGroup.alpha != 0.0f ? lerpRate * FADE_IN_DURATION : 0.0f;
            
            fadeStartTime = 1.0f - (lerpRate * (animation.FadeDuration + animation.FadeCompleteDuration));
            fadeEndTime = 1.0f - (lerpRate * animation.FadeCompleteDuration);


            // Handle the camera movement & rotation.
            lerpTime = 0.0f;
            while(lerpTime < 1.0f)
            {
                // Position & Rotation Changes.
                transform.position = Vector3.Lerp(animation.StartPos, animation.EndPos, lerpTime);
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(animation.StartEulerAngles), Quaternion.Euler(animation.EndEulerAngles), lerpTime);

                // Facing.
                if (lerpTime < fadeInEndTime)
                {
                    // Fade in at the start of the camera movement.
                    float fadeLerpTime = Mathf.InverseLerp(0.0f, fadeInEndTime, lerpTime);
                    _fadeCanvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, fadeLerpTime);
                }
                else if (lerpTime > fadeStartTime)
                {
                    // Fade out at the end of the camera movement.
                    float fadeLerpTime = Mathf.InverseLerp(fadeStartTime, fadeEndTime, lerpTime);
                    _fadeCanvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, fadeLerpTime);
                }
                else if (_fadeCanvasGroup.alpha != 0.0f)
                {
                    // Don't be faded when not near the start/end of the movement.
                    _fadeCanvasGroup.alpha = 0.0f;
                }

                // Update rate.
                yield return null;
                lerpTime += lerpRate * Time.deltaTime;
            }

            // Ensure that values have been properly set.
            transform.position = animation.EndPos;
            transform.rotation = Quaternion.Euler(animation.EndEulerAngles);
            if (animation.FadeDuration > 0.0f)
                _fadeCanvasGroup.alpha = 1.0f;
        }
    }



    [System.Serializable]
    private class CameraAnimation
    {
        public string Name;

        [Header("Camera Positions")]
        public Vector3 StartPos;
        public Vector3 StartEulerAngles;

        [Space(5)]
        public Vector3 EndPos;
        public Vector3 EndEulerAngles;


        [Header("Testing")]
        public float Duration = 3.5f;

        public float FadeDuration = 0.5f;
        public float FadeCompleteDuration = 0.25f;
    }
}
