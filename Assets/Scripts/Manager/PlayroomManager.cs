using UnityEngine;
using Playroom;

public class PlayroomManager : MonoBehaviour
{

    public static PlayroomManager Instance { get; private set; }
    private PlayroomKit _playroomKit;
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
            print($"[Unity Log] isHost: {_playroomKit.IsHost()}");

        });
    }

    void spawnPlayer(PlayroomKit.Player player)
    {
    }
}
