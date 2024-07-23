using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGameState : MonoBehaviour
{
    public GameStateName GameStateName;
    public void Change()
    {
        GameStateManager.ChangeGameState(GameStateName);
    }
}
