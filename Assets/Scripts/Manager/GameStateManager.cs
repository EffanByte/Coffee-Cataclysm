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
    public event EventHandler OnStateOutSide;
    public event EventHandler OnStateGameBar;

    private StateMachine<GameSceneState> fsm;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }
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
        OnStateOutSide?.Invoke(this,EventArgs.Empty);
        SceneManager.LoadScene(SceneIndex);
    }

    public void GoToGameBar(int SceneIndex)
    {
        fsm.Trigger("GameBar");
        currentState = GameSceneState.GameBar;
        OnStateGameBar?.Invoke(this, EventArgs.Empty);
        SceneManager.LoadScene(SceneIndex);
    }
}
