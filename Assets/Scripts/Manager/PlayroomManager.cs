using UnityEngine;
using Playroom;
using System.Collections.Generic;
using SimpleJSON;
using System;

public class PlayerData
{
    public PlayroomKit.Player player;
    public GameObject playerObject;
    public Player playerScript;
    public HeldItemType HeldItem;
    public PlayerData(PlayroomKit.Player player, GameObject playerObject, Player playerScript)
    {
        this.player = player;
        this.playerObject = playerObject;
        this.playerScript = playerScript;
        HeldItem = HeldItemType.None;
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
            _playroomKit.OnPlayerJoin(spawnPlayer);
            _playroomKit.RpcRegister("HandleHeldItem", HandleHeldItem);
            _playroomKit.RpcRegister("HandleDropItem", HandleDropItem);
            _playroomKit.RpcRegister("HandleBlendCoffee", HandleHeldItem);
            _playroomKit.RpcRegister("HandleReceiveBlend", HandleReceiveBlend);
            _playroomKit.RpcRegister("HandleInsertBean", HandleInsertBean);
            _playroomKit.RpcRegister("HandleReceiveOrder", HandleReceiveOrder);
        });
    }

    private void HandleReceiveOrder(string data, string sender)
    {
        var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
        string customerId = parts[0];
        HeldItemType item = (HeldItemType)Enum.Parse(typeof(HeldItemType), parts[1]);
        AiBar aiBar = AiBar.GetByCustomerId(customerId);
        if (aiBar != null)
        {
            aiBar.ReceiveOrder(item);
            Players.TryGetValue(_playroomKit.GetPlayer(sender), out PlayerData playerData);
            playerData.HeldItem = HeldItemType.None;
            playerData.playerObject.GetComponentInChildren<HeldItemVisualizer>().Hide();
        }
    }
    private void HandleInsertBean(string data, string sender)
    {
        var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
        string makerId = parts[0];
        HeldItemType bean = (HeldItemType)Enum.Parse(typeof(HeldItemType), parts[1]);
        var maker = CoffeeMakerInteraction.GetById(makerId);
        maker?.ProcessInsertBean(bean);
        Players.TryGetValue(_playroomKit.GetPlayer(sender), out PlayerData playerData);
        playerData.HeldItem = HeldItemType.None;
        playerData.playerObject.GetComponentInChildren<HeldItemVisualizer>().Hide();
    }

    private void HandleReceiveBlend(string data, string sender)
    {
        var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
        string makerId = parts[0];
        HeldItemType blendType = (HeldItemType)Enum.Parse(typeof(HeldItemType), parts[1]);
        CoffeeMakerInteraction maker = CoffeeMakerInteraction.GetById(makerId);
        maker.insertedBeans.Clear();
        maker.isBlendReady = false;

        HandleHeldItem(blendType.ToString(), sender);
    }
    private void HandleHeldItem(string data, string sender)
    {
        Players.TryGetValue(_playroomKit.GetPlayer(sender), out PlayerData playerData);
        GameObject playerObject = playerData.playerObject;
        HeldItemType item = (HeldItemType)Enum.Parse(typeof(HeldItemType), data);
        playerObject.GetComponentInChildren<HeldItemVisualizer>().Show(item);
        playerData.HeldItem = item;
        Debug.Log(playerData.HeldItem);
        playerObject.GetComponent<HoldingManager>().HeldItem = item;
        Debug.Log(playerObject.GetComponent<HoldingManager>().HeldItem);
    }

    private void HandleDropItem(string data, string sender)
    {
        Players.TryGetValue(_playroomKit.GetPlayer(sender), out PlayerData playerData);
        GameObject playerObject = playerData.playerObject;
        playerObject.GetComponentInChildren<HeldItemVisualizer>().Hide();
        playerData.HeldItem = HeldItemType.None;
    }
    void spawnPlayer(PlayroomKit.Player player)
    {
        GameObject playerObject;
        playerObject = Instantiate(PlayerPrefab, new Vector3(0, 1, 2), Quaternion.identity); // using default position for now   
        Player playerScript = playerObject.GetComponent<Player>();

        Players.Add(player, new PlayerData(player, playerObject, playerScript));
        spawned = true;
        if (_playroomKit.GetPlayer(player.id) == _playroomKit.MyPlayer())
        Debug.Log("My Player ID: " + player.id);
        else
        {
            Debug.Log("Other Player ID: " + player.id);
        }
    
    }

    public PlayroomKit GetPlayroomKit()
    {
        return _playroomKit;
    }
}
