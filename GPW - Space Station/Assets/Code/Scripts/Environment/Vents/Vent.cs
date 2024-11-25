using UnityEngine;

namespace Environment.Vents
{
    public class Vent : MonoBehaviour
    {
        private VentEntrance[] _ventEntrances;
        public VentEntrance[] VentEntrances => _ventEntrances;

        private void Awake() => _ventEntrances = GetComponentsInChildren<VentEntrance>();
    }
}