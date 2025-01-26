using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ReVR.Vehicles.Bike.Editors
{
    public class BikeEditorUtility
    {
        public static List<string> GetAllBikeFeaturesPathName()
        {
            List<string> options = new();
            FindAllFeature(typeof(BikeFeature), options, "", false);

            return options;
        }

        public static List<Type> GetDerivedTypes(Type baseType)
        {
            List<Type> derivedTypes = new();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    IEnumerable<Type> types = assembly.GetTypes().Where(type => type.BaseType == baseType);
                    derivedTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogWarning($"Could not load types from assembly: {assembly.FullName}\n{ex.Message}");
                }
            }

            derivedTypes.Remove(baseType);
            return derivedTypes;
        }

        public static string FormatStringWithSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Replace underscore (_) or dashes (-) with spaces
            string result = input.Replace('_', ' ').Replace('-', ' ');

            // Insert space before uppercase letters
            result = Regex.Replace(result, "(?<!^)([A-Z])", " $1");

            // Normalize multiple spaces into a single space
            result = Regex.Replace(result, @"\s+", " ").Trim();

            return result;
        }

        public static string FormatFeatureName(string featureName)
        {
            string name = FormatStringWithSpaces(featureName);

            name = name.Replace("Feature", "").Trim();
            name = Regex.Replace(name, @"\s+", " ");

            return name;
        }

        private static void FindAllFeature(Type baseType, List<string> options, string menuName, bool addMenuNameToOptionsList = true)
        {
            if (addMenuNameToOptionsList)
            {
                string name = FormatFeatureName(baseType.Name);
                menuName += string.IsNullOrEmpty(menuName) ? name : $"/{name}";

                if (!baseType.IsAbstract)
                    options.Add(menuName);
            }

            List<Type> types = GetDerivedTypes(baseType);
            foreach (Type type in types)
            {
                FindAllFeature(type, options, menuName);
            }
        }

        public static string TrimPathToFolder(string path, string folderName)
        {
            int index = path.Length;
            string[] foldersName = path.Split('/');

            for (int i = foldersName.Length - 1; i >= 0; i--)
            {
                index -= foldersName[i].Length;
                if (string.Compare(foldersName[i], folderName) == 0)
                    break;
                index -= 1;
            }

            return (index < 0) ? path : path.Substring(0, index + folderName.Length);
        }
    }
}
