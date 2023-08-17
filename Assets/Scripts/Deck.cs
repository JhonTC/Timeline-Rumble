using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AccessType
{
    Deck,
    Hand,
    DiscardPile
}

[System.Serializable]
public class Deck
{
    [SerializeField] private int handSize = 2;
    [SerializeField] private int maxHandSize = 8;

    [SerializeField] private List<Card> deck = new List<Card>();
    [SerializeField] private List<Card> hand = new List<Card>();
    [SerializeField] private List<Card> discardPile = new List<Card>();

    public Action<int> OnDeckSizeChanged;
    public Action<int> OnDiscardPileSizeChanged;

    public int deckSize => deck.Count;
    public int discardPileSize => discardPile.Count;

    public bool isPlayingCard = false;
    public void SetDeck(Card[] _cards)
    {
        deck.Clear();
        deck = _cards.ToList();

        deck.Shuffle();

        OnDeckSizeChanged?.Invoke(deck.Count);
    }

    public void DealCards(out int _initialCount, int _cardCountModifier = 0)
    {
        _initialCount = deckSize;

        for (int i = 0; i < handSize + _cardCountModifier; i++)
        {
            if(deck.Count <= 0)
            {
                ShuffleDiscardIntoDeck();
            }

            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    public PlayCardProcess PlayCardFromHand(CardView _cardView, Player _player)
    {
        if (hand.Contains(_cardView.card))
        {
            hand.Remove(_cardView.card);
        }

        PlayCardProcess playCardProcess = new PlayCardProcess(_cardView, _player, true);
        playCardProcess.onComplete += (sender, value, parameters) =>
        {
            discardPile.Add(_cardView.card);
            OnDiscardPileSizeChanged?.Invoke(discardPile.Count);
        };

        return playCardProcess;
    }

    public void EmptyHandIntoDiscardPile()
    {
        discardPile.AddRange(hand);
        hand.Clear();

        OnDiscardPileSizeChanged?.Invoke(discardPile.Count);
    }

    private void ShuffleDiscardIntoDeck()
    {
        deck.AddRange(discardPile);
        deck.Shuffle();

        discardPile.Clear();

        OnDeckSizeChanged?.Invoke(deck.Count);
        OnDiscardPileSizeChanged?.Invoke(discardPile.Count);
    }
}

