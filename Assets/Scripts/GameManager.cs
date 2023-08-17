using Carp.Process;
using System;
using UnityEngine;

public enum GameEventType
{
    CardPlayed,
    CreatureDeath,
    RoundStart,
    TurnStart
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GroupProcess gameFlowHandler;

    public Action<GameEventType> OnRoundStart;
    public Action<GameEventType> OnTurnStart;

    public PrebuiltDeck defaultDeck;
    public Player tempPlayer1;
    public Player tempPlayer2;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.init();

        gameFlowHandler = new GroupProcess();

        Team[] teams = new Team[]
        {
            new Team(tempPlayer1),
            new Team(tempPlayer2)
        };

        tempPlayer1.deck.SetDeck(defaultDeck.cards);
        tempPlayer2.deck.SetDeck(defaultDeck.cards);

        CombatLocationProcess locationProcess = new CombatLocationProcess(teams);
        gameFlowHandler.RequestProcess(locationProcess);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameFlowHandler != null)
        {
            gameFlowHandler.Update();
        }
    }
}
