using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityHFSM;

public enum GameSceneState
{
    GameBar,
    OutSide,
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameSceneState currentState;

    private StateMachine<GameSceneState> fsm;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        fsm = new StateMachine<GameSceneState>();

        var gamebar = new GameBar();
        var outSide = new OutSide();

        fsm.AddState(GameSceneState.GameBar, gamebar.state);
        fsm.AddState(GameSceneState.OutSide, outSide.state);

        fsm.AddTriggerTransition(trigger: "OutSide", from: GameSceneState.GameBar, to: GameSceneState.OutSide);
        fsm.AddTriggerTransition(trigger: "GameBar", from: GameSceneState.OutSide, to: GameSceneState.GameBar);

        fsm.SetStartState(GameSceneState.GameBar);
        fsm.Init();
        currentState = GameSceneState.GameBar;

    }

    public void GoToOutSide(int SceneIndex)
    {
        fsm.Trigger("OutSide");
        currentState = GameSceneState.OutSide;
        SceneManager.LoadScene(SceneIndex);
    }

    public void GoToGameBar(int SceneIndex)
    {
        fsm.Trigger("GameBar");
        currentState = GameSceneState.GameBar;
        SceneManager.LoadScene(SceneIndex);
    }
}
