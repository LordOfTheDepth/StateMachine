using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UiManager : MonoBehaviour
{
    public StateMachineSettings settings;
    private UiCanvas[] _uiCanvases;
    private Dictionary<GameStateName, UIName> _uiDict;

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
        foreach (var item in settings.PairsList)
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
        if (_uiDict.ContainsKey(gameState))
        {
            ShowUI(_uiDict[gameState]);
        }
    }

}

[Serializable]
public struct UiNameInput
{
    private int _GS;
    private int _UI;
    public GameStateName GameState => (GameStateName)_GS;
    public UIName UIName => (UIName)_UI;

    public UiNameInput(GameStateName gameState, UIName uIName)
    {
        _GS = (int)gameState;
        _UI = (int)uIName;
    }
}