using Carp.Process;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerTurnProcess : GroupProcess
{
    private Player player;

    private Action<CardView> onCardViewSelected;

    public Dictionary<int, CardView> handCardViews = new Dictionary<int, CardView>();

    private PlayerTurnView playerTurnView;

    private List<DisplayCardProcess> displayCardProcesses = new List<DisplayCardProcess>();

    //private int startingMana = 4;

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

        if (PrefabStore.GetPrefabOfType(out PlayerTurnView playerTurnViewPrefab))
        {
            playerTurnView = UnityEngine.Object.Instantiate(playerTurnViewPrefab, UIManager.Instance.transform);

            playerTurnView.SetManaText(player.character.mana, player.character.maxMana);
            playerTurnView.SetDeckCountText(player.deck.deckSize);
            playerTurnView.SetDiscardCountText(player.deck.discardPileSize);

            player.character.OnManaChanged += playerTurnView.SetManaText; //todo: disconnect?
            player.deck.OnDeckSizeChanged += playerTurnView.SetDeckCountText; //todo: disconnect?
            player.deck.OnDiscardPileSizeChanged += playerTurnView.SetDiscardCountText; //todo: disconnect?

            playerTurnView.endTurnButton.onClick.AddListener(() =>
            {
                player.deck.EmptyHandIntoDiscardPile();
                onComplete?.Invoke(this, null, null);
            });

            player.deck.DealCards(out int initialDeckSize);

            //these calls are separated to allow the CurvedHorizontalLayout to handle the new cards before we scale and move the container
            SpawnPlayerHand(playerTurnView.handViewTransform);
            playerTurnView.StartCoroutine(DisplayPlayerHand(initialDeckSize));
        }

        onComplete += OnComplete;
    }

    public void SpawnPlayerHand(RectTransform _parent)
    {
        List<Card> hand = player.deck.GetHand();
        for (int i = 0; i < hand.Count; i++)
        {
            DisplayCardProcess displayCardProcess = new DisplayCardProcess(hand[i], player, true, 1, _parent, Vector3.zero, true);
            displayCardProcess.deckPosition = playerTurnView.deckPosition;
            displayCardProcess.useDeckPosition = true;
            displayCardProcess.onCardViewCreated += OnCardViewCreated;
            RequestProcess(displayCardProcess);

            displayCardProcesses.Add(displayCardProcess);
        }
    }

    private IEnumerator DisplayPlayerHand(int _initialDeckCount)
    {
        for (int i = 0; i < displayCardProcesses.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);

            displayCardProcesses[i].DisplayCard();

            player.deck.OnDeckSizeChanged?.Invoke(_initialDeckCount - (i + 1));
        }

        displayCardProcesses.Clear();
    }

    public void OnCardViewCreated(CardView cardView)
    {
        cardView.onCardViewSelected += OnCardViewSelected; //todo: disconnect?
        player.character.OnManaChanged += cardView.OnPlayerManaChanged;   //todo: disconnect?
        handCardViews.Add(cardView.GetHashCode(), cardView);
    }

    public void OnCardViewSelected(CardView cardView)
    {
        PlayCardProcess playCardAction = player.deck.PlayCardFromHand(cardView, player);
        playCardAction.discardPilePosition = playerTurnView.discardPilePosition;
        playCardAction.useDiscardPilePosition = true;
        Debug.Log(playCardAction.discardPilePosition);
        if (playCardAction != null)
        {
            RequestProcess(playCardAction);
        }
    }

    public void OnComplete(AbstractProcess sender, object o1, object o2)
    {
        ClearHandCardViews();

        if (playerTurnView != null)
        {
            UnityEngine.Object.Destroy(playerTurnView.gameObject);
        }
    }

    public void ClearHandCardViews()
    {
        foreach (CardView cardView in handCardViews.Values)
        {
            if (cardView != null)
            {
                UnityEngine.Object.Destroy(cardView.gameObject);
            }
        }

        handCardViews.Clear();
    }
}
