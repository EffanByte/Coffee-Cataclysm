using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OutsideCanvas : MonoBehaviour
{
    public static event Action OnSlothActive;
    public static event Action OnDragonActive;
    public static event Action OnPlantActive;
    public static event Action OnSerpentActive;
    public static event Action OnSpiderActive;

    [SerializeField] private TextMeshProUGUI orePoints;
    [SerializeField] private Image xpProgressionBar;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private Button slothButton;
    [SerializeField] private Button dragonButton;
    [SerializeField] private Button spiderButton;
    [SerializeField] private Button plantButton;
    [SerializeField] private Button serpentButton;


    private void Awake()
    {
        slothButton.onClick.AddListener(() => { OnSlothActive?.Invoke(); ShopPanelHide(); });
        dragonButton.onClick.AddListener(() => { OnDragonActive?.Invoke(); ShopPanelHide(); });
        spiderButton.onClick.AddListener(() => { OnSpiderActive?.Invoke(); ShopPanelHide(); });
        plantButton.onClick.AddListener(() => { OnPlantActive?.Invoke(); ShopPanelHide(); });
        serpentButton.onClick.AddListener(() => { OnSerpentActive?.Invoke(); ShopPanelHide(); });
    }


    private void Start()
    {
        ShopPanelHide();
        UIManager.Instance.SetOrePointText(orePoints);
        UIManager.Instance.SetXpProgressionBar(xpProgressionBar);

        UIManager.Instance.OnXpBarLevelUp += UIManager_OnXpBarLevelUp;
    }

    private void UIManager_OnXpBarLevelUp(object sender, EventArgs e)
    {
        ShopPanelShow();
    }

    public void ShopPanelShow()
    {
        shopPanel.SetActive(true);
    }

    void ShopPanelHide()
    {
        shopPanel.SetActive(false);
    }
}
