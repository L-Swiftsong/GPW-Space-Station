using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Collectables
{
    [CreateAssetMenu(menuName = "Collectables/New Key Item Data", fileName = "KeyItemData")]
    public class KeyItemData : CollectableData
    {
        [Header("Key Item")]
        [SerializeField] private Sprite _keyItemSprite;
        [SerializeField] private GameObject _keyItemPrefab;

        public Sprite KeyItemSprite => _keyItemSprite;
        public GameObject KeyItemPrefab => _keyItemPrefab;
    }
}
