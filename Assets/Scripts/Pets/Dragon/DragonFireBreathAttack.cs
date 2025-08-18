using UnityEngine;

public class DragonFireBreathAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Dragon attacking enemy");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Dragon attacking enemyy");
        }
    }

}
