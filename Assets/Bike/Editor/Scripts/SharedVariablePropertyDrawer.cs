using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(SharedVariable), true)]
public class SharedVariablePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        SerializedProperty valueProperty = property.FindPropertyRelative("value");

        VisualElement root;
        bool hasRequiredFieldAttribute = fieldInfo.IsDefined(typeof(RequiredField), false);

        if (hasRequiredFieldAttribute && IsSupportedType(valueProperty))
        {
            root = CreateRequiredFieldElement(valueProperty, property.displayName);
        }
        else
        {
            string tooltip = string.Empty;
            if (fieldInfo.IsDefined(typeof(TooltipAttribute), false))
                tooltip = fieldInfo.GetCustomAttribute<TooltipAttribute>().tooltip;

            PropertyField propertyField = new(valueProperty, property.displayName) { tooltip = tooltip };
            root = propertyField;

            propertyField.RegisterCallback<AttachToPanelEvent>((e) =>
            {
                Label label = propertyField.Q<Label>();
                if (label != null)
                    label.tooltip = tooltip;
            });
        }

        return root;
    }

    private bool IsSupportedType(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.ObjectReference: return true;
            case SerializedPropertyType.ExposedReference: return true;
            case SerializedPropertyType.AnimationCurve: return true;
            case SerializedPropertyType.String: return true;
            default: return false;
        }
    }

    private VisualElement CreateRequiredFieldElement(SerializedProperty property, string displayName)
    {
        VisualElement root = new() { name = $"{property.propertyPath}:RequiredFieldRoot" };
        root.style.flexDirection = FlexDirection.Row;

        VisualElement iconElement = CreateIconElement(property, displayName);
        VisualElement propertyElement = CreatePropertyElement(property, displayName, iconElement);

        root.Add(propertyElement);
        root.Add(iconElement);

        return root;
    }

    private VisualElement CreatePropertyElement(SerializedProperty property, string displayName, VisualElement iconContainer)
    {
        string tooltip = string.Empty;
        if (fieldInfo.IsDefined(typeof(TooltipAttribute), false))
            tooltip = fieldInfo.GetCustomAttribute<TooltipAttribute>().tooltip;

        PropertyField propertyField = new(property, displayName) { tooltip = tooltip };
        propertyField.style.flexGrow = 1.0f;
        propertyField.RegisterValueChangeCallback((e) => OnPropertyValueChanged(e, iconContainer));
        propertyField.RegisterCallback<AttachToPanelEvent>((e) =>
        {
            Label label = propertyField.Q<Label>();
            if (label != null)
                label.tooltip = tooltip;
        });

        if (property.propertyType == SerializedPropertyType.AnimationCurve)
        {
            propertyField.style.height = 20.0f;
        }

        return propertyField;
    }

    private VisualElement CreateIconElement(SerializedProperty property, string displayName)
    {
        VisualElement icon = new() { name = "Icon" };
        icon.style.flexGrow = 1.0f;
        icon.style.backgroundImage = RequiredFieldResourceHandler.RequiredIcon;
        USSUtility.SetBackgroundImageScaleMode(icon.style, ScaleMode.ScaleToFit);

        VisualElement iconContainer = new()
        {
            name = "IconContainer",
            tooltip = $"This field <i><b>{displayName}</b></i> of type <b>({property.type.Replace("PPtr<$", "").Replace(">", "")})</b> is required."
        };

        iconContainer.style.width = 20.0f;
        iconContainer.style.display = DisplayStyle.Flex;
        USSUtility.SetMargin(iconContainer.style, 1.0f, 0.0f, 1.0f, 5.0f);
        USSUtility.SetPadding(iconContainer.style, 1.0f);

        iconContainer.Add(icon);
        return iconContainer;
    }

    private void OnPropertyValueChanged(SerializedPropertyChangeEvent e, VisualElement iconContainer)
    {
        OnValueChanged(e.changedProperty, iconContainer);
    }

    private bool IsFieldAssigned(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.ObjectReference: return property.objectReferenceValue != null;
            case SerializedPropertyType.ExposedReference: return property.exposedReferenceValue != null;
            case SerializedPropertyType.AnimationCurve: return property.animationCurveValue != null && property.animationCurveValue.length > 0;
            case SerializedPropertyType.String: return !string.IsNullOrEmpty(property.stringValue);
            default: return false;
        }
    }

    private void OnValueChanged(SerializedProperty property, VisualElement iconContainer)
    {
        if (IsFieldAssigned(property))
        {
            if (iconContainer.style.display == DisplayStyle.Flex)
            {
                iconContainer.style.display = DisplayStyle.None;
                EditorApplication.RepaintHierarchyWindow();
            }
        }
        else
        {
            iconContainer.style.display = DisplayStyle.Flex;
            EditorApplication.RepaintHierarchyWindow();
        }

    }
}
