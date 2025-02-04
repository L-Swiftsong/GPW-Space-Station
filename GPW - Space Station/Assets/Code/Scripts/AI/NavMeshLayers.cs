using UnityEngine;

[System.Flags]
public enum NavMeshLayers
{
    None = 0,
    Walkable = 1 << 0,
    NotWalkable = 1 << 1,
    Jump = 1 << 2,
    Crawlable = 1 << 3
}