

public class AITurnProcess : PlayerTurnProcess
{
    public AITurnProcess(Player _player) : base(_player)
    {

    }

    public override void InvokeProcess()
    {
        SetupPlayerTypes();


    }
}
