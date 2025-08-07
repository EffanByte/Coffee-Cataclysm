using UnityEngine;
using Playroom;
using System.Collections.Generic;
using System;

public class PlayerData
{
    public PlayroomKit.Player player;
    public GameObject playerObject;
    public Player playerScript;
    public PlayerData(PlayroomKit.Player player, GameObject playerObject, Player playerScript)
    {
        this.player = player;
        this.playerObject = playerObject;
        this.playerScript = playerScript;
    }
}

public class PlayroomManager : MonoBehaviour
{

    bool spawned = false;
    public static PlayroomManager Instance { get; private set; }
    private PlayroomKit _playroomKit;
    public static Dictionary<PlayroomKit.Player, PlayerData> Players = new();

    [SerializeField] GameObject PlayerPrefab;

    void Awake()
    {
        _playroomKit = new PlayroomKit();
        Instance = this;
    }
    void Start()
    {
        InitializePlayroom();
    }
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (spawned)
        {
            var myPlayer = _playroomKit.MyPlayer();
            myPlayer.SetState("position", Players[myPlayer].playerObject.transform.position);

            foreach (var player in Players)
            {
                if (player.Key.id != myPlayer.id)
                {
                    GameObject playerObject = player.Value.playerObject;
                    playerObject.transform.position = player.Key.GetState<Vector3>("position");
                }
            }
        }
    }
    void InitializePlayroom()
    {
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 4,
        }, () =>
        {
            _playroomKit.OnPlayerJoin(SpawnPlayer);
            _playroomKit.RpcRegister("HandleHeldItem", HandleHeldItem);
        });
    }

    private void HandleHeldItem(string data, string sender)
    {
        Players.TryGetValue(_playroomKit.GetPlayer(sender), out PlayerData playerData);
        GameObject playerObject = playerData.playerObject;
        playerObject.GetComponentInChildren<HoldingManager>().PickUpItem((HeldItemType)System.Enum.Parse(typeof(HeldItemType), data));
    }
    void SpawnPlayer(PlayroomKit.Player player)
    {
        if (Players.ContainsKey(player))
            return; // Player already spawned, skip

        GameObject playerObject = Instantiate(PlayerPrefab, new Vector3(0, 1, 2), Quaternion.identity); // using default position for now   
        Player playerScript = playerObject.GetComponent<Player>();

        Players.Add(player, new PlayerData(player, playerObject, playerScript));
        spawned = true;
    }

    public PlayroomKit GetPlayroomKit()
    {
        return _playroomKit;
    }
}
