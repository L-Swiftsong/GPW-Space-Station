using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vent : MonoBehaviour
{
    private List<VentEntrance> _ventEntrances;
    public List<VentEntrance> VentEntrances => _ventEntrances;

    private void Awake() => _ventEntrances = GetComponentsInChildren<VentEntrance>().ToList();
}
