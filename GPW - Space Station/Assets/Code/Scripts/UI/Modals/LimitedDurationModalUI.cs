using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Modals
{
    public class LimitedDurationModalUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _modalText;
        [SerializeField] private Image _modalImage;

        public event System.Action OnModalEnabled;
        public event System.Action OnModalDurationElapsed;

        private void OnEnable() => OnModalEnabled?.Invoke();

        public LimitedDurationModalUI SetModal(string text, Sprite sprite, float duration, bool destroyOnDurationElapsed = false)
        {
            _modalText.text = text;
            _modalImage.sprite = sprite;

            StartCoroutine(ModalLifetime(duration, destroyOnDurationElapsed));
            return this;
        }
        private IEnumerator ModalLifetime(float duration, bool destroyOnElapsed)
        {
            yield return new WaitForSeconds(duration);

            OnModalDurationElapsed?.Invoke();

            if (destroyOnElapsed)
            {
                // We want to destroy this modal now that the duration is elapsed.
                Destroy(this.gameObject);
            }
        }
    }
}