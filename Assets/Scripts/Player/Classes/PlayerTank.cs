using UnityEngine;

public class PlayerTank : ClassManager
{

    public PlayerTank(PlayerType playerType) : base(playerType)
    {
        Debug.Log("I am a " + playerType);

    }
    protected override void Start()
    {
        base.Start();
        gameInput.OnAttackPlayer += GameInput_OnAttackPlayer;
    }

    private void GameInput_OnAttackPlayer(object sender, System.EventArgs e)
    {
        if (GameStateManager.Instance.currentState == GameSceneState.OutSide)
        {
            Attack();
        }
        else
        {
            Debug.Log("Chill dude u are inside Your Bar Go OutSide if u wanna fight");
        }
    }

    public override void Attack()
    {
        base.Attack();
        Debug.Log("Player Tank Attacking");
    }

    public override void Damage()
    {
        base.Damage();
    }
    public override void Health()
    {
        base.Health();
    }

    public override void UseAbility()
    {
        base.UseAbility();
    }



}
