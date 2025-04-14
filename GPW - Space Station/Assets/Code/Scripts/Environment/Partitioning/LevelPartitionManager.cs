using System.Collections;
using System.Collections.Generic;

namespace Environment.Partitioning
{
    public static class LevelPartitionManager
    {
        // Events for LevelPartitionRoots.
        public static event System.Action<LevelSection> OnLevelSectionEnabled;
        public static event System.Action<LevelSection> OnLevelSectionDisabled;

        // We're using a LevelSection to int dictionary rather than a bool[] or similar in order to prevent unintended toggling in the case that we have multiple LevelPartitionTriggers set to the same LevelSection.
        private static Dictionary<LevelSection, int> s_levelSectionCounts = new Dictionary<LevelSection, int>();


        public static void ResetPartitioningInformation() => s_levelSectionCounts = new Dictionary<LevelSection, int>();


        // Perform checks for our existing level sections so that they start enabled/disabled depending on where the player is.
        public static void InitialiseCheck(LevelSection levelSectionType) => PerformCheck(levelSectionType);
        private static void PerformCheck(LevelSection levelSectionType)
        {
            if (s_levelSectionCounts.TryGetValue(levelSectionType, out int enabledCount) == false || enabledCount == 0)
            {
                // We either have no value for this level section count, or our value is 0.
                // Disable this level section.
                OnLevelSectionDisabled?.Invoke(levelSectionType);
            }
        }


        public static void AddToEnabledCount(LevelSection levelSectionType)
        {
            if (s_levelSectionCounts.ContainsKey(levelSectionType))
            {
                ++s_levelSectionCounts[levelSectionType];
            }
            else
            {
                s_levelSectionCounts.Add(levelSectionType, 1);
            }


            // If this was the first addition to this level section, enable it.
            if (s_levelSectionCounts[levelSectionType] == 1)
            {
                OnLevelSectionEnabled?.Invoke(levelSectionType);
            }
        }
        public static void SubtractFromEnabledCount(LevelSection levelSectionType)
        {
            if (s_levelSectionCounts.ContainsKey(levelSectionType))
            {
                --s_levelSectionCounts[levelSectionType];
            }
            else
            {
                s_levelSectionCounts.Add(levelSectionType, 0);
            }


            // If we no longer wish for our level section to be enabled, disable it.
            if (s_levelSectionCounts[levelSectionType] == 0)
            {
                OnLevelSectionDisabled?.Invoke(levelSectionType);
            }
        }
    }
}