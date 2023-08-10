using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hand
{
    public List<Card> cards = new List<Card>();
    //[SerializeField] private List<CardHandler> activeCardHandlers = new List<CardHandler>();

    public void AddCard(Card _card)
    {
        cards.Add(_card);
    }

    public bool RemoveCard(Card _card)
    {
        return cards.Remove(_card);
    }

    public PlayCardAction PlayCard(Card _card, Player _player)
    {
        return new PlayCardAction(_card, _player);
    }

    public void DeactivateAllActiveCardHandlers()
    {
        /*for (int i = 0; i < activeCardHandlers.Count; i++)
        {
            //activeCardHandlers[i].Deactivate();
        }

        activeCardHandlers.Clear();*/
    }
}