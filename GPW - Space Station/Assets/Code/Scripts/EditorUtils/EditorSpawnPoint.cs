#if UNITY_EDITOR
using UnityEngine;
using Entities.Player;

namespace EditorUtils
{
    public class EditorSpawnPoint : MonoBehaviour
    {
        private static EditorSpawnPoint s_instance;
        private static bool s_hasTriggered = false;

        private void Awake()
        {
            if (s_hasTriggered)
            {
                Destroy(this.gameObject);
            }

            s_instance = this;
            s_hasTriggered = true;
        }

        public static void SetPlayerSpawn()
        {
            if (s_instance == null)
            {
                // No EditorSpawnPoint exists within the scene.
                s_hasTriggered = true;
                return;
            }

            Transform instanceTransform = s_instance.transform;
            PlayerManager.Instance.SetPlayerPositionAndRotation(instanceTransform.position, instanceTransform.rotation.eulerAngles.y);

            Destroy(s_instance.gameObject);
        }
    }
}
#endif