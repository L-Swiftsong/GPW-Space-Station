using UnityEngine;

namespace Environment.Partitioning
{
    public class LevelPartitionRoot : MonoBehaviour
    {
        [SerializeField] private LevelSection _levelSection;
        private static bool s_hasPerformedInitialCheck = false;


        private void Awake()
        {
            LevelPartitionManager.OnLevelSectionEnabled += LevelPartitionManager_OnLevelSectionEnabled;
            LevelPartitionManager.OnLevelSectionDisabled += LevelPartitionManager_OnLevelSectionDisabled;
        }
        private void Start()
        {
            if (!s_hasPerformedInitialCheck)
            {
                LevelPartitionManager.InitialiseCheck();
                s_hasPerformedInitialCheck = true;
            }
        }
        private void OnDestroy()
        {
            LevelPartitionManager.OnLevelSectionEnabled -= LevelPartitionManager_OnLevelSectionEnabled;
            LevelPartitionManager.OnLevelSectionDisabled -= LevelPartitionManager_OnLevelSectionDisabled;
        }


        private void LevelPartitionManager_OnLevelSectionEnabled(LevelSection toggledSectionType)
        {
            if (toggledSectionType == this._levelSection)
            {
                this.gameObject.SetActive(true);
            }
        }
        private void LevelPartitionManager_OnLevelSectionDisabled(LevelSection toggledSectionType)
        {
            if (toggledSectionType == this._levelSection)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}