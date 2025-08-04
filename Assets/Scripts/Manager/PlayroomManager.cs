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
            _playroomKit.OnPlayerJoin(spawnPlayer);
        });
    }

    void spawnPlayer(PlayroomKit.Player player)
    {
        GameObject playerObject = Instantiate(PlayerPrefab, new Vector3(0, 1, 2), Quaternion.identity); // using default position for now   
        if (player.id != _playroomKit.MyPlayer().id)
        playerObject.GetComponentInChildren<GameInput>().gameObject.SetActive(false);
    }
}
