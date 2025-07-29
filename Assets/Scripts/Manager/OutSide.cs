
using UnityHFSM;

public class OutSide
{
    public State<GameSceneState> state;

    public OutSide()
    {
        state = new State<GameSceneState>(onEnter: state => { });
    }
}

