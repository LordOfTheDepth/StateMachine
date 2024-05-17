using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
[CustomEditor(typeof(UiManager))]
public class UIManagerEditor : Editor
{
    private void OnEnable()
    {
        var UiManager = target as UiManager;
        UiManager.LoadEnums();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var UiManager = target as UiManager;

        if (GUILayout.Button("Save"))
        {
            UiManager.SaveEnums();
        }
    }

}
[CustomEditor(typeof(GameStateManager))]
public class GameStateManagerEditor : Editor
{
    private void OnEnable()
    {
        var manager = target as GameStateManager;
        manager.LoadEnums();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var manager = target as GameStateManager;

        if (GUILayout.Button("Save"))
        {
            manager.SaveEnums();
        }
    }

}

