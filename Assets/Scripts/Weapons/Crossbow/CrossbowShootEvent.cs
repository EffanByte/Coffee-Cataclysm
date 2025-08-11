using UnityEngine;

public class CrossbowShootEvent : MonoBehaviour
{
    public void Shoot()
    {
        Crossbow.Instance.ShootArrow();
    }
}
