using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapInteraction : MonoBehaviour


    {
        private Vector3 originPosition;
        private Quaternion originRotation;

        private Vector3 targetPosition;
        private Quaternion targetRotation;

        private bool isMovingToCamera = false;
        private float moveSpeed = 3f;

        [SerializeField] private float _cameraOffsetDistance = 1f;

        [field: SerializeField] public bool IsInteractable { get; set; } = true;

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        void Start()
        {
            originPosition = transform.position;
            originRotation = transform.rotation;
        }

        void Update()
        {
            if (isMovingToCamera)
            {
                SetTargetPosition();

                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, originPosition, Time.deltaTime * moveSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, originRotation, Time.deltaTime * moveSpeed);
            }
        }

        public void Interact(PlayerInteraction player)
        {
            StartInteraction();
            OnSuccessfulInteraction?.Invoke();
        }

       


        public void StartInteraction()
        {
            isMovingToCamera = true;
            PlayerInput.PreventAllActions(typeof(Lock), disableGlobalMaps: true);

            PlayerInput.OnUICancelPerformed += StopInteraction;
        }


        public void StopInteraction()
        {
            isMovingToCamera = false;
            PlayerInput.RemoveAllActionPrevention(typeof(Lock));

            PlayerInput.OnUICancelPerformed += StopInteraction;
        }


        private void SetTargetPosition()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            targetPosition = cam.transform.position + cam.transform.forward * _cameraOffsetDistance;
            targetRotation = Quaternion.LookRotation(-cam.transform.forward); // faces the camera
        }
    }
