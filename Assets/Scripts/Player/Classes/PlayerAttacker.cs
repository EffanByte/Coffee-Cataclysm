using UnityEngine;

public class PlayerAttacker : ClassManager
{
    public PlayerAttacker(PlayerType playerType) : base(playerType)
    {
        Debug.Log("I am a " + playerType);
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void Damage()
    {
        base.Damage();
    }

    public override void Health()
    {
        base.Health();
    }

    public override void Movement()
    {
        base.Movement();
    }

    public override void UseAbility()
    {
        base.UseAbility();
    }
}