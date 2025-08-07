using UnityEngine;

public class PlayerWeaponActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;

    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        GameStateManager.Instance.OnStateOutSide += GameStateMananger_OnStateOutSide;
        GameStateManager.Instance.OnStateGameBar += GameStateManager_OnStateGameBar;
    }

    private void GameStateManager_OnStateGameBar(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameStateMananger_OnStateOutSide(object sender, System.EventArgs e)
    {
        Show();
    }

    public void Show()
    {
        foreach (var weapon in weapons)
        {
            weapon.SetActive(true);
        }
    }

    public void Hide()
    {
        foreach (var weapon in weapons)
        {
            weapon.SetActive(false);
        }
    }
}
