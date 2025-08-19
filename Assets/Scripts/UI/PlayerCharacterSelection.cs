using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterSelection : MonoBehaviour
{
    [SerializeField] private GameObject playerRanged;
    [SerializeField] private GameObject playerMelee;
    [SerializeField] private GameObject playerTank;
    [SerializeField] private GameObject playerHealer;

    [SerializeField] private Button playerRangedButton;
    [SerializeField] private Button playerMeleeButton;
    [SerializeField] private Button playerTankButton;
    [SerializeField] private Button playerHealerButton;

    private void Awake()
    {
        playerRangedButton.onClick.AddListener(() =>
        {
            PlayroomManager.Instance.SetPlayerPrefab(playerRanged);
            PlayroomManager.Instance.GetPlayroomKit().OnPlayerJoin(PlayroomManager.Instance.SpawnPlayer);
            Hide();
        });
        playerMeleeButton.onClick.AddListener(() =>
        {
            PlayroomManager.Instance.SetPlayerPrefab(playerMelee);
            PlayroomManager.Instance.GetPlayroomKit().OnPlayerJoin(PlayroomManager.Instance.SpawnPlayer);
            Hide();

        });
        playerTankButton.onClick.AddListener(() =>
        {
            PlayroomManager.Instance.SetPlayerPrefab(playerTank);
            PlayroomManager.Instance.GetPlayroomKit().OnPlayerJoin(PlayroomManager.Instance.SpawnPlayer);
            Hide();
        });
        playerHealerButton.onClick.AddListener(() =>
        {
            PlayroomManager.Instance.SetPlayerPrefab(playerHealer);
            PlayroomManager.Instance.GetPlayroomKit().OnPlayerJoin(PlayroomManager.Instance.SpawnPlayer);
            Hide();
        });
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
