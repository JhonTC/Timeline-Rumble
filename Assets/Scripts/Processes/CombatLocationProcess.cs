using UnityEngine;

public class CombatLocationProcess : LocationProcess
{
    public CombatLocationProcess(Team[] _teams) : base(_teams)
    {
        CombatRoundProcess combatRoundProcess = new CombatRoundProcess(teams);
        RequestProcess(combatRoundProcess);
    }

    public override void InvokeProcess()
    {
        Debug.Log("Enter Combat Location");
    }
}
