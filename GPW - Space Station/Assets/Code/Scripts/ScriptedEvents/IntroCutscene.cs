using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using Entities.Player;
using Saving.LevelData;

namespace ScriptedEvents
{
    public class IntroCutscene : MonoBehaviour, ISaveableObject
    {
        #if UNITY_EDITOR
        [SerializeField] private bool _disableInEditor;
        #endif
        

        #region Saving Variables & References

        [field: SerializeField] public SerializableGuid ID { get; set; }
        [SerializeField] private ObjectSaveData _saveData;

        #endregion


        [Header("References")]
        [SerializeField] private Animator _animator;
        private readonly int INTRO_CUTSCENE_CLIP_HASH = Animator.StringToHash("IntroCutsceneCameraMovement");


        private void Awake()
        {
            #if UNITY_EDITOR
            if (_disableInEditor)
            {
                Destroy(this.gameObject);
                return;
            }
            #endif

            SceneLoader.OnLoadFinished += SceneLoader_OnLoadFinished;
        }
        private void OnDestroy()
        {
            SceneLoader.OnLoadFinished -= SceneLoader_OnLoadFinished;
            PlayerInput.RemoveAllActionPrevention(typeof(IntroCutscene));
            _saveData.DisabledState = DisabledState.Destroyed;
        }


        private void SceneLoader_OnLoadFinished()
        {
            DisablePlayerAndInput();
            _animator.Play(INTRO_CUTSCENE_CLIP_HASH);
        }
        public void OnIntroCutsceneCompleted()
        {
            Debug.Log("Intro Cutscene Complete");
            EnablePlayerAndInput();
            Destroy(this.gameObject);
        }
        
        
        private void DisablePlayerAndInput()
        {
            PlayerManager.Instance.Player.gameObject.SetActive(false);
            PlayerInput.PreventAllActions(typeof(IntroCutscene), disableGlobalMaps: true);
        }
        private void EnablePlayerAndInput()
        {
            PlayerManager.Instance.Player.gameObject.SetActive(true);
            PlayerInput.RemoveAllActionPrevention(typeof(IntroCutscene));
        }


        #region Saving Functions

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = saveData;
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData, this, onDestroyCallback: OnIntroCutsceneCompleted);
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

            ISaveableObject.UpdatePositionAndRotationInformation(this._saveData, this);

            return this._saveData;
        }

        #endregion
    }
}
