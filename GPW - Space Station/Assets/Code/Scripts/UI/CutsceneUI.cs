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


        protected override void Awake()
        {
            base.Awake();
            HideSelf();
        }
        public void ShowCutsceneBars(float transitionDuration)
        {
            ShowSelf();
            SmoothlyMoveHeight(_defaultCutsceneBarSize, transitionDuration);
        }
        public void HideCutsceneBars(float transitionDuration)
        {
            SmoothlyMoveHeight(0.0f, transitionDuration, HideSelf);
        }
        private IEnumerator SmoothlyMoveHeight(float targetSize, float transitionDuration, System.Action onCompleteCallback = null)
        {
            float transitionRate = 1.0f / transitionDuration;
            while (Mathf.Approximately(_topBarTransform.sizeDelta.y, targetSize))
            {
                float currentHeight = Mathf.MoveTowards(_topBarTransform.sizeDelta.y, targetSize, transitionRate * Time.deltaTime);
                _topBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, targetSize);
                _bottomBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, targetSize);

                yield return null;
            }

            _topBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, targetSize);
            _bottomBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, targetSize);

            onCompleteCallback?.Invoke();
        }


        [ContextMenu("Test")]
        private void Test()
        {
            _topBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, _defaultCutsceneBarSize);
            _bottomBarTransform.sizeDelta = new Vector2(_topBarTransform.sizeDelta.x, _defaultCutsceneBarSize);
        }


        private void ShowSelf() => gameObject.SetActive(true);
        private void HideSelf() => gameObject.SetActive(false);
    }
}
