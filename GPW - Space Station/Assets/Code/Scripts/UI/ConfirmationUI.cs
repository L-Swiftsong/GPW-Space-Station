using UnityEngine;
using TMPro;

namespace UI
{
    public class ConfirmationUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _confirmationQueryText;

        private System.Action _onCancelCallback;
        private System.Action _onConfirmCallback;


        private void OnDisable()
        {
            // Reset Values.
            _onCancelCallback = null;
            _onConfirmCallback = null;
        }


        public void RequestConfirmation(string queryStatement, System.Action onCancelCallback, System.Action onConfirmCallback)
        {
            _confirmationQueryText.text = queryStatement;

            _onCancelCallback = onCancelCallback;
            _onConfirmCallback = onConfirmCallback;

            ShowConfirmationUI();
        }


        public void Cancel()
        {
            _onCancelCallback?.Invoke();
            HideConfirmationUI();
        }
        public void Confirm()
        {
            _onConfirmCallback?.Invoke();
            HideConfirmationUI();
        }


        private void ShowConfirmationUI() => this.gameObject.SetActive(true);
        private void HideConfirmationUI() => this.gameObject.SetActive(false);
    }
}
