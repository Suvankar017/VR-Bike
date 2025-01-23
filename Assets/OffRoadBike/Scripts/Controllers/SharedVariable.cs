using System;
using UnityEngine;

public abstract class SharedVariable { }

[Serializable]
public class SharedVariable<T> : SharedVariable
{
    public T value;

    public SharedVariable() { }

    public SharedVariable(T value)
    {
        this.value = value;
    }

    public static implicit operator SharedVariable<T>(T value)
    {
        return new SharedVariable<T>(value);
    }
}

[Serializable]
public class SharedFloat : SharedVariable<float>
{
    public SharedFloat() { }

    public SharedFloat(float value) : base(value) { }

    public static implicit operator SharedFloat(float value)
    {
        return new SharedFloat(value);
    }
}

[Serializable]
public class SharedBool : SharedVariable<bool>
{
    public SharedBool() { }

    public SharedBool(bool value) : base(value) { }

    public static implicit operator SharedBool(bool value)
    {
        return new SharedBool(value);
    }
}

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
