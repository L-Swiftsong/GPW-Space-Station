namespace Interaction
{
    public interface IInteractable
    {
        public void Interact(PlayerInteraction interactingScript);

        public void Highlight() { }
        public void StopHighlighting() { }
    }
}