using Carp.Process;
using System;
using System.ComponentModel;
using UnityEngine;

public class PlayerTurnProcess : GroupProcess
{
    private Player player;

    private Action<CardView> onCardViewSelected;

    public PlayerTurnProcess(Player _player) : base(false)
    {
        player = _player;

        onCardViewSelected += OnCardViewSelected;
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

    public override void InvokeProcess()
    {
        Debug.Log($"Start player turn: {player}");

        SetupPlayerTypes();

        UIManager.Instance.DisplayPlayerHand(player, onCardViewSelected);

        onComplete += OnComplete;
    }

    public void OnCardViewSelected(CardView cardView)
    {
        PlayCardProcess playCardAction = player.hand.PlayCard(cardView, player);
        if (playCardAction != null)
        {
            RequestProcess(playCardAction);
        }
    }

    public void OnComplete(AbstractProcess sender, object o1, object o2)
    {
        UIManager.Instance.ClearHandCardViews();
    }
}
