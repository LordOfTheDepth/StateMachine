using System;
using System.Collections.Generic;
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
    public static event Action<GameStateName> GameStateChanged;

    public static event Action<GameStateName, GameStateName> GameStateChangedFromTo;

    private Stack<GameStateName> _history = new Stack<GameStateName>();
    [SerializeField] private GameStateName _currentGameState;

    public static GameStateName CurrentGameState { get => instance._currentGameState; }

    public static void ChangeGameState(GameStateName gameState)
    {
        instance._history.Push(CurrentGameState);
        SetAppState(gameState);
    }
    private void Start()
    {
        ChangeGameState(_currentGameState);
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
