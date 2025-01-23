using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ExtendedMonoBehaviourEditor : Editor
{
    private readonly struct ButtonInfo
    {
        public readonly string displayName;
        public readonly MethodInfo methodInfo;
        public readonly object[] parameters;
        public readonly int parameterIndexOfTabIndexParameter;

        public ButtonInfo(string buttonName, MethodInfo info, object[] parameters, int parameterIndex)
        {
            parameterIndexOfTabIndexParameter = parameterIndex;
            this.parameters = parameters;
            methodInfo = info;

            bool useMethodName = string.IsNullOrEmpty(buttonName) || string.IsNullOrWhiteSpace(buttonName);
            string name = useMethodName ? info.Name : buttonName.Trim();
            displayName = useMethodName ? ToProperTitleCase(name) : name;
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new();

        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        MonoBehaviour monoBehaviour = target as MonoBehaviour;
        if (monoBehaviour == null)
            return root;

        ShowMethodButtons(root, monoBehaviour);
        
        ShowMethodTabButtons(root, monoBehaviour);

        return root;
    }

    private static void ShowMethodTabButtons(VisualElement root, MonoBehaviour monoBehaviour)
    {
        MethodInfo[] methodInfos = monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (methodInfos == null)
            return;

        List<List<ButtonInfo>> tabInfos = new();
        foreach (MethodInfo info in methodInfos)
        {
            ShowAsTabButtonInInspector buttonAttribute = info.GetCustomAttribute<ShowAsTabButtonInInspector>();
            if (buttonAttribute == null)
                continue;

            ParameterInfo[] parameterInfos = info.GetParameters();
            object[] parameters = (parameterInfos == null || parameterInfos.Length == 0) ? null : new object[parameterInfos.Length];

            int index = -1;
            int firstIntParameterIndex = -1;

            if (parameterInfos != null)
            {
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    if (parameterInfos[i].HasDefaultValue)
                        parameters[i] = parameterInfos[i].DefaultValue;
                }

                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    ParameterInfo parameterInfo = parameterInfos[i];

                    if (parameterInfo.ParameterType != typeof(int))
                        continue;

                    if (firstIntParameterIndex < 0)
                        firstIntParameterIndex = i;

                    if (string.Compare(parameterInfo.Name, "tabIndex") != 0)
                        continue;

                    index = i;
                    break;
                }
            }
            
            if (index < 0)
                index = firstIntParameterIndex;

            string[] buttonNames = (buttonAttribute.buttonNames == null || buttonAttribute.buttonNames.Length == 0) ? new string[] { ToProperTitleCase(info.Name) } : buttonAttribute.buttonNames;
            List<ButtonInfo> buttonInfos = new();

            foreach (string buttonName in buttonNames)
                buttonInfos.Add(new(buttonName, info, parameters, index));

            tabInfos.Add(buttonInfos);
        }

        if (tabInfos.Count == 0)
            return;

        if (tabInfos.Count > 0)
        {
            VisualElement seperator = new();
            seperator.style.borderTopColor = new Color(0.733f, 0.733f, 0.733f);
            seperator.style.borderTopWidth = 1.0f;

            USSUtility.SetMargin(seperator.style, 15.0f, 20.0f, 10.0f, 20.0f);

            root.Add(seperator);
        }

        foreach (var tabInfo in tabInfos)
        {
            VisualElement tabButtonRoot = new();
            tabButtonRoot.style.flexDirection = FlexDirection.Row;
            tabButtonRoot.style.overflow = Overflow.Hidden;

            int buttonIndex = 0;
            foreach (ButtonInfo info in tabInfo)
            {
                int i = buttonIndex;
                Button button = new(() => OnTabButtonClicked(i, info.methodInfo, monoBehaviour, info.parameters, info.parameterIndexOfTabIndexParameter)) { text = info.displayName };
                buttonIndex++;

                button.style.flexGrow = 1.0f;
                button.style.unityFontStyleAndWeight = FontStyle.Bold;

                USSUtility.SetMargin(button.style, 3.0f, 0.0f);
                USSUtility.SetPadding(button.style, 5.0f, 0.0f);

                tabButtonRoot.Add(button);
            }

            root.Add(tabButtonRoot);
        }
    }

    private static void ShowMethodButtons(VisualElement root, MonoBehaviour monoBehaviour)
    {
        MethodInfo[] methodInfos = monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (methodInfos == null)
            return;

        List<ButtonInfo> buttonInfos = new();
        foreach (MethodInfo info in methodInfos)
        {
            ShowAsButtonInInspector buttonAttribute = info.GetCustomAttribute<ShowAsButtonInInspector>();
            if (buttonAttribute == null)
                continue;

            ParameterInfo[] parameterInfos = info.GetParameters();
            object[] parameters = (parameterInfos == null || parameterInfos.Length == 0) ? null : new object[parameterInfos.Length];
            
            if (parameterInfos != null)
            {
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    if (parameterInfos[i].HasDefaultValue)
                        parameters[i] = parameterInfos[i].DefaultValue;
                }
            }

            buttonInfos.Add(new(buttonAttribute.name, info, parameters, -1));
        }

        if (buttonInfos.Count > 0)
        {
            VisualElement seperator = new();
            seperator.style.borderTopColor = new Color(0.733f, 0.733f, 0.733f);
            seperator.style.borderTopWidth = 1.0f;

            USSUtility.SetMargin(seperator.style, 15.0f, 20.0f, 10.0f, 20.0f);

            root.Add(seperator);
        }

        foreach (ButtonInfo info in buttonInfos)
        {
            Button button = new(() => OnButtonClicked(info.methodInfo, monoBehaviour, info.parameters)) { text = info.displayName };

            button.style.unityFontStyleAndWeight = FontStyle.Bold;

            USSUtility.SetMargin(button.style, 3.0f, 0.0f);
            USSUtility.SetPadding(button.style, 5.0f, 0.0f);

            root.Add(button);
        }
    }

    private static void OnButtonClicked(MethodInfo methodInfo, MonoBehaviour monoBehaviour, object[] parameters)
    {
        methodInfo.Invoke(monoBehaviour, parameters);
        EditorUtility.SetDirty(monoBehaviour);
    }

    private static void OnTabButtonClicked(int tabIndex, MethodInfo methodInfo, MonoBehaviour monoBehaviour, object[] methodParameters, int parameterIndex)
    {
        if (parameterIndex >= 0)
            methodParameters[parameterIndex] = tabIndex;

        methodInfo.Invoke(monoBehaviour, methodParameters);
        EditorUtility.SetDirty(monoBehaviour);
    }

    public static string ToProperTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Handle snake_case by replacing underscores with spaces
        input = input.Replace("_", " ");

        // Handle transitions between consecutive uppercase letters followed by a lowercase letter
        input = Regex.Replace(input, @"([A-Z])([A-Z])(?=[a-z])", "$1 $2");

        // Handle transitions from lowercase to uppercase
        input = Regex.Replace(input, @"([a-z])([A-Z])", "$1 $2");

        // Capitalize the first letter of each word
        input = Regex.Replace(input, @"\b[a-z]", match => match.Value.ToUpper());

        return input;
    }

}