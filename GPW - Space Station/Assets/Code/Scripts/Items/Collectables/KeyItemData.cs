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


        public Sprite KeyItemSprite => _keyItemSprite;
    }
}
