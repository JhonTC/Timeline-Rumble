using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerTurnAction : FlowAction
{
    private Player player;

    public PlayerTurnAction(Player _player) : base(true)
    {
        player = _player;
    }

    public void SetupPlayerTypes()
    {
        foreach (Player listPlayer in Player.list.Values)
        {
            Character listCharacter = listPlayer.character;
            listCharacter.characterTypes.Clear();

            listCharacter.characterTypes.Add(CharacterType.Any);

            if (listPlayer.teamId == player.teamId)
            {
                if (listPlayer == player)
                {
                    listCharacter.characterTypes.Add(CharacterType.Self);
                }
                else
                {
                    listCharacter.characterTypes.Add(CharacterType.Ally);
                }

                listCharacter.characterTypes.Add(CharacterType.Friendly);
            } else
            {
                listCharacter.characterTypes.Add(CharacterType.Enemy);
            }
        }
    }

    public override void InvokeRequest()
    {
        SetupPlayerTypes();

        PlayCardAction playCardAction = player.PlayFirstCardInHand();
        if (playCardAction != null)
        {
            RequestAction(playCardAction);
        }
    }
}
