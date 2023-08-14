using Carp.Process;
using UnityEngine;

public class CombatRoundProcess : GroupProcess
{
    public CombatRoundProcess(Team[] _teams) : base(true)
    {
        foreach (Team team in _teams)
        {
            foreach(Player player in team.players)
            {
                RequestProcess(new PlayerTurnProcess(player));
            }
        }
    }

    public override void InvokeProcess()
    {
        Debug.Log("Combat Round Started");
    }
}
