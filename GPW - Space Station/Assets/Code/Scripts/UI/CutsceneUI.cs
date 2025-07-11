using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class CutsceneUI : Singleton<CutsceneUI>
    {
        [SerializeField] private RectTransform _topBarTransform;
        [SerializeField] private RectTransform _bottomBarTransform;

        [Space(5)]
        [SerializeField] private float _defaultCutsceneBarSize;
        
        private Coroutine _smoothTransitionCoroutine;


        protected override void Awake()
        {
            base.Awake();
            HideSelf();
        }
        public void ShowCutsceneBars(float transitionDuration)
        {
            ShowSelf();

            StopSmoothMove();
            _smoothTransitionCoroutine = StartCoroutine(SmoothlyMoveHeight(0.0f, _defaultCutsceneBarSize, transitionDuration));
        }
        public void HideCutsceneBars(float transitionDuration)
        {
            if (IsHidden())
                return;

            StopSmoothMove();
            _smoothTransitionCoroutine = StartCoroutine(SmoothlyMoveHeight(_defaultCutsceneBarSize, 0.0f, transitionDuration, HideSelf));
        }

        private void StopSmoothMove()
        {
            if (_smoothTransitionCoroutine == null)
                return;
            
            StopCoroutine(_smoothTransitionCoroutine);
        }
        private IEnumerator SmoothlyMoveHeight(float initialSize, float targetSize, float transitionDuration, System.Action onCompleteCallback = null)
        {
            float lerpTime = 0.0f;
            while (lerpTime < 1.0f)
            {
                float currentHeight = Mathf.Lerp(initialSize, targetSize, lerpTime);
                _topBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, currentHeight);
                _bottomBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, currentHeight);

                yield return null;
                lerpTime += Time.deltaTime / transitionDuration;
            }

            _topBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, targetSize);
            _bottomBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, targetSize);

            onCompleteCallback?.Invoke();
        }


        private void ShowSelf() => gameObject.SetActive(true);
        private void HideSelf() => gameObject.SetActive(false);
        private bool IsHidden() => !gameObject.activeInHierarchy;
    }
}
