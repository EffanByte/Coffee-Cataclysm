using TMPro;
using UnityEngine;

public class OutsideCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    private void Start()
    {
        UIManager.Instance.SetScoreText(score);
    }
}
