#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Saving.LevelData
{
    [InitializeOnLoad]
    public class SaveableObjectPrefabInstanceIDSetupManager
    {
        static SaveableObjectPrefabInstanceIDSetupManager()
        {
            ObjectChangeEvents.changesPublished += ChangesPublished;
        }

        static void ChangesPublished(ref ObjectChangeEventStream stream)
        {
            for(int i = 0; i < stream.length; ++i)
            {
                if (stream.GetEventType(i) == ObjectChangeKind.CreateGameObjectHierarchy)
                {
                    stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
                    var newGameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;
                    foreach (var saveableObject in newGameObject.GetComponentsInChildren<ISaveableObject>())
                    {
                        saveableObject.InitialiseID();
                    }
                }
            }
        }
    }
}
#endif