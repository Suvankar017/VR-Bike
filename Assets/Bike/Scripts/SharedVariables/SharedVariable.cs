using System;

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
