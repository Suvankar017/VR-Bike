using System;

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
