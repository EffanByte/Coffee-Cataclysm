using UnityEngine;
using Playroom;
using System.Collections.Generic;
using System;

public class PlayerData
{
    public PlayroomKit.Player player;
    public GameObject playerObject;
    public Player playerScript;
    public HeldItemType HeldItem;
    public GameSceneState gameState = GameSceneState.GameBar;
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

    public bool spawned = false;
    public static PlayroomManager Instance { get; private set; }
    private PlayroomKit _playroomKit;
    public static Dictionary<PlayroomKit.Player, PlayerData> Players = new();
    private GameStateManager gameState;
    [SerializeField] GameObject PlayerPrefab;

    void Awake()
    {
        _playroomKit = new PlayroomKit();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        InitializePlayroom();
        gameState = GameStateManager.Instance;
    }
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (spawned)
        {
            var myPlayer = _playroomKit.MyPlayer();
            if (Players.TryGetValue(myPlayer, out PlayerData localPlayerData))
            {
                // Broadcast only position
                myPlayer.SetState("position", localPlayerData.playerObject.transform.position);

                // Apply remote positions only when both players are in the same game state/scene
                foreach (var entry in Players)
                {
                    if (entry.Key.id == myPlayer.id)
                        continue;

                    if (entry.Value.gameState != localPlayerData.gameState)
                        continue;

                    GameObject playerObject = entry.Value.playerObject;
                    playerObject.transform.position = entry.Key.GetState<Vector3>("position");
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
            _playroomKit.RpcRegister("HandleDropItem", HandleDropItem);
            _playroomKit.RpcRegister("HandleBlendCoffee", HandleHeldItem);
            _playroomKit.RpcRegister("HandleReceiveBlend", HandleReceiveBlend);
            _playroomKit.RpcRegister("HandleInsertBean", HandleInsertBean);
            _playroomKit.RpcRegister("HandleReceiveOrder", HandleReceiveOrder);
            _playroomKit.RpcRegister("HandleNewOrder", HandleNewOrder);
            _playroomKit.RpcRegister("SyncPlayerStateChange", HandleSyncPlayerStateChange);
        });
    }

    private void HandleSyncPlayerStateChange(string data, string sender)
    {

        GameSceneState newState = (GameSceneState)Enum.Parse(typeof(GameSceneState), data);

        // Find the player and update their state
        foreach (var player in Players)
        {
            if (player.Key.id == sender)
            {
                player.Value.gameState = newState;
                
                // Update visibility based on current game state (guard against early calls)
                if (gameState != null && gameState.currentState == newState)
                    player.Value.playerObject.SetActive(true);
                else
                    player.Value.playerObject.SetActive(false);
                break;
            }
        }
    }
    private void HandleNewOrder(string data, string sender)
    {
        if (gameState.currentState == GameSceneState.GameBar)
        {
            var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                Debug.LogError($"Invalid HandleNewOrder payload: {data}");
                return;
            }

            string customerId = parts[0];
            HeldItemType orderType = (HeldItemType)Enum.Parse(typeof(HeldItemType), parts[1]);

            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.SyncOrderFromRPC(customerId, orderType);

                AiBar aiBar = AiBar.GetByCustomerId(customerId);
                if (aiBar != null)
                {
                    GameObject orderBubble = OrderManager.Instance.CreateOrderBubble(aiBar.transform, orderType);

                    if (orderBubble != null)
                    {
                        aiBar.SetOrderBubble(orderBubble);
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not find AI customer with ID: {customerId}");
                }
            }
            else
            {
                Debug.LogWarning("OrderManager instance not found when syncing order");
            }
        }
    }
    private void HandleReceiveOrder(string data, string sender)
    {
        if (gameState.currentState == GameSceneState.GameBar)
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
    }
    private void HandleInsertBean(string data, string sender)
    {
        if (gameState.currentState == GameSceneState.GameBar)
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
    }

    private void HandleReceiveBlend(string data, string sender)
    {
        if (gameState.currentState == GameSceneState.GameBar)
        {
            var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
            string makerId = parts[0];
            HeldItemType blendType = (HeldItemType)Enum.Parse(typeof(HeldItemType), parts[1]);
            CoffeeMakerInteraction maker = CoffeeMakerInteraction.GetById(makerId);
            maker.insertedBeans.Clear();
            maker.isBlendReady = false;

            HandleHeldItem(blendType.ToString(), sender);
        }
    }
    private void HandleHeldItem(string data, string sender)
    {
        if (gameState.currentState == GameSceneState.GameBar)
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
    }
    private void HandleDropItem(string data, string sender)
    {
        if (gameState.currentState == GameSceneState.GameBar)
        {
            Players.TryGetValue(_playroomKit.GetPlayer(sender), out PlayerData playerData);
            GameObject playerObject = playerData.playerObject;
            playerData.HeldItem = HeldItemType.None;
            playerObject.GetComponent<HoldingManager>().DropItem();
        }
    }
    void SpawnPlayer(PlayroomKit.Player player)
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
