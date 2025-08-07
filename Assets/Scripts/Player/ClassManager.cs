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
    protected int maxHealth;
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
    }

    public virtual void Attack()
    {

    }

    public virtual void Damage()
    {

    }


    public virtual void Health()
    {

    }
    public virtual void UseAbility()
    {

    }
}
