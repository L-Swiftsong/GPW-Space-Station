namespace Environment.Doors
{
    public class ExternalInputDoor : Door, ITriggerable
    {
        /// <remarks> Toggles the door's open state (Closed > Open | Open > Closed).</remarks>
        public void Trigger() => base.ToggleOpen();

        /// <remarks> Opens the door.</remarks>
        public void Activate() => base.Open();
        /// <remarks> Closes the door.</remarks>
        public void Deactivate() => base.Close();
    }
}