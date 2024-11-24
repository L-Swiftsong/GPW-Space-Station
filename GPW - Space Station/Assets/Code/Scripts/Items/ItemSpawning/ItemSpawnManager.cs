using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Keycards;

public class ItemSpawnManager : MonoBehaviour
{
    private static ItemSpawnManager s_instance;
    [SerializeField] private List<KeycardSpawnPositions> _keycardSpawnPositionsList = new List<KeycardSpawnPositions>();


    private void Awake()
    {
        s_instance = this;

        // Spawn & setup all keycard instances.
        for (int i = 0; i < _keycardSpawnPositionsList.Count; i++)
        {
            SpawnPosition spawnPosition = _keycardSpawnPositionsList[i].SpawnPositions[Random.Range(0, _keycardSpawnPositionsList[i].SpawnPositions.Length)];
            Transform keycardInstance = Instantiate(_keycardSpawnPositionsList[i].KeycardPrefab, spawnPosition.Position, Quaternion.Euler(spawnPosition.Rotation));

            if (!keycardInstance.TryGetComponent(out KeycardPickup keycard))
            {
                Debug.LogError("Error: KeycardSpawnPosition at index " + i + "'s Keycard Prefab does not contain the 'Keycard' class");
            }

            keycard.SetupKeycard(_keycardSpawnPositionsList[i].SecurityLevel);
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
        [SerializeField] private int _securityLevel;
        

        [SerializeField] private Transform _keycardPrefab;
        [SerializeField] private SpawnPosition[] _spawnPositions;


        [SerializeField] private Color _debugColour;


        #region Properties

        public Transform KeycardPrefab => _keycardPrefab;

        public int SecurityLevel => _securityLevel;

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
