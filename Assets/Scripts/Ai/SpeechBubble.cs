using UnityEngine;
using UnityEngine.UI;

public class OrderBubble : MonoBehaviour

{
    [SerializeField] private Image brownBlendSprite;
    [SerializeField] private Image whiteBlendSprite;
    [SerializeField] private Image mixedBlendSprite;

    public void SetIcon(HeldItemType item)
    {
        Image selectedIcon = item switch
        {
            HeldItemType.CoffeeBlendBrown => brownBlendSprite,
            HeldItemType.CoffeeBlendWhite => whiteBlendSprite,
            HeldItemType.MixedCoffeeBlend => mixedBlendSprite,
            _ => null
        };
    }

    void LateUpdate()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward; // Face the camera
    }
}
