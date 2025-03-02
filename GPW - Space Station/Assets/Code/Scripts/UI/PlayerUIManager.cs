using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        private static PlayerUIManager s_instance;
        public static PlayerUIManager Instance
        {
            get => s_instance;
            private set
            {
                if (s_instance != null)
                {
                    Destroy(value);
                    return;
                }

                s_instance = value;
            }
        }


        [SerializeField] private GameObject _interactionUIRoot;


        private void Awake()
        {
            Instance = this;
        }

        public void ShowInteractionUI() => _interactionUIRoot.SetActive(true);
        public void HideInteractionUI() => _interactionUIRoot.SetActive(false);
    }
}
