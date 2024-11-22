using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Animations
{
    [RequireComponent(typeof(RectTransform))]
    public class UIElementTranslation : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;


        [Header("Forward Transition Settings")]
        [SerializeField] private float _animationDuration;
        [SerializeField] private AnimationCurve _positionTransitionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        [Space(5)]
        [SerializeField] private Vector2 _startPosition;
        [SerializeField] private Vector2 _endPosition;

        [Space(5)]
        [SerializeField] private float _startAlpha = 0.0f;
        [SerializeField] private float _endAlpha = 1.0f;
        [SerializeField] private AnimationCurve _alphaCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);


        private void Awake()
        {
            // Get references.
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void StartAnimation()
        {
            _rectTransform.localPosition = _startPosition;
            StartCoroutine(TranslatePosition());
        }
        public void StartReverseAnimation()
        {
            _rectTransform.localPosition = _endPosition;
            StartCoroutine(TranslatePositionReversed());
        }
        private IEnumerator TranslatePosition()
        {
            bool lerpAlpha = _canvasGroup != null;

            float lerpTime = 0.0f;
            while(lerpTime < 1.0f)
            {
                // Lerp the position from the start to the end using our positionTransitionCurve & lerpTime.
                _rectTransform.localPosition = Vector2.Lerp(_startPosition, _endPosition, _positionTransitionCurve.Evaluate(lerpTime));

                // Lerp the alpha.
                if (lerpAlpha)
                {
                    _canvasGroup.alpha = Mathf.Lerp(_startAlpha, _endAlpha, _alphaCurve.Evaluate(lerpTime));
                }

                lerpTime += Time.deltaTime / _animationDuration;

                yield return null;
            }

            // Ensure we reach our desired values.
            transform.localPosition = _endPosition;
            if (lerpAlpha)
                _canvasGroup.alpha = _endAlpha;
        }
        private IEnumerator TranslatePositionReversed()
        {
            bool lerpAlpha = _canvasGroup != null;

            float lerpTime = 0.0f;
            while (lerpTime < 1.0f)
            {
                // Lerp the position from the end to the start using our position transition curve and the inverse of our lerp time.
                _rectTransform.localPosition = Vector2.Lerp(_startPosition, _endPosition, _positionTransitionCurve.Evaluate(1.0f - lerpTime));

                // Lerp the alpha.
                if (lerpAlpha)
                {
                    _canvasGroup.alpha = Mathf.Lerp(_startAlpha, _endAlpha, _alphaCurve.Evaluate(1.0f - lerpTime));
                }

                lerpTime += Time.deltaTime / _animationDuration;

                yield return null;
            }

            // Ensure we reach our desired values.
            transform.localPosition = _startPosition;
            if (lerpAlpha)
                _canvasGroup.alpha = _endAlpha;
        }


        public float GetDuration() => _animationDuration;
    }
}