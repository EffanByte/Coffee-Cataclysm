using UnityEngine;

public enum PlayerType
{
    Attacker,
    Tank,
    Melee,
    Healer,
}

public abstract class ClassManager : IPlayer
{
    protected float speed;
    protected PlayerType _playerType;

    public ClassManager(PlayerType playerType)
    {
        _playerType = playerType;
    }

    public virtual void Attack()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Damage()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Health()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Movement()
    {
        Debug.Log("Moving");
    }

    public virtual void UseAbility()
    {
        throw new System.NotImplementedException();
    }
}
