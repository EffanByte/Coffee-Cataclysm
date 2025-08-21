using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Collections;

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

    private Vector3 shopPanelCenter = Vector3.zero;

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
        float animationDuration = 2.0f;
        shopPanel.SetActive(true);
        shopPanel.GetComponent<RectTransform>().DOAnchorPosY(shopPanelCenter.y, animationDuration);
    }

    void ShopPanelHide()
    {
        float animationDuration = 2.0f;
        float panelOffScreen = 1200f;
        shopPanel.GetComponent<RectTransform>().DOAnchorPosY(panelOffScreen, animationDuration);
        StartCoroutine(WaitForHidingPanel());
    }

    IEnumerator WaitForHidingPanel()
    {
        yield return new WaitForSeconds(5.0f);
        shopPanel.SetActive(false);
    }
}
