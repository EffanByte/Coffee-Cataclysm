using UnityEngine;

public enum PlayerType
{
    Attacker,
    Tank,
    Melee,
    Healer,
}

public abstract class ClassManager : MonoBehaviour, IPlayer  
{
    protected float speed;
    protected int maxHealth;
    protected PlayerType _playerType;
    protected GameInput gameInput;



    public ClassManager(PlayerType playerType)
    {
        _playerType = playerType;
    }

    protected virtual void Start()
    {
        gameInput = GetComponentInChildren<GameInput>();
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
