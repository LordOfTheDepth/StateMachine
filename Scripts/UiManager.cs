using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;




public class UiManager : MonoBehaviour
{
    private static UiManager _instance;
    public static UiManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UiManager>();
            }
            return _instance;
        }

    }
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
        var names = new List<string>();

        var text = "public enum UIName\r\n{";
        for (int i = 0; i < UINames.Count; i++)
        {
            if (!names.Contains(UINames[i]))
            {
                names.Add(UINames[i]);
            }
        }

        foreach (var item in names)
        {
            text += item + ",";
        }
        text += "}";
        File.WriteAllText("Packages/com.danqa1337.statemachine/Scripts/" + "UIName.cs", text);
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
        if(_uiDict.ContainsKey(gameState))
        {
            ShowUI(_uiDict[gameState]);
        }
    }

    public void GenerateScripts()
    {
        
    }
}

[Serializable]
public struct UiNameInput
{
    public GameStateName GameState;
    public UIName UIName;
}
