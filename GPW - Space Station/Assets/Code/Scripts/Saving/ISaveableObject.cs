using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving.LevelData
{
    public interface ISaveableObject
    {
        SerializableInstanceGuid ID { get; set; }

        public void BindExisting(ObjectSaveData saveData);
        public ObjectSaveData BindNew();
        public void InitialiseID();


        #region Static Functions

        protected static DisabledState DetermineDisabledState(MonoBehaviour monoBehaviour)
        {
            DisabledState disabledState = DisabledState.None;
            if (!monoBehaviour.enabled)
                disabledState |= DisabledState.ComponentDisabled;
            if (!monoBehaviour.gameObject.activeSelf)
                disabledState |= DisabledState.EntityDisabled;

            return disabledState;
        }


        protected static void PerformBindingChecks(ObjectSaveData saveData, MonoBehaviour monoBehaviour)
        {
            if (saveData.DisabledState.HasFlag(DisabledState.Destroyed))
            {
                GameObject.Destroy(monoBehaviour.gameObject);
            }
            PerformSharedBindingChecks(saveData, monoBehaviour);
        }
        protected static void PerformBindingChecks(ObjectSaveData saveData, MonoBehaviour monoBehaviour, System.Action onDestroyCallback)
        {
            if (saveData.DisabledState.HasFlag(DisabledState.Destroyed))
            {
                onDestroyCallback?.Invoke();
            }
            PerformSharedBindingChecks(saveData, monoBehaviour);
        }
        private static void PerformSharedBindingChecks(ObjectSaveData saveData, MonoBehaviour monoBehaviour)
        {
            monoBehaviour.enabled = !saveData.DisabledState.HasFlag(DisabledState.ComponentDisabled);
            //monoBehaviour.gameObject.SetActive(!saveData.DisabledState.HasFlag(DisabledState.EntityDisabled));

            monoBehaviour.transform.position = saveData.Position;
            monoBehaviour.transform.rotation = saveData.Rotation;
        }


        protected static void DefaultOnEnableSetting(ObjectSaveData saveData, MonoBehaviour monoBehaviour)
        {
            saveData.DisabledState &= ~DisabledState.ComponentDisabled;
            if (monoBehaviour.gameObject.activeSelf)
                saveData.DisabledState &= ~DisabledState.EntityDisabled;
        }
        protected static void DefaultOnDisableSetting(ObjectSaveData saveData, MonoBehaviour monoBehaviour)
        {
            if (!monoBehaviour.enabled)
                saveData.DisabledState |= DisabledState.ComponentDisabled;
            if (!monoBehaviour.gameObject.activeSelf) // Note: Only triggers if the component is enabled when the GO is disabled.
                saveData.DisabledState |= DisabledState.EntityDisabled;
        }
        protected static void UpdatePositionAndRotationInformation(ObjectSaveData saveData, MonoBehaviour monoBehaviour)
        {
            saveData.Position = monoBehaviour.transform.position;
            saveData.Rotation = monoBehaviour.transform.rotation;
        }

        #endregion
    }
}