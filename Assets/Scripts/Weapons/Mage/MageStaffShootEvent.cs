using UnityEngine;

public class MageStaffShootEvent : MonoBehaviour
{

    public void Shoot()
    {
        MageStaff.Instance.ShootMagic();
    }
}
