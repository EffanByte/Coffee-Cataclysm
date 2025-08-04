using UnityEngine;
using Playroom;
using System.Collections.Generic;

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

    public static PlayroomManager Instance { get; private set; }
    private PlayroomKit _playroomKit;
    public Dictionary<PlayroomKit.Player, PlayerData> Players = new();

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

    void InitializePlayroom()
    {
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 4,
        }, () =>
        {
            _playroomKit.OnPlayerJoin(spawnPlayer);
        });
    }

    void spawnPlayer(PlayroomKit.Player player)
    {
        GameObject playerObject = Instantiate(PlayerPrefab, new Vector3(0, 1, 2), Quaternion.identity); // using default position for now   
        Player playerScript = playerObject.GetComponent<Player>();

        Players.Add(player, new PlayerData(player, playerObject, playerScript));
        
        if (player.id != _playroomKit.MyPlayer().id)
            playerObject.GetComponentInChildren<GameInput>().gameObject.SetActive(false);
    }
}
