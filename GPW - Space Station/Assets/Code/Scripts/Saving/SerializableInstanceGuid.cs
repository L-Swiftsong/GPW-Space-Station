using System;
using UnityEngine;

[Serializable]
public class SerializableInstanceGuid : IEquatable<SerializableInstanceGuid>
{
    [SerializeField, HideInInspector] public GameObject LinkedGameObject;
    [property: SerializeField, HideInInspector] public int InstanceID => IsUnlinked() ? 0 : LinkedGameObject.GetInstanceID();
    [SerializeField, HideInInspector] public uint Part2;
    [SerializeField, HideInInspector] public uint Part3;
    [SerializeField, HideInInspector] public uint Part4;

    public static SerializableInstanceGuid Empty => new(0, 0, 0);

    public SerializableInstanceGuid(uint val2, uint val3, uint val4)
    {
        LinkedGameObject = null;
        Part2 = val2;
        Part3 = val3;
        Part4 = val4;
    }
    public SerializableInstanceGuid(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        LinkedGameObject = null;
        Part2 = BitConverter.ToUInt32(bytes, 4);
        Part3 = BitConverter.ToUInt32(bytes, 8);
        Part4 = BitConverter.ToUInt32(bytes, 12);
    }

    public static SerializableInstanceGuid NewUnlinkedGuid() => new SerializableInstanceGuid(Guid.NewGuid());

    public bool IsUnlinked() => this.LinkedGameObject == null;
    public void LinkGuidToGameObject(GameObject gameObject) => LinkedGameObject = gameObject;


    public override bool Equals(object obj) => obj is SerializableInstanceGuid guid && this.Equals(guid);
    public bool Equals(SerializableInstanceGuid other) => InstanceID == other.InstanceID && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;

    public override int GetHashCode() => HashCode.Combine(InstanceID, Part2, Part3, Part4);
    public override string ToString() => String.Concat(InstanceID, Part2, Part3, Part4);

    public static bool operator ==(SerializableInstanceGuid left, SerializableInstanceGuid right) => left.Equals(right);
    public static bool operator !=(SerializableInstanceGuid left, SerializableInstanceGuid right) => !(left == right);
}
