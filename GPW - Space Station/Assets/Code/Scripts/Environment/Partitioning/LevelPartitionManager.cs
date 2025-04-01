using System.Collections;
using System.Collections.Generic;

namespace Environment.Partitioning
{
    public static class LevelPartitionManager
    {
        public static event System.Action<LevelSection> OnLevelSectionEnabled;
        public static event System.Action<LevelSection> OnLevelSectionDisabled;

        private static Dictionary<LevelSection, int> s_levelSectionCounts = new Dictionary<LevelSection, int>();


        public static void InitialiseCheck()
        {
            PerformCheck(LevelSection.Hub);
            PerformCheck(LevelSection.Engineering);
            PerformCheck(LevelSection.Medical);
            PerformCheck(LevelSection.CrewQuarters);
            PerformCheck(LevelSection.StealthArea);
            PerformCheck(LevelSection.VentChase);
        }
        private static void PerformCheck(LevelSection levelSectionType)
        {
            if (s_levelSectionCounts.TryGetValue(levelSectionType, out int enabledCount) == false || enabledCount == 0)
            {
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


            if (s_levelSectionCounts[levelSectionType] == 0)
            {
                OnLevelSectionDisabled?.Invoke(levelSectionType);
            }
        }
    }
}