using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
// Register a SettingsProvider using IMGUI for the drawing framework:
static class StateMachineSettingsRegister
{
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

                    var settingsObject = StateMachineSettings.GetSettings();
                    EditorUtility.SetDirty(settingsObject);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
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
        var values = Enum.GetValues(typeof(T));
        var result = new List<string>();
        foreach (var value in values)
        {
            result.Add(value.ToString());
        }
        return result;
    }
    private static void SaveEnums<T>(List<string> names)
    {

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
        var packageDirectoryPath = "Packages/com.danqa1337.statemachine/Scripts/";
        var assetsDirectoryPath = "Assets/StateMachine/Scripts/";
        var fileName = typeof(T).ToString() + ".cs";
        var finalPath = "";


        if (Directory.Exists(packageDirectoryPath))
        {
            finalPath = packageDirectoryPath + fileName;
        }
        else
        {
            finalPath = assetsDirectoryPath + fileName;
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
        var gameStateValues = Enum.GetValues(typeof(GameStateName)) as GameStateName[];

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("GameState");
        EditorGUILayout.LabelField("UI");


        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < list.Count; i++)
        {
            var input = DrawDictElement(list[i].GameState, list[i].UIName);
            if(!list.Any(t => t.GameState == input.Item1) || list[i].GameState == input.Item1)
            {
                list[i] = new UiNameInput(input.Item1, input.Item2);
            }
        }
        EditorGUILayout.BeginHorizontal();
        if (list.Count < gameStateValues.Length && EditorGUILayout.LinkButton("Add"))
        {
            var state = gameStateValues.FirstOrDefault(n => !list.Any(l => l.GameState == n));
            var ui = (Enum.GetValues(typeof(UIName)) as UIName[]).FirstOrDefault(u => u.ToString() == state.ToString());
            list.Add(new UiNameInput(state ,  ui));
        }
        if (list.Count > 0 && EditorGUILayout.LinkButton("Remove"))
        {
            list.RemoveAt(list.Count - 1);
        }
        EditorGUILayout.EndHorizontal();
        
        var settingsSer = new SerializedObject(settingsObject);


        settingsObject.GetType().GetField("_pairsList", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(settingsObject, list);
        settingsSer.ApplyModifiedProperties();

    }
    private static (GameStateName, UIName) DrawDictElement(GameStateName gameStateName, UIName uIName)
    {
        EditorGUILayout.BeginHorizontal();
        gameStateName = (GameStateName)EditorGUILayout.EnumPopup(gameStateName);
        uIName = (UIName)EditorGUILayout.EnumPopup(uIName);
        EditorGUILayout.EndHorizontal();
        return (gameStateName, uIName);
    }
    
}

