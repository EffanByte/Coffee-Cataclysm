using UnityHFSM;

public class GameBar
{
    public State<GameSceneState> state;

    public GameBar()
    {
        state = new State<GameSceneState>(onEnter: state => { });
    }
}

