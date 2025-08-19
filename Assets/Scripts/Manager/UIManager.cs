using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Image playerHealthBarUI;
    [SerializeField] private TextMeshProUGUI scoreText;


    private bool playerSelected;
    private int score;

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

        score = 0;
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

    public void ScoreUp()
    {
        score++;
        scoreText.text = "Score : " + score.ToString();
    }

    public void SetScoreText(TextMeshProUGUI text)
    {
        scoreText = text;
    }

    public void SetPlayerSelectedBool(bool Selected)
    {
        playerSelected = Selected;
    }

    public bool GetPlayerSelectedBool()
    {
        return playerSelected;
    }
}
