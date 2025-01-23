using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ShowAsTabButtonInInspector : Attribute
{
    public readonly string[] buttonNames;
    public ShowAsTabButtonInInspector(params string[] buttons)
    {
        buttonNames = buttons;
    }
}
