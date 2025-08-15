using UnityEngine;

public class DragonFireBreathAttack : MonoBehaviour
{
    [SerializeField] ParticleSystem fireParticle;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            fireParticle.Play();
            Debug.Log("Dragon attacking enemy");
        }
    }

}
