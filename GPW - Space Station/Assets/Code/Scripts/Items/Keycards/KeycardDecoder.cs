using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Keycards
{
    public class KeycardDecoder : MonoBehaviour
    {
        [SerializeField] private int _securityLevel = 0;


        public void SetSecurityLevel(int newAccessibleLevel, bool allowReduction = false)
        {
            if (allowReduction || _securityLevel < newAccessibleLevel)
            {
                _securityLevel = newAccessibleLevel;
            }
        }
        public int GetSecurityLevel() => _securityLevel;
    }
}