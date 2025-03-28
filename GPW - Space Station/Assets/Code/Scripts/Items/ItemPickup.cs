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

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private ObjectSaveData _saveData;

        #endregion

        #region Saving Functions

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = saveData;
            _saveData.ID = ID;

            if (_saveData.WasDestroyed)
            {
                Destroy(this.gameObject);
            }
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new ObjectSaveData()
                {
                    ID = this.ID,
                    Exists = true,
                    WasDestroyed = false
                };
            }

            return this._saveData;
        }
        private void OnDestroy() => _saveData.WasDestroyed = true;

        #endregion

        #endregion
    }
}
