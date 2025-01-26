using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class RequiredFieldHierarchyDrawer
{
    private static readonly GUIContent s_IconContent = new();

    static RequiredFieldHierarchyDrawer()
    {
        s_IconContent.tooltip = "One or more required fields are missing or empty.";

        EditorApplication.hierarchyWindowItemOnGUI -= OnDrawHierarchy;
        EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchy;
    }

    private static void InitIcon()
    {
        if (s_IconContent.image != null)
            return;

        s_IconContent.image = RequiredFieldResourceHandler.RequiredIcon;
    }

    private static void DrawIcon(Rect selectionRect)
    {
        Rect iconRect = new(selectionRect.xMax - 20.0f, selectionRect.y, 16.0f, 16.0f);
        //GUI.Label(iconRect, s_IconContent);

        EditorGUIUtility.AddCursorRect(iconRect, MouseCursor.Link);

        if (GUI.Button(iconRect, s_IconContent, GUIStyle.none))
            Debug.Log(s_IconContent.tooltip);
    }

    private static bool IsArrayOrList(Type type)
    {
        return type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
    }

    private static void OnDrawHierarchy(int instanceID, Rect selectionRect)
    {
        if (EditorUtility.InstanceIDToObject(instanceID) is not GameObject gameObject)
            return;


    }

}
