using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurnAction : PlayerTurnAction
{
    public AITurnAction(Player _player) : base(_player)
    {

    }

    public override void InvokeRequest()
    {
        SetupPlayerTypes();


    }
}
