using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    [SerializeField] private List<Card> cards = new List<Card>();

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public bool RemoveCard(Card card)
    {
        return cards.Remove(card);
    }
}

