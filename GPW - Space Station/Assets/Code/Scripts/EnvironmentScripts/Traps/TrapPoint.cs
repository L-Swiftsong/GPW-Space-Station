using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPoint : MonoBehaviour
{
    private void OnEnable() => TrapManager.AddTrapPoint(this);
    private void OnDisable() => TrapManager.RemoveTrapPoint(this);
}
