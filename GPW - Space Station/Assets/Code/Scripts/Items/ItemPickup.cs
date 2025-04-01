using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving.LevelData;

namespace Items
{
    public abstract class ItemPickup : MonoBehaviour, IInteractable, ISaveableObject
    {
        #region Interaction

        #region IInteractable Properties & Events

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion

        #region Interaction Functions

        public void Interact(PlayerInteraction interactingScript)
        {
            if (PerformInteraction(interactingScript))
            {
                OnSuccessfulInteraction?.Invoke();
                Destroy(this.gameObject);
            }
            else
            {
                OnFailedInteraction?.Invoke();
            }
        }
        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);

        protected abstract bool PerformInteraction(PlayerInteraction interactingScript);

        #endregion

        #endregion


        #region Saving

        #region Saving Variables & References

        [field: SerializeField] public SerializableGuid ID { get; set; }
        [SerializeField] private ObjectSaveData _saveData;

        #endregion

        #region Saving Functions

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = saveData;
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData, this);
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new ObjectSaveData()
                {
                    ID = this.ID,
                    Exists = true
                };
            }

            return this._saveData;
        }
        public void InitialiseID()
        {
            ID = SerializableGuid.NewGuid();
            Debug.Log(ID);
        }

        protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData, this);
        protected virtual void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
        protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData, this);
        protected virtual void LateUpdate() => ISaveableObject.UpdatePositionAndRotationInformation(this._saveData, this);

        #endregion

        #endregion
    }
}
