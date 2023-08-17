using Carp.Process;
using UnityEngine;

public class CombatLocationProcess : LocationProcess
{
    public CombatLocationProcess(Team[] _teams) : base(_teams) {}

    public override void InvokeProcess()
    {
        Debug.Log("Enter Combat Location");

        for (int i = 0; i < teams.Length; i++)
        {
            for (int j = 0; j < teams[i].players.Count; j++)
            {
                teams[i].players[j].character.OnDeath += CheckForCombatComplete; //todo: disconnect?
            }
        }

        RecursiveRounds();
    }

    public void RecursiveRounds(AbstractProcess _lastRound = null, object _value = null, object _parameters = null)
    {
        if (_lastRound != null)
        {
            _lastRound.onComplete -= RecursiveRounds;
        } 

        CombatRoundProcess combatRoundProcess = new CombatRoundProcess(teams);
        combatRoundProcess.onComplete += RecursiveRounds;

        RequestProcess(combatRoundProcess);
    }

    public void CheckForCombatComplete(GameEventType _gameEventType)
    {
        int remaningTeams = 0;
        for (int i = 0; i < teams.Length; i++)
        {
            if (!IsTeamDead(teams[i]))
            {
                remaningTeams++;
            }
        }

        if (remaningTeams <= 1) 
        {
            onComplete?.Invoke(this, null, null);
        }
    }

    public bool IsTeamDead(Team _team)
    {
        for (int i = 0; i < _team.players.Count; i++)
        {
            if (!_team.players[i].character.isDead)
            {
                return false;
            }
        }

        return true;
    }
}
