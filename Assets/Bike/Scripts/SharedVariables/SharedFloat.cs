using System;

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
