using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Computers
{
    [CreateAssetMenu(menuName = "Computers/Terminal Logs", fileName = "NewTerminalLog")]
    public class TerminalLogSO : ScriptableObject
    {
        public string LogName;
        [TextArea()] public string LogText;
    }
}
