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
    public FlowAction gameFlowHandler;

    public Action<GameEventType> OnRoundStart;
    public Action<GameEventType> OnTurnStart;

    public Player tempPlayer1;
    public Player tempPlayer2;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameFlowHandler = new FlowAction();

        PlayerTurnAction player1TurnAction = new PlayerTurnAction(tempPlayer1);
        gameFlowHandler.RequestAction(player1TurnAction);

        FlowAction player2TurnAction = new PlayerTurnAction(tempPlayer2);
        gameFlowHandler.RequestAction(player2TurnAction);
    }

    // Update is called once per frame
    void Update()
    {
        gameFlowHandler.Update();
    }
}
