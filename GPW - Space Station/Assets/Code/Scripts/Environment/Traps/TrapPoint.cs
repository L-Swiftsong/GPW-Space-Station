using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Traps
{
    public class TrapPoint : MonoBehaviour
    {
        private void OnEnable() => TrapManager.AddTrapPoint(this);
        private void OnDisable() => TrapManager.RemoveTrapPoint(this);
    }
}