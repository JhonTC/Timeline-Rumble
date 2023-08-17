using Carp.Process;
using UnityEngine;

public class CombatRoundProcess : GroupProcess
{
    Team[] teams;

    public CombatRoundProcess(Team[] _teams) : base(true)
    {
        teams = _teams;
    }

    public override void InvokeProcess()
    {
        Debug.Log("Combat Round Started");

        foreach (Team team in teams)
        {
            foreach (Player player in team.players)
            {
                RequestProcess(new PlayerTurnProcess(player));
            }
        }
    }
}
