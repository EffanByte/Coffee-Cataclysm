using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public event EventHandler OnXpBarLevelUp;

    [SerializeField] private Image playerHealthBarUI;
    [SerializeField] private TextMeshProUGUI orePointText;
    [SerializeField] private Image xpProgressionBar;

    private int currentLevel = 1;
    private int scoreNeeded = 10;
    private bool playerSelected;
    private int score;
    private int orePoints;
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
    public void SetXpProgressionBar(Image image)
    {
        xpProgressionBar = image;
    }

    public void ScoreUp()
    {
        score++;
        orePoints++;
        orePointText.text =  orePoints.ToString();
        
        float progress = (float)score / scoreNeeded;
        xpProgressionBar.fillAmount = Mathf.Clamp01(progress);

        if (score >= scoreNeeded)
        {
            LevelUp();
            OnXpBarLevelUp?.Invoke(this,EventArgs.Empty);
        }
    }
    private void LevelUp()
    {
        score -= scoreNeeded;

        currentLevel++;
        scoreNeeded = currentLevel * 10;

        //scoreText.text = "Score : " + score;
        xpProgressionBar.fillAmount = (float)score / scoreNeeded;
    }

    public void SetOrePointText(TextMeshProUGUI text)
    {
        orePointText = text;
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
