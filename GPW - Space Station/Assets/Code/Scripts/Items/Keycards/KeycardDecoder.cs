using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Keycards
{
    public class KeycardDecoder : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private int _securityLevel = 0;


        [Header("Equipping Animation")]
        private Animator _animator;
        private const string ANIMATOR_EQUIPPED_IDENTIFIER = "IsEquipped";


        private void Awake()
        {
            _animator = GetComponent<Animator>();

            KeycardReader.OnAnyKeycardReaderHighlighted += KeycardReader_OnAnyKeycardReaderHighlighted;
            KeycardReader.OnAnyKeycardReaderStopHighlighted += KeycardReader_OnAnyKeycardReaderStopHighlighted;
        }
        private void OnDestroy()
        {
            KeycardReader.OnAnyKeycardReaderHighlighted -= KeycardReader_OnAnyKeycardReaderHighlighted;
            KeycardReader.OnAnyKeycardReaderStopHighlighted -= KeycardReader_OnAnyKeycardReaderStopHighlighted;
        }


        private void KeycardReader_OnAnyKeycardReaderHighlighted(object sender, System.EventArgs e) => Equip();
        private void KeycardReader_OnAnyKeycardReaderStopHighlighted(object sender, System.EventArgs e) => Unequip();


        public void SetSecurityLevel(int newAccessibleLevel, bool allowReduction = false)
        {
            if (allowReduction || _securityLevel < newAccessibleLevel)
            {
                _securityLevel = newAccessibleLevel;
            }
        }
        public int GetSecurityLevel() => _securityLevel;


        private void Equip()
        {
            Debug.Log("Equip");
            _animator.SetBool(ANIMATOR_EQUIPPED_IDENTIFIER, true);
        }
        private void Unequip()
        {
            _animator.SetBool(ANIMATOR_EQUIPPED_IDENTIFIER, false);
        }
    }
}