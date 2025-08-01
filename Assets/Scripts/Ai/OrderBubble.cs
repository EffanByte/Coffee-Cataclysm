using UnityEngine;
using UnityEngine.UI;

public class OrderBubble : MonoBehaviour

{
    [SerializeField] private Sprite brownBlendSprite;
    [SerializeField] private Sprite whiteBlendSprite;
    [SerializeField] private Sprite mixedBlendSprite;

    public void SetIcon(HeldItemType item)
    {
        Sprite selectedIcon = item switch
        {
            HeldItemType.CoffeeBlendBrown => brownBlendSprite,
            HeldItemType.CoffeeBlendWhite => whiteBlendSprite,
            HeldItemType.MixedCoffeeBlend => mixedBlendSprite,
            _ => null
        };
        if (selectedIcon != null)
        {
            Image image = GetComponentInChildren<Image>();
            if (image != null)
            {
                image.sprite = selectedIcon;
            }
            else
            {
                Debug.LogError("Image component not found on OrderBubble.");
            }
        }
        else
        {
            Debug.LogWarning("No icon set for the given item type: " + item);
        }
    }
}
