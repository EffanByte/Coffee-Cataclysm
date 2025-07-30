using UnityHFSM;
using UnityEngine;


public class IdleState
{
    public State<PlayerState> state;

    public IdleState()
    {
        state = new State<PlayerState>(onEnter: state =>
        {
            //Debug.Log("Idle State");
        });
    }
}

