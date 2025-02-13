namespace Items.Collectables
{
    [System.Serializable]
    public enum CollectableDataType
    {
        Codex,
        KeyItem,
    }


    public static class CollectableDataTypeExtensions
    {
        public static System.Type ToSystemType(this CollectableDataType collectableDataType)
        {
            return collectableDataType switch
            {
                CollectableDataType.Codex => typeof(CodexData),
                CollectableDataType.KeyItem => typeof(KeyItemData),

                _ => throw new System.NotImplementedException($"Conversion to System.Type is not implemented for CollectableDataType: {collectableDataType.ToString()}"),
            };
        }
        public static CollectableDataType ToCollectableType<T>(this T systemType) where T : CollectableData
        {
            return systemType switch
            {
                CodexData => CollectableDataType.Codex,
                KeyItemData => CollectableDataType.KeyItem,

                _ => throw new System.NotImplementedException($"Conversion to CollectableDataType is not implemented for System.Type: {systemType.ToString()}"),
            };
        }
    }
}