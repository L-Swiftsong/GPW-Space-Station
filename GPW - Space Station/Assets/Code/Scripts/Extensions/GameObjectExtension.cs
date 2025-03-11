using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
    public static void SetLayerRecursive(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetLayerRecursive(layer);
        }
    }
    public static void SetLayerRecursive(this GameObject gameObject, int layer, int layerToOverride)
    {
        if (gameObject.layer == layerToOverride)
        {
            gameObject.layer = layer;
        }

        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetLayerRecursive(layer, layerToOverride);
        }
    }
}
