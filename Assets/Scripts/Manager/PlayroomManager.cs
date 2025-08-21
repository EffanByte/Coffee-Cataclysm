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
    // Tracks order players arrived in the Outside scene. Earliest stays host until they leave.
    private Dictionary<string, bool> outsideQueue = new Dictionary<string, bool>();

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
                myPlayer.SetState("rotation", localPlayerData.playerObject.transform.rotation);

                // Apply remote positions only when both players are in the same game state/scene
                foreach (var entry in Players)
                {
                    if (entry.Key.id == myPlayer.id)
                        continue;

                    if (entry.Value.gameState != localPlayerData.gameState)
                        continue;

                    GameObject playerObject = entry.Value.playerObject;
                    playerObject.transform.position = entry.Key.GetState<Vector3>("position");
                    playerObject.transform.rotation = entry.Key.GetState<Quaternion>("rotation");
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
            _playroomKit.RpcRegister("SyncPlayerAnim", HandleSyncPlayerAnim);
            _playroomKit.RpcRegister("HandleAnimChange", HandleAnimChange);
            _playroomKit.RpcRegister("HandleActionAnim", HandleActionAnim);
            _playroomKit.RpcRegister("HandleEnemySpawn", HandleEnemySpawn);
            _playroomKit.RpcRegister("RequestEnemySnapshot", HandleRequestEnemySnapshot);
            _playroomKit.RpcRegister("ApplyEnemySnapshot", HandleApplyEnemySnapshot);
        });
    }

    private void HandleEnemySpawn(string data, string sender)
    {
        // only apply outside
        if (gameState == null || gameState.currentState != GameSceneState.OutSide)
            return;

        // data: "<prefabIndex>|?|<x>|<y>|<z>"
        var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
        if (parts.Length != 4)
        {
            Debug.LogError($"Invalid HandleEnemySpawn payload: {data}");
            return;
        }
        if (!int.TryParse(parts[0], out int prefabIndex)) prefabIndex = 0;
        float.TryParse(parts[1], out float x);
        float.TryParse(parts[2], out float y);
        float.TryParse(parts[3], out float z);

        Vector3 pos = new Vector3(x, y, z);
        var enemy = EnemyPoolHandler.Instance != null ? EnemyPoolHandler.Instance.GetNPC(prefabIndex) : null;
        if (enemy != null)
        {
            enemy.transform.position = pos;
        }
    }

    private void HandleRequestEnemySnapshot(string data, string sender)
    {
        // Only host should respond with snapshot; determine host by queue[0]
        var queue = GetOutsideQueue();
        if (queue.Count == 0 || queue[_playroomKit.MyPlayer().id] != true)
            return;

        if (gameState == null || gameState.currentState != GameSceneState.OutSide)
            return;

        // Build snapshot: semicolon-separated entries: idx,x,y,z
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var metas = FindObjectsOfType<EnemyMeta>();
        for (int i = 0; i < metas.Length; i++)
        {
            var m = metas[i];
            if (!m.gameObject.activeInHierarchy) continue;
            Vector3 p = m.transform.position;
            if (sb.Length > 0) sb.Append(";");
            sb.Append(m.PrefabIndex).Append("|?|").Append(p.x).Append("|?|").Append(p.y).Append("|?|").Append(p.z);
        }

        // Send snapshot addressed to requesterId; broadcast to OTHERS, receivers will filter
        string payload = sender + "#" + sb.ToString();
        _playroomKit.RpcCall("ApplyEnemySnapshot", payload, PlayroomKit.RpcMode.OTHERS);
    }

    private void HandleApplyEnemySnapshot(string data, string sender)
    {
        if (gameState == null || gameState.currentState != GameSceneState.OutSide)
            return;

        if (string.IsNullOrEmpty(data)) return;

        int hashIdx = data.IndexOf('#');
        if (hashIdx < 0) return;
        string targetId = data.Substring(0, hashIdx);
        string snapshot = data.Substring(hashIdx + 1);

        // Only apply if this client is the intended recipient
        if (_playroomKit.MyPlayer().id != targetId)
            return;

        // Clear current active enemies before applying snapshot to avoid duplicates
        var existing = FindObjectsOfType<EnemyMeta>();
        foreach (var m in existing)
        {
            if (m.gameObject.activeInHierarchy)
            {
                m.gameObject.SetActive(false);
            }
        }

        var entries = snapshot.Split(';');
        foreach (var entry in entries)
        {
            var parts = entry.Split(new[] { "|?|" }, StringSplitOptions.None);
            if (parts.Length != 4) continue;
            if (!int.TryParse(parts[0], out int idx)) idx = 0;
            float.TryParse(parts[1], out float x);
            float.TryParse(parts[2], out float y);
            float.TryParse(parts[3], out float z);
            Vector3 pos = new Vector3(x, y, z);
            var enemy = EnemyPoolHandler.Instance != null ? EnemyPoolHandler.Instance.GetNPC(idx) : null;
            if (enemy != null)
            {
                enemy.transform.position = pos;
            }
        }
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

                // Update visibility based on current game state
                if (gameState.currentState == newState)
                    player.Value.playerObject.SetActive(true);
                else
                    player.Value.playerObject.SetActive(false);

                UpdateOutsideQueue(sender, newState);

                Debug.Log("Outside queue updated in HandleSyncPlayerStateChange: " + string.Join(", ", outsideQueue));
                break;
            }
        }
    }

    private void HandleSyncPlayerAnim(string data, string sender)
    {
        // data format: "<param>|?|<value>"; example: "IsWalking|?|True"
        var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
        if (parts.Length != 2)
        {
            Debug.LogError($"Invalid SyncPlayerAnim payload: {data}");
            return;
        }

        string param = parts[0];
        string value = parts[1];

        foreach (var entry in Players)
        {
            if (entry.Key.id != sender)
                continue;

            // Only apply if the remote player's scene matches this client's current scene
            if (gameState == null || entry.Value.gameState != gameState.currentState)
                return;

            // Do not apply walking toggles while an action animation is active
            var pac = entry.Value.playerObject.GetComponent<PlayerAnimatonController>();
            if (pac != null)
            {
                string current = pac.GetCurrentAnimation();
                if (IsActionAnimation(current))
                    return;
            }

            var animator = entry.Value.playerObject.GetComponentInChildren<Animator>();
            if (animator == null)
                return;

            // Currently only syncing bool params; extend as needed
            if (param == "IsWalking")
            {
                bool boolValue;
                bool.TryParse(value, out boolValue);
                animator.SetBool("IsWalking", boolValue);
            }
            return;
        }
    }

    private void HandleAnimChange(string animation, string sender)
    {
        foreach (var entry in Players)
        {
            if (entry.Key.id != sender)
                continue;

            if (gameState == null || entry.Value.gameState != gameState.currentState)
                return;

            var pac = entry.Value.playerObject.GetComponent<PlayerAnimatonController>();
            if (pac != null)
            {
                // Route action animations to action flow (duration-aware)
                if (IsActionAnimation(animation))
                {
                    float duration = pac.GetClipLength(animation);
                    pac.PlayActionRemote(animation, duration);
                }
                else
                {
                    // Use controller API so it updates its internal state and avoids flicker
                    pac.ChangeAnimation(animation);
                }
                return;
            }

            // Fallback: direct crossfade
            var animator = entry.Value.playerObject.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.CrossFade(animation, 0.2f);
            }
            return;
        }
    }

    private void HandleActionAnim(string data, string sender)
    {
        // data format: "<name>|?|<duration>"
        var parts = data.Split(new[] { "|?|" }, StringSplitOptions.None);
        if (parts.Length != 2)
        {
            Debug.LogError($"Invalid HandleActionAnim payload: {data}");
            return;
        }

        string name = parts[0];
        if (!float.TryParse(parts[1], out float duration))
        {
            Debug.LogError($"Invalid duration in HandleActionAnim payload: {data}");
            return;
        }

        foreach (var entry in Players)
        {
            if (entry.Key.id != sender)
                continue;

            if (gameState == null || entry.Value.gameState != gameState.currentState)
                return;

            var pac = entry.Value.playerObject.GetComponent<PlayerAnimatonController>();
            if (pac != null)
            {
                pac.PlayActionRemote(name, duration);
                return;
            }
        }
    }

    private bool IsActionAnimation(string current)
    {
        if (string.IsNullOrEmpty(current)) return false;
        switch (current)
        {
            case "Shoot":
            case "Melee":
            case "Mage_Shoot":
            case "Hit_A":
            case "Hit_B":
                return true;
            default:
                return false;
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

    public void SyncPlayerAnimBool(string parameterName, bool value)
    {
        _playroomKit.RpcCall("SyncPlayerAnim", $"{parameterName}|?|{value}", PlayroomKit.RpcMode.ALL);
    }

    public Dictionary<string, bool> GetOutsideQueue()
    {
        return outsideQueue;
    }

    public void UpdateOutsideQueue(string playerId, GameSceneState newState)
    {
        if (newState == GameSceneState.OutSide)
        {
            bool hostExists = outsideQueue.ContainsValue(true); 
            outsideQueue[playerId] = !hostExists;               // true only if no host yet
        }
        else
            outsideQueue[playerId] = false;                     // leaving OutSide
        Debug.Log("Updated Outside Queue: " + string.Join(", ", outsideQueue));
    }
}
