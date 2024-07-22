using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameStateManager>();
            }
            return _instance;
        }

    }
    public List<string> GameStateNames = new List<string>();
    public static event Action<GameStateName> GameStateChanged;

    public static event Action<GameStateName, GameStateName> GameStateChangedFromTo;

    private Stack<GameStateName> _history = new Stack<GameStateName>();
    [SerializeField] private GameStateName _currentGameState;

    public static GameStateName CurrentGameState { get => instance._currentGameState; }

    public void LoadEnums()
    {
        var values = Enum.GetValues(typeof(GameStateName));
        GameStateNames = new List<string>();
        foreach (var value in values)
        {
            GameStateNames.Add(value.ToString());
        }
    }
    public void SaveEnums()
    {
#if UNITY_EDITOR
        var names = new List<string>();

        var text = "public enum GameStateName\r\n{";
        for (int i = 0; i < GameStateNames.Count; i++)
        {
            if (!names.Contains(GameStateNames[i]))
            {
                names.Add(GameStateNames[i]);
            }
        }

        foreach (var item in names)
        {
            text += item + ",";
        }
        text += "}";
        File.WriteAllText("Packages/com.danqa1337.statemachine/Scripts/" + "GameStateName.cs", text);
        AssetDatabase.Refresh();
#endif
    }
    public static void ChangeGameState(GameStateName gameState)
    {
        instance._history.Push(CurrentGameState);
        SetAppState(gameState);
    }

    public static void Back()
    {
        SetAppState(instance._history.Pop());
    }

    private static void SetAppState(GameStateName gameState)
    {
        var oldState = instance._currentGameState;
        instance._currentGameState = gameState;
        Debug.Log("AppStateChanged to " + gameState);
        GameStateChanged?.Invoke(gameState);
        GameStateChangedFromTo?.Invoke(oldState, gameState);
    }
}
