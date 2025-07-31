// CoffeeBlendItem.cs
using UnityEngine;

public class CoffeeBlendItem : MonoBehaviour
{
    public HeldItemType blendType = HeldItemType.None;

    public void SetBlendType(HeldItemType type)
    {
        blendType = type;
    }
}
