﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
// Register a SettingsProvider using IMGUI for the drawing framework:
static class StateMachineSettingsRegister
{
    private const string MainDataPath = "Assets/StateMachine/";
    private const string PackageDirectoryPath = "Packages/com.danqa1337.statemachine/Scripts/";
    private const string ScriptsDirectoryPath = "Assets/StateMachine/Scripts/";
    private static List<string> _gameStateNames = new List<string>() {};
    private static List<string> _UINames = new List<string>() {};
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {

        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/MyCustomIMGUISettings", SettingsScope.Project)
        {
            // By default the last token of the path is used as display name if no label is provided.
            label = "State Machine Settings",
            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            activateHandler = (s, v) =>
            {
                _gameStateNames = LoadEnums<GameStateName>();
                _UINames = LoadEnums<UIName>();
            },

            guiHandler = (searchContext) =>
            {
                EditorGUILayout.LabelField("Game state names");

                DrawEnum(_gameStateNames);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("UI names");

                DrawEnum(_UINames);
                EditorGUILayout.Space();


                EditorGUILayout.LabelField("!Don't forget to save!");
                if(EditorGUILayout.LinkButton("Save"))
                {
                    SaveEnums<GameStateName>(_gameStateNames);
                    _gameStateNames = LoadEnums<GameStateName>();

                    SaveEnums<UIName>(_UINames);
                    _UINames = LoadEnums<UIName>();

                }
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("GameState/UI pairs");
                DrawPairs();
            },
            


            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "Number", "Some String" })
        };

        return provider;
    }
    private static void DrawEnum(List<string> enumNames)
    {
        for (int i = 0; i < enumNames.Count; i++)
        {
            var input = EditorGUILayout.TextField(enumNames[i].ToString());
            
            input = new string((from c in input
                              where char.IsLetter(c)
                              select c
                   ).ToArray());

            enumNames[i] = input;
        }
        EditorGUILayout.BeginHorizontal();
        if (EditorGUILayout.LinkButton("Add"))
        {
            enumNames.Add("");
        }
        if (enumNames.Count > 0 && EditorGUILayout.LinkButton("Remove"))
        {
            enumNames.RemoveAt(enumNames.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

    }
    private static List<string> LoadEnums<T>()
    {
        
        var result = new List<string>();

        if (File.Exists(MainDataPath + typeof(T).ToString() + ".txt"))
        {
            var values = JsonUtility.FromJson<EnumData>(File.ReadAllText(MainDataPath + typeof(T).ToString() + ".txt")).names;
            foreach (var value in values)
            {
                result.Add(value);
            }
        }
        else
        {
            var values = Enum.GetValues(typeof(T));
            foreach (var value in values)
            {
                result.Add(value.ToString());
            }
        }

        return result;
    }
    private static void SaveEnums<T>(List<string> names)
    {
        var data = new EnumData { names = names.ToArray() };

        File.WriteAllText(MainDataPath + typeof(T).ToString() + ".txt", JsonUtility.ToJson(data));
        


        var text = "public enum " + typeof(T).ToString() + "\r\n{";
        var filteredNames = new List<string>();

        foreach (var name in names)
        {
            if (name != string.Empty)
            {
                if (!filteredNames.Contains(name))
                {
                    filteredNames.Add(name);
                }
            }
        }
        if (filteredNames.Count == 0)
        {
            filteredNames.Add("Default");
        }

        foreach (var item in filteredNames)
        {
            text += item + ",";
        }
        text += "}";

        var fileName = typeof(T).ToString() + ".cs";
        var finalPath = "";


        if (Directory.Exists(PackageDirectoryPath))
        {
            finalPath = PackageDirectoryPath + fileName;
        }
        else
        {
            finalPath = ScriptsDirectoryPath + fileName;
        }

        var isRealyNeedToWrite = true;
        if (File.Exists(finalPath))
        {
            var oldText = File.ReadAllText(finalPath);
            if (oldText == text)
            {
                isRealyNeedToWrite = false;
            }
            else
            {
                Debug.Log(typeof(T).ToString() + " Files are identical");
            }
        }

        if (isRealyNeedToWrite)
        {
            
            File.WriteAllText(finalPath, text);
            AssetDatabase.Refresh();
            Debug.Log("Enum " + typeof(T).ToString() + " updated");
        }
    }


    private static void DrawPairs()
    {
        var settingsObject = StateMachineSettings.GetSettings();


        var list = settingsObject.PairsList;
        var serializedSettings = new SerializedObject(settingsObject);

        ArrayGUI(serializedSettings, "_pairsList");

        void ArrayGUI(SerializedObject obj, string name)
        {
            int arraySize = obj.FindProperty(name + ".Array.size").intValue;
            EditorGUI.indentLevel = 3;
            int sizeValue = EditorGUILayout.IntField("Size", arraySize);
            if (sizeValue != arraySize)
            {
                obj.FindProperty(name + ".Array.size").intValue = sizeValue;
                arraySize = sizeValue;
            }

            for (int i = 0; i < arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var element = obj.FindProperty(string.Format("{0}.Array.data[{1}]", name, i));
                var prop = element.FindPropertyRelative("_gameState");
                var prop2 = element.FindPropertyRelative("_ui");

                EditorGUILayout.PropertyField(prop);
                EditorGUILayout.PropertyField(prop2);


                EditorGUILayout.EndHorizontal();

            }

            
            obj.ApplyModifiedProperties();
            obj.Update();
        }

        //sp.Next(true); // skip generic field
        //sp.Next(true); // advance to array size field

        //// Get the array size
        //arrayLength = sp.intValue;

        //List<int> values = new List<int>(arrayLength);
        //int lastIndex = arrayLength - 1;
        //for (int i = 0; i < arrayLength; i++)
        //{
        //    values.Add(sp.intValue); // copy the value to the list
        //    if (i < lastIndex) sp.Next(false); // advance without drilling into children
        //}


        settingsObject.GetType().GetField("_pairsList", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(settingsObject, list);

    }
    private static (GameStateName, UIName) DrawDictElement(GameStateName gameStateName, UIName uIName)
    {
        EditorGUILayout.BeginHorizontal();
        gameStateName = (GameStateName)EditorGUILayout.EnumPopup(gameStateName);
        uIName = (UIName)EditorGUILayout.EnumPopup(uIName);
        EditorGUILayout.EndHorizontal();
        return (gameStateName, uIName);
    }
    [Serializable]
    private class EnumData
    {
        public string[] names;
    }
}

