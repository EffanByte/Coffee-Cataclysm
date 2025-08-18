using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityHFSM;
using Playroom;
using System.Collections.Generic;
using System.Numerics;
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

    private PlayroomKit _playroom;

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
        _playroom = PlayroomManager.Instance.GetPlayroomKit();

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
        fsm.Trigger("OutSide"); // Updates local game state
        OnStateOutSide?.Invoke(this, EventArgs.Empty);
        currentState = GameSceneState.OutSide;
        
        // Update ONLY local player's game state BEFORE scene change
        if (PlayroomManager.Players.TryGetValue(_playroom.MyPlayer(), out PlayerData myPlayerData_Out))
        {
            myPlayerData_Out.gameState = currentState;
            PlayroomManager.Instance.UpdateOutsideQueue(_playroom.MyPlayer().id, currentState);
        }
        
        // Now activate/deactivate players based on updated state
        foreach (KeyValuePair<PlayroomKit.Player, PlayerData> entry in PlayroomManager.Players)
        {
            if (entry.Value.gameState == currentState)
                entry.Value.playerObject.SetActive(true);
            else
                entry.Value.playerObject.SetActive(false);
        }

        // Reset local player position
        PlayroomManager.Players[_playroom.MyPlayer()].playerObject.transform.position = new UnityEngine.Vector3(0, 1, 2);
        
        // Load scene after state management
        SceneManager.LoadScene(SceneIndex);
    }

    public void GoToGameBar(int SceneIndex)
    {
        fsm.Trigger("GameBar");
        OnStateGameBar?.Invoke(this, EventArgs.Empty);
        currentState = GameSceneState.GameBar;
        
        // Update ONLY local player's game state BEFORE scene change
        if (PlayroomManager.Players.TryGetValue(_playroom.MyPlayer(), out PlayerData myPlayerData_In))
        {
            myPlayerData_In.gameState = currentState;
            PlayroomManager.Instance.UpdateOutsideQueue(_playroom.MyPlayer().id, currentState);
        }
        
        // Now activate/deactivate players based on updated state
        foreach (KeyValuePair<PlayroomKit.Player, PlayerData> entry in PlayroomManager.Players)
        {
            if (entry.Value.gameState == currentState)
                entry.Value.playerObject.SetActive(true);
            else
                entry.Value.playerObject.SetActive(false);
        }
        
        // Reset local player position
        PlayroomManager.Players[_playroom.MyPlayer()].playerObject.transform.position = new UnityEngine.Vector3(0, 1, 2);
        
        // Load scene after state management
        SceneManager.LoadScene(SceneIndex);
    }
}
