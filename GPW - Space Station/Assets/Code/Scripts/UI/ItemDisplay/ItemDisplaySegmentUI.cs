using UnityEngine;
using Items.Collectables;
using TMPro;

namespace UI.ItemDisplay
{
    public abstract class ItemDisplaySegmentUI : MonoBehaviour
    {
        [SerializeField] protected TMP_Text CollectableNameText;


        public virtual void SetupCollectableEntry(CollectableData collectableData)
        {
            CollectableNameText.text = collectableData.CollectableName;
        }
    }
}
