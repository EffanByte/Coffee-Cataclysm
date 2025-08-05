using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public interface IPlayer
{
    public  void Damage();
    public void UseAbility();
    public void Attack();
    public void Movement();
    public void Health();
}
