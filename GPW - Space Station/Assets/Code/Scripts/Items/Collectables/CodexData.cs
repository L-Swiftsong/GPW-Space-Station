using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Codex
{
    [CreateAssetMenu(menuName = "Collectables/New Codex Data")]
    public class CodexData : ScriptableObject
    {
        [SerializeField] private string _codexName;
        [SerializeField] private string _codexDate;
        [SerializeField] [TextArea(5, 15)] private string _condexContents;
    }
}
