using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Puzzles
{
    public class GridBreakerPuzzle : MonoBehaviour
    {
        [SerializeField] private List<BreakerSwitch> _breakerInitialisationList = new List<BreakerSwitch>();
        private BreakerSwitch[,] _breakerSwitches;
        [SerializeField] private bool[] _startingValues;

        [SerializeField] private int _columnCount = 5;
        private int _rowCount = 0;


        [System.Serializable] private enum ToggleType { Orthogonal, Diagonal, EightNeighbour }
        [SerializeField] private ToggleType _toggleType = ToggleType.Orthogonal;

        [SerializeField] private Light _completionLight;


        private void Start()
        {
            _rowCount = Mathf.FloorToInt(_breakerInitialisationList.Count / _columnCount);
            _breakerSwitches = new BreakerSwitch[_columnCount, _rowCount];

            for (int x = 0; x < _columnCount; ++x)
            {
                for(int y = 0; y < _rowCount; ++y)
                {
                    _breakerSwitches[x, y] = _breakerInitialisationList[(_columnCount * x) + y];
                    _breakerSwitches[x, y].SetInitialValue(x, y, _startingValues[(_columnCount * x) + y]);
                    Debug.Log((_columnCount * x) + y);
                }
            }

            _completionLight.enabled = false;
        }


        public void SwitchToggled(int gridX, int gridY)
        {
            switch (_toggleType)
            {
                case ToggleType.Orthogonal:
                    _breakerSwitches[gridX, gridY].ToggleEnabled();
                    if (gridX - 1 >= 0) { _breakerSwitches[gridX - 1, gridY].ToggleEnabled(); }             // Left.
                    if (gridX + 1 < _columnCount) { _breakerSwitches[gridX + 1, gridY].ToggleEnabled(); }   // Right.
                    if (gridY - 1 >= 0) { _breakerSwitches[gridX, gridY - 1].ToggleEnabled(); }             // Bottom.
                    if (gridY + 1 < _rowCount) { _breakerSwitches[gridX, gridY + 1].ToggleEnabled(); }      // Top.
                    break;
                case ToggleType.Diagonal:
                    _breakerSwitches[gridX, gridY].ToggleEnabled();
                    if (gridX - 1 >= 0 && gridY + 1 < _rowCount) { _breakerSwitches[gridX - 1, gridY + 1].ToggleEnabled(); } // Top Left.
                    if (gridX - 1 >= 0 && gridY - 1 >= 0) { _breakerSwitches[gridX - 1, gridY - 1].ToggleEnabled(); } // Bottom Left.
                    if (gridX + 1 < _columnCount && gridY + 1 < _rowCount) { _breakerSwitches[gridX + 1, gridY + 1].ToggleEnabled(); } // Top Right.
                    if (gridX + 1 < _columnCount && gridY - 1 >= 0) { _breakerSwitches[gridX + 1, gridY - 1].ToggleEnabled(); } // Bottom Right.
                    break;
                case ToggleType.EightNeighbour:
                    for (int x = -1; x <= 1; ++x)
                    {
                        if (gridX + x < 0 || gridX + x >= _columnCount)
                        {
                            // Outwith grid (Horizontally).
                            continue;
                        }

                        for (int y = -1; y <= 1; ++y)
                        {
                            if (gridY + y < 0 || gridY + y >= _rowCount)
                            {
                                // Outwith grid (Vertically).
                                continue;
                            }

                            _breakerSwitches[gridX + x, gridY + y].ToggleEnabled();
                        }
                    }
                    break;
            }

            CheckCompletionState();
        }


        private void CheckCompletionState()
        {
            bool allComplete = true;
            for(int x = 0; x < _columnCount; ++x)
            {
                for(int y = 0; y < _rowCount; ++y)
                {
                    if (_breakerSwitches[x, y].GetIsEnabled() == false)
                    {
                        allComplete = false;
                        break;
                    }
                }

                if (!allComplete)
                    break;
            }

            if (allComplete)
            {
                Debug.Log("Complete");
                _completionLight.enabled = true;
            }
        }
    }
}