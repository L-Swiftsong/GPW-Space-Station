using System.Collections.Generic;
using UnityEngine;

namespace Environment.Partitioning
{
    public class LevelPartitionRoot : MonoBehaviour
    {
        [SerializeField] private LevelSection _levelSection;
        [Tooltip("If true, then this root hides when the level section is enabled and shows when it is to be disabled.")]
            [SerializeField] private bool _invertToggleState = false;

        [SerializeField] private GameObject[] _toggledRoots = new GameObject[0];


        private void Awake()
        {
            LevelPartitionManager.OnLevelSectionEnabled += LevelPartitionManager_OnLevelSectionEnabled;
            LevelPartitionManager.OnLevelSectionDisabled += LevelPartitionManager_OnLevelSectionDisabled;
        }
        private void Start() => LevelPartitionManager.InitialiseCheck(_levelSection); // Perform our initial enabled check (Rather than only disabling when the player has entered then left a trigger).
        
        private void OnDestroy()
        {
            LevelPartitionManager.OnLevelSectionEnabled -= LevelPartitionManager_OnLevelSectionEnabled;
            LevelPartitionManager.OnLevelSectionDisabled -= LevelPartitionManager_OnLevelSectionDisabled;
        }


        private void LevelPartitionManager_OnLevelSectionEnabled(LevelSection toggledSectionType)
        {
            if (toggledSectionType == this._levelSection)
            {
                // Enable if we aren't inverting our toggle state, otherwise disable.
                ToggleChildren(!_invertToggleState ? true : false);
            }
        }
        private void LevelPartitionManager_OnLevelSectionDisabled(LevelSection toggledSectionType)
        {
            if (toggledSectionType == this._levelSection)
            {
                // Disable if we aren't inverting our toggle state, otherwise enable.
                ToggleChildren(!_invertToggleState ? false : true);
            }
        }

        private void ToggleChildren(bool desiredToggleState)
        {
            bool hasToggled = false;
            foreach (GameObject root in _toggledRoots)
            {
                root.SetActive(desiredToggleState);
                hasToggled = true;
            }

            if (!hasToggled)
            {
                // We didn't toggle any roots, so as a fallback toggle ourself.
                this.gameObject.SetActive(desiredToggleState);
            }
        }
    }
}