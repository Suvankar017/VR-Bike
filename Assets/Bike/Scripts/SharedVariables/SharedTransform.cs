using System;
using UnityEngine;

[Serializable]
public class SharedTransform : SharedVariable<Transform>
{
    public SharedTransform() { }

    public SharedTransform(Transform value) : base(value) { }

    public static implicit operator SharedTransform(Transform value)
    {
        return new SharedTransform(value);
    }
}
