using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving
{
    public interface ISaveable
    {
        SerializableGuid SaveID { get; set; }
    }
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid ID { get; set; }
        void Bind(TData data);
    }
}
