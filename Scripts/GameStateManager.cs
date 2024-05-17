using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class GameStateManager : Singleton<GameStateManager>
{
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
        var text = "public enum GameStateName\r\n{";
        foreach (var item in GameStateNames)
        {
            text += item + ",";
        }
        text += "}";
        File.WriteAllText("Packages/com.danqa1337.statemachine/" + "GameStateName.cs", text);
        AssetDatabase.Refresh();
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
