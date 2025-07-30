using UnityHFSM;
using UnityEngine;
public class MoveState
{
    public State<PlayerState> state;

    public MoveState()
    {
        state = new State<PlayerState>(onEnter: state =>
        {
            Debug.Log("Player Moving");
        });
    }

}
