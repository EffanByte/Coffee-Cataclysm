using UnityEngine;

public class PlayerMelee : ClassManager
{

    public PlayerMelee(PlayerType playerType) : base(playerType)
    {
        Debug.Log("I am a " + playerType);
    }
    protected override void Start()
    {
        base.Start();
        gameInput.OnAttackPlayer += GameInput_OnAttackPlayer;

        if (_playerType == PlayerType.Melee)
            _playerAttackState = PlayerAttackState.Melee;
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
        playerAnimatonController.ChangeAnimation("Melee",1);
        Debug.Log("Player Melee Attacking");
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
