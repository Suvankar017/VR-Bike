using UnityEngine.UIElements;
using UnityEditor;

[CustomPropertyDrawer(typeof(FlexiableHeader), true)]
public class FlexiableHeaderDecoratorDrawer : DecoratorDrawer
{
    public override VisualElement CreatePropertyGUI()
    {
        FlexiableHeader header = attribute as FlexiableHeader;

        Label label = new(header.text) { name = "FlexiableHeader:Label", enableRichText = true };
        label.style.unityFontStyleAndWeight = header.style;
        label.style.fontSize = header.size;
        label.style.color = header.color;
        label.style.unityTextAlign = header.anchor;
        label.style.whiteSpace = header.whiteSpace;

        label.style.marginTop = 13.0f;

        label.style.overflow = Overflow.Hidden;
        label.style.textOverflow = header.overflow;

        return label;
    }
}
