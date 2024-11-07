using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Doors;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;
using System.Xml.Linq;

public class ItemSpawnManager : MonoBehaviour
{
    [SerializeField] private List<KeycardSpawnPositions> _keycardSpawnPositionsList = new List<KeycardSpawnPositions>();


    [Header("Testing")]
    [SerializeField] private bool _runTestingChecks = true;

    [Space(5)]
    [SerializeField] private bool _overrideDoorMaterials = true;
    [SerializeField] private bool _overrideKeycardReaderMateirals = true;


    private void Awake()
    {
        // Spawn & setup all keycard instances.
        for (int i = 0; i < _keycardSpawnPositionsList.Count; i++)
        {
            SpawnPosition spawnPosition = _keycardSpawnPositionsList[i].SpawnPositions[Random.Range(0, _keycardSpawnPositionsList[i].SpawnPositions.Length)];
            Transform keycardInstance = Instantiate(_keycardSpawnPositionsList[i].KeycardPrefab, spawnPosition.Position, Quaternion.Euler(spawnPosition.Rotation));

            if (!keycardInstance.TryGetComponent(out KeyCard keycard))
            {
                Debug.LogError("Error: KeycardSpawnPosition at index " + i + "'s Keycard Prefab does not contain the 'Keycard' class");
            }

            keycard.SetupKeycard(_keycardSpawnPositionsList[i].KeycardID, _keycardSpawnPositionsList[i].KeycardMaterial);
        }


        if (_runTestingChecks)
        {
            PerformTestingChecks();
        }
    }


    private void PerformTestingChecks()
    {
        HashSet<int> foundIDs = new HashSet<int>();

        // Override Interactable Door Materials.
        foreach (InteractableDoor door in FindObjectsOfType<InteractableDoor>())
        {
            // Doors with a Required Keycard ID of -1 are unlocked.
            if (door.RequiredKeycardID == -1)
                continue;

            foundIDs.Add(door.RequiredKeycardID);

            if (_overrideDoorMaterials)
            {
                Material overrideMaterial = null;

                try
                {
                    overrideMaterial = _keycardSpawnPositionsList.Where(ksp => ksp.KeycardID == door.RequiredKeycardID).First().KeycardMaterial;
                }
                catch
                {
                    Debug.LogError("Error: No spawned Keycards are assigned to the KeycardID " + door.RequiredKeycardID + ", however the Interactable Door " + door.name + " requires it.");
                    return;
                }

                door.OverrideMaterial(overrideMaterial);
            }
        }

        // Override DoorButton Materials.
        foreach (DoorButton doorButton in FindObjectsOfType<DoorButton>())
        {
            if (doorButton.RequiredKeycardID == -1)
            {
                // This button doesn't require a keycard to use.
                continue;
            }

            foundIDs.Add(doorButton.RequiredKeycardID);

            if (_overrideKeycardReaderMateirals)
            {
                Material overrideMaterial = null;
                try
                {
                    overrideMaterial = _keycardSpawnPositionsList.Where(ksp => ksp.KeycardID == doorButton.RequiredKeycardID).First().KeycardMaterial;
                }
                catch
                {
                    Debug.LogError("Error: No spawned Keycards are assigned to the KeycardID " + doorButton.RequiredKeycardID + ", however the Keycard Reader " + doorButton.name + " requires it.");
                    return;
                }

                doorButton.OverrideMaterial(overrideMaterial);
            }
        }


        // Ensure that we aren't spawning in keycards which don't link to any doors.
        for (int i = 0; i < _keycardSpawnPositionsList.Count; i++)
        {
            if (!foundIDs.Contains(_keycardSpawnPositionsList[i].KeycardID))
            {
                Debug.LogError("Error: No Interactable Doors or Keycard Readers are assigned to the KeycardID " + _keycardSpawnPositionsList[i].KeycardID + ", however you have spawned a Keycard with this ID.");
            }
        }
    }



    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _keycardSpawnPositionsList.Count; i++)
        {
            _keycardSpawnPositionsList[i].DrawGizmos();
        }
    }

    [System.Serializable]
    private struct KeycardSpawnPositions
    {
        [SerializeField] private Transform _keycardPrefab;
        
        [SerializeField] private int _keycardID;
        [SerializeField] private Material _keycardMaterial;

        [SerializeField] private SpawnPosition[] _spawnPositions;


        [SerializeField] private Color _debugColour;


        #region Properties

        public Transform KeycardPrefab => _keycardPrefab;

        public int KeycardID => _keycardID;
        public Material KeycardMaterial => _keycardMaterial;

        public SpawnPosition[] SpawnPositions => _spawnPositions;

        #endregion


        public void DrawGizmos()
        {
            for(int i = 0; i < _spawnPositions.Length; i++)
            {
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.Rotate(Quaternion.Euler(_spawnPositions[i].Rotation));

                Gizmos.color = _debugColour;
                Gizmos.DrawCube(_spawnPositions[i].Position, new Vector3(1f, 1f, 2f) * 0.1f);

                Gizmos.matrix = oldMatrix;
            }
        }
    }

    [System.Serializable]
    private struct SpawnPosition
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _rotation;

        public Vector3 Position => _position;
        public Vector3 Rotation => _rotation;
    }
}
