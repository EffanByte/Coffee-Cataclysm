using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Image playerHealthBarUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }



    public void UpdatePlayerHealthUI(int currentHealth, int maxHealth)
    {
        if (playerHealthBarUI != null)
        {
            playerHealthBarUI.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void SetPlayerHealthBar(Image image)
    {
        playerHealthBarUI = image;
    }

}
