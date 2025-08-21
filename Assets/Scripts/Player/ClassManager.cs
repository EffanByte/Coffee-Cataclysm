using UnityEngine;

public enum PlayerType
{
    Attacker,
    Tank,
    Melee,
    Healer,
}
public enum PlayerAttackState
{
    Melee,
    Shoot,
    UseItem,
}

public abstract class ClassManager : MonoBehaviour, IPlayer
{
    protected float speed;
    [SerializeField] protected int maxHealth = 5;
    protected int currentHealth;
    protected PlayerType _playerType;
    protected PlayerAttackState _playerAttackState;
    protected GameInput gameInput;
    protected Animator animator;
    protected PlayerAnimatonController playerAnimatonController;


    public ClassManager(PlayerType playerType)
    {
        _playerType = playerType;
    }

    protected virtual void Start()
    {
        gameInput = GetComponentInChildren<GameInput>();
        animator = GetComponentInChildren<Animator>();
        playerAnimatonController = GetComponent<PlayerAnimatonController>();

        currentHealth = maxHealth;
        UIManager.Instance.UpdatePlayerHealthUI(currentHealth, maxHealth);
    }

    public virtual void Attack()
    {

    }

    public virtual void Damage()
    {
        Debug.Log("Player is being Damage");
        if (currentHealth > 0)
        {
            currentHealth--;
            UIManager.Instance.UpdatePlayerHealthUI(currentHealth, maxHealth);
        }
        else if (currentHealth <= 0)
        {
            //Die Command here
        }
    }


    public virtual void Health()
    {

    }
    public virtual void UseAbility()
    {

    }

}
