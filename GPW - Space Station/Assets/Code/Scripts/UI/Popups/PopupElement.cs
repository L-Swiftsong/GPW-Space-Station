using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public class PopupElement : MonoBehaviour
    {
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private Transform _offsetTransform; 

        private Action _onDisableCallback;


        public void SetPosition(Transform pivot, Vector3 offset, bool rotateInPlace)
        {
            if (rotateInPlace)
            {
                _pivotTransform.position = pivot.position + offset;
                _offsetTransform.localPosition = Vector3.zero;
            }
            else
            {
                _pivotTransform.position = pivot.position;
                _offsetTransform.localPosition = offset;
            }
        }


        private void Update()
        {
            
        }


        public void SetInformation(string preText, Sprite sprite, string postText)
        {

        }
    }
}
