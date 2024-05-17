using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;




public class UiManager : Singleton<UiManager>
{
    private UiCanvas[] _uiCanvases;
    public List<string> UINames;
    public List<UiNameInput> UIPairs;
    private Dictionary<GameStateName, UIName> _uiDict;

    public void LoadEnums()
    {
        var values = Enum.GetValues(typeof(UIName));
        UINames = new List<string>();
        foreach (var value in values)
        {
            UINames.Add(value.ToString());
        }
    }
    public void SaveEnums()
    {

        var text = "public enum UIName\r\n{";
        foreach (var item in UINames)
        {
            text += item + ",";
        }
        text += "}";
        File.WriteAllText("Packages/com.danqa1337.statemachine/" + "UIName.cs", text);
        AssetDatabase.Refresh();
    }

    private void OnEnable()
    {
        GameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.GameStateChanged -= OnGameStateChanged;
    }
    private void Awake()
    {
        _uiDict = new Dictionary<GameStateName, UIName>();
        foreach (var item in UIPairs)
        {
            _uiDict.Add(item.GameState, item.UIName);
        }
        _uiCanvases = GetComponentsInChildren<UiCanvas>();
    }

    public static void ShowUI(UIName uIName)
    {
        Debug.Log("Showing UI " + uIName);
        instance.HidAll();
        
        var canvas = instance.GetCanvas(uIName);
        canvas.Show();
    }

    private void HidAll()
    {
        foreach (var item in _uiCanvases)
        {
            item.Hide();
        }
    }

    private UiCanvas GetCanvas(UIName uIName)
    {
        var canvas = _uiCanvases.FirstOrDefault(u => u.UIName == uIName);
        if (canvas != null)
        {
            return canvas;
        }
        else
        {
            throw new System.Exception("Canvas not found: " + uIName);
        }
    }


    public void OnGameStateChanged(GameStateName gameState)
    {
        
    }
}
[Serializable]
public struct UiNameInput
{
    public GameStateName GameState;
    public UIName UIName;
}
