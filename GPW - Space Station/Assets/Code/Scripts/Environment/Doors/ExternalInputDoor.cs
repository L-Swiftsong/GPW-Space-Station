namespace Environment.Doors
{
    public class ExternalInputDoor : Door, ITriggerable
    {
        /// <summary> Toggles the door's open state (Closed > Open | Open > Closed).</summary>
        public void Trigger() => base.ToggleOpen();

        /// <summary> Opens the door.</summary>
        public void Activate() => base.Open();

        /// <summary> Closes the door.</summary>
        public void Deactivate() => base.Close();
    }
}