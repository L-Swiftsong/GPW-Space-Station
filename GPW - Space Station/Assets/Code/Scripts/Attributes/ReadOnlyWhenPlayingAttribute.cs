using UnityEngine;

/// <summary> An attribute that makes a public/serialized field visible in the inspector, but only editable when the project isn't playing.</summary>
public class ReadOnlyWhenPlayingAttribute : PropertyAttribute
{
    
}
