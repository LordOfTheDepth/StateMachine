using UnityEngine;

public class ChangeGameState : MonoBehaviour
{
    public GameStateName GameStateName;
    public void Change()
    {
        GameStateManager.ChangeGameState(GameStateName);
    }
}
