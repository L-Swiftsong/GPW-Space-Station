using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public abstract class PopupTrigger : MonoBehaviour
    {
        [SerializeField] protected PopupTextData TextData;

        public abstract void Trigger();
    }
}