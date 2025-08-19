using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutsideCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orePoints;
    [SerializeField] private Image xpProgressionBar;
    private void Start()
    {
        UIManager.Instance.SetOrePointText(orePoints); 
        UIManager.Instance.SetXpProgressionBar(xpProgressionBar);
    }
}
