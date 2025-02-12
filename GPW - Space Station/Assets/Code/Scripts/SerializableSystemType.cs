// Simple helper class that allows you to serialize System.Type objects.
// Use it however you like, but crediting or even just contacting the author would be appreciated (Always 
// nice to see people using your stuff!)
//
// Written by Bryan Keiren (http://www.bryankeiren.com)

using UnityEngine;

[System.Serializable]
public class SerializableSystemType
{
    [SerializeField] private string m_name;
    [SerializeField] private string m_assemblyQualifiedName;
    [SerializeField] private string m_assemblyName;
    private System.Type m_systemType;


    public string Name => m_name;
    public string AssemblyQualifiedName => m_assemblyQualifiedName;
    public string AssemblyName => m_assemblyName;
    public System.Type SystemType
    {
        get
        {
            if (m_systemType == null)
            {
                GetSystemType();
            }
            return m_systemType;
        }
    }



    private void GetSystemType()
    {
        m_systemType = System.Type.GetType(m_assemblyQualifiedName);
    }

    public SerializableSystemType(System.Type _SystemType)
    {
        m_systemType = _SystemType;
        m_name = _SystemType.Name;
        m_assemblyQualifiedName = _SystemType.AssemblyQualifiedName;
        m_assemblyName = _SystemType.Assembly.FullName;
    }

    public override bool Equals(System.Object obj)
    {
        SerializableSystemType temp = obj as SerializableSystemType;
        if ((object)temp == null)
        {
            return false;
        }
        return this.Equals(temp);
    }

    public bool Equals(SerializableSystemType _Object)
    {
        return _Object.SystemType.Equals(SystemType);
    }

    public override int GetHashCode()
    {
        return SystemType != null ? SystemType.GetHashCode() : 0;
    }

    public static bool operator ==(SerializableSystemType a, SerializableSystemType b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(SerializableSystemType a, SerializableSystemType b)
    {
        return !(a == b);
    }
}