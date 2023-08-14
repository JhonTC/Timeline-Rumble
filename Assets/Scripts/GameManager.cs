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

    public Player tempPlayer1;
    public Player tempPlayer2;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameFlowHandler = new GroupProcess();

        Team[] teams = new Team[]
        {
            new Team(tempPlayer1),
            new Team(tempPlayer2)
        };

        CombatLocationProcess locationProcess = new CombatLocationProcess(teams);
        gameFlowHandler.RequestProcess(locationProcess);
    }

    // Update is called once per frame
    void Update()
    {
        gameFlowHandler.Update();
    }
}
