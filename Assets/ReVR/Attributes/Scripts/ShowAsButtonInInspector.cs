using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ShowAsButtonInInspector : Attribute
{
    public readonly string name;

    public ShowAsButtonInInspector()
    {
        name = string.Empty;
    }

    public ShowAsButtonInInspector(string name)
    {
        this.name = name;
    }
}
