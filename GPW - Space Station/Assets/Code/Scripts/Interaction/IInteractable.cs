namespace Interaction
{
    public interface IInteractable
    {
        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;
        public void Interact(PlayerInteraction interactingScript);

        public void Highlight() { }
        public void StopHighlighting() { }
    }
}