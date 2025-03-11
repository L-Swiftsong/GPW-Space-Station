namespace Interaction
{
    public interface IInteractable
    {
        protected const int INTERACTION_OUTLINE_LAYER = 11;


        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;
        public void Interact(PlayerInteraction interactingScript);

        public void Highlight();
        public void StopHighlighting();


        protected static void StartHighlight(UnityEngine.GameObject gameObject, ref int previousLayer)
        {
            if (gameObject.layer != INTERACTION_OUTLINE_LAYER)
            {
                previousLayer = gameObject.layer;
                gameObject.SetLayerRecursive(INTERACTION_OUTLINE_LAYER);
            }
        }
        protected static void StopHighlight(UnityEngine.GameObject gameObject, int previousLayer) => gameObject.SetLayerRecursive(previousLayer);
    }
}