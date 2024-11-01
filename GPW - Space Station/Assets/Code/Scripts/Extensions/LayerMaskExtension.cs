using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtension
{
    public static bool Contains(this LayerMask mask, GameObject testedObject) => mask == (mask | (1 << testedObject.layer));
    public static bool Contains(this LayerMask mask, MonoBehaviour mono) => mask == (mask | (1 << mono.gameObject.layer));
    public static bool Contains(this LayerMask mask, int layer) => mask == (mask | (1 << layer));
}