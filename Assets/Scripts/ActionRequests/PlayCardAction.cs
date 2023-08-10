using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayCardAction : FlowAction
{
    public Card card;
    public Player player;
    private Dictionary<Effect, EffectTargetGroup> effectTargetGroups = new Dictionary<Effect, EffectTargetGroup>();
    List<GameEventTriggerConnection> connectedGameEvents = new List<GameEventTriggerConnection>();

    public PlayCardAction(Card _card, Player _player) : base(false)
    {
        card = _card;
        player = _player;
    }

    public override void InvokeRequest()
    {
        Debug.Log($"Play Card: {card}");

        UIManager.Instance.DisplayCard(card);

        AssignTargets();
    }

    public void AssignTargets()
    {
        //Debug.Log($"AssignTargets: {card}");
        for (int i = 0; i < card.effects.Length; i++)
        {
            //in separate loop so that the groups pre-exist and can be checked against before any actionRequests go through
            effectTargetGroups.Add(card.effects[i], new EffectTargetGroup(card.effects[i].action.useCustomActionTarget));
        }

        for (int i = 0; i < card.effects.Length; i++)
        {
            Effect effect = card.effects[i];

            CharacterType targetType = effect.trigger.triggerTargetType;
            TargetQuantityType quantityType = effect.trigger.targetQuantityType;
            LocateEffectTargets(effect, targetType, quantityType, true);

            if (effect.action.useCustomActionTarget)
            {
                targetType = effect.action.actionTargetConfig.value.actionTargetType;
                quantityType = effect.action.actionTargetConfig.value.targetQuantityType;
                LocateEffectTargets(effect, targetType, quantityType, false);
            }
        }
    }

    private void LocateEffectTargets(Effect _effect, CharacterType _targetType, TargetQuantityType _quantityType, bool _isTrigger)
    {
        if (_targetType == CharacterType.Self)
        {
            SetTargetGroupValues(_effect, new Character[] { player.character }, _isTrigger);
        }
        else if (_quantityType == TargetQuantityType.All)
        {
            SetTargetGroupValues(_effect, Player.GetAllCharactersOfType(_targetType), _isTrigger);
        } else
        {
            //Trigger Target Selection Request
            CreateSelectionRequest(_effect, _targetType, _quantityType, _isTrigger);
        }
    }

    private void CreateSelectionRequest(Effect effect, CharacterType _targetType, TargetQuantityType _quantityType, bool isTrigger)
    {
        int numberOfTargets = effect.GetTargetQuantityAsInteger(isTrigger);
        //Debug.Log($"{(isTrigger ? "Trigger" : "Action")}Targets: {numberOfTargets}");

        CharacterSelectionAction<SelectionRequestPair> selectionRequest = new CharacterSelectionAction<SelectionRequestPair>(
            _targetType,
            new SelectionRequestPair(effect, isTrigger),
            numberOfTargets,
            GetSelectorDescription(_targetType, _quantityType));

        selectionRequest.onComplete += OnTargetsSelected;
        RequestAction(selectionRequest);
    }

    private string GetSelectorDescription(CharacterType _targetType, TargetQuantityType _quantityType)
    {
        string description = "Target";

        string prefix = string.Empty;
        string targetTypeString = string.Empty;
        string suffix = "creature";

        switch (_targetType)
        {
            case CharacterType.Any:
                prefix = "any";
                break;
            case CharacterType.Friendly:
                prefix = "a";
                targetTypeString = "friendly";
                break;
            case CharacterType.Enemy:
                prefix = "an";
                targetTypeString = "enemy";
                break;
            case CharacterType.Ally:
                prefix = "an";
                targetTypeString = "allied";
                break;
        }

        switch (_quantityType)
        {
            case TargetQuantityType.Two:
                prefix = "two";
                suffix += "s";
                break;
            case TargetQuantityType.All:
                prefix = "all";
                suffix += "s";
                break;
        }

        string[] phraseParts = new string[3] { prefix, targetTypeString, suffix };
        for (int i = 0; i < phraseParts.Length; i++)
        {
            if (description != string.Empty && phraseParts[i] != string.Empty)
            {
                description += " ";
            }

            if (phraseParts[i] != string.Empty)
            {
                description += phraseParts[i];
            }
        }

        return description;
    }

    public void OnTargetsSelected(AbstractAction sender, object value, object parameter)
    {
        //Debug.Log($"OnTargetsSelected: {value}");

        Character[] characters = value as Character[];
        if (characters == null) return; //todo: fire off error code

        SelectionRequestPair selectionRequestPair = parameter as SelectionRequestPair;
        if (selectionRequestPair == null) return; //todo: fire off error code

        SetTargetGroupValues(selectionRequestPair.effect, characters, selectionRequestPair.isTriggerTarget);
    }

    private void SetTargetGroupValues(Effect effect, Character[] characters, bool isTriggerTarget)
    {
        if (effect != null)
        {
            if (effectTargetGroups.ContainsKey(effect))
            {
                if (isTriggerTarget)
                {
                    effectTargetGroups[effect].triggerTargets = characters;
                }
                else
                {
                    effectTargetGroups[effect].actionTargets = characters;
                }
            }
        }

        if (AreAllTargetsAssigned())
        {
            ConnectRequiredGameEvents(out bool triggerWhenPlayed);

            if (triggerWhenPlayed)
            {
                ActivateCardEffectsWithTrigger(GameEventType.CardPlayed);
            }

            GameManager.instance.StartCoroutine(TriggerOnCompleteAfterDelay(4)); //FOR TESTING
            //onComplete?.Invoke(this, null, null);
        }
    }

    IEnumerator TriggerOnCompleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        UIManager.Instance.RemoveCardView(card);
        onComplete?.Invoke(this, null, null);
    }

    public void ConnectRequiredGameEvents(out bool triggerWhenPlayed)
    {
        triggerWhenPlayed = false;

        List<GameEventTriggerConnection> effectTriggers = new List<GameEventTriggerConnection>();

        for (int i = 0; i < card.effects.Length; i++)
        {
            GameEventType effectTrigger = card.effects[i].trigger.triggerType;
            switch (effectTrigger)
            {
                case GameEventType.CreatureDeath:
                    if (effectTargetGroups.ContainsKey(card.effects[i]))
                    {
                        foreach (Character character in effectTargetGroups[card.effects[i]].triggerTargets)
                        {
                            GameEventTriggerConnection creatureDeathTriggerConnection = new GameEventTriggerConnection(effectTrigger, character);
                            if (!effectTriggers.Contains(creatureDeathTriggerConnection))
                            {
                                effectTriggers.Add(creatureDeathTriggerConnection);
                            }
                        }
                    }
                    break;
                case GameEventType.CardPlayed:
                case GameEventType.RoundStart:
                case GameEventType.TurnStart:
                    GameEventTriggerConnection defaultTriggerConnection = new GameEventTriggerConnection(effectTrigger, null);
                    if (!effectTriggers.Contains(defaultTriggerConnection))
                    {
                        effectTriggers.Add(defaultTriggerConnection);
                    }
                    break;
            }
        }

        foreach (var triggerConnection in effectTriggers)
        {
            switch (triggerConnection.eventTriggerType)
            {
                case GameEventType.CreatureDeath:
                    ConnectToGameEvent(ref triggerConnection.triggerTarget.OnDeath, triggerConnection);
                    break;
                case GameEventType.CardPlayed:
                    triggerWhenPlayed = true;
                    break;
                case GameEventType.RoundStart:
                    ConnectToGameEvent(ref GameManager.instance.OnRoundStart, triggerConnection);
                    break;
                case GameEventType.TurnStart:
                    ConnectToGameEvent(ref GameManager.instance.OnTurnStart, triggerConnection);
                    break;
            }
        }
    }

    public void ConnectToGameEvent(ref Action<GameEventType> gameEvent, GameEventTriggerConnection gameEventTriggerConnection)
    {
        gameEventTriggerConnection.eventAction = gameEvent;
        connectedGameEvents.Add(gameEventTriggerConnection);

        gameEvent += ActivateCardEffectsWithTrigger;
    }

    public bool AreAllTargetsAssigned()
    {
        foreach (EffectTargetGroup targetGroup in effectTargetGroups.Values)
        {
            if (!targetGroup.AreTargetsAssigned())
            {
                return false;
            }
        }

        return true;
    }

    public void ActivateCardEffectsWithTrigger(GameEventType _trigger)
    {
        Debug.Log($"Activate Card ({card}), with trigger: {_trigger}");
        for (int i = 0; i < card.effects.Length; i++)
        {
            Effect effect = card.effects[i];
            if (effect == _trigger)
            {
                if (effectTargetGroups.ContainsKey(effect))
                {
                    effect.ActivateEffect(effectTargetGroups[effect].triggerTargets, effectTargetGroups[effect].actionTargets);
                    effectTargetGroups.Remove(effect);
                }
                else
                {
                    Debug.LogWarning("KEY DOESNT EXIST");
                }
            }
        }

        /*for (int i = connectedGameEvents.Count - 1; i >= 0; i--)
        {
            if (connectedGameEvents[i].eventTriggerType == _trigger)
            {
                connectedGameEvents[i].eventAction -= ActivateCardEffectsWithTrigger;
                connectedGameEvents.Remove(connectedGameEvents[i]);
            }
        }*/
    }

    public void Deactivate()
    {
        for (int i = 0; i < connectedGameEvents.Count; i++)
        {
            connectedGameEvents[i].eventAction -= ActivateCardEffectsWithTrigger;
        }
        connectedGameEvents.Clear();
        effectTargetGroups.Clear();
        card = null;
    }
}

public class SelectionRequestPair
{
    public Effect effect;
    public bool isTriggerTarget;

    public SelectionRequestPair(Effect _effect, bool _isTriggerTarget)
    {
        effect = _effect;
        isTriggerTarget = _isTriggerTarget;
    }
}

public class EffectTargetGroup
{
    public Character[] triggerTargets = null;
    public Character[] actionTargets = null;
    public bool usesCustomActionTarget;

    public EffectTargetGroup(bool _usesCustomActionTarget)
    {
        usesCustomActionTarget = _usesCustomActionTarget;
    }

    public bool AreTargetsAssigned()
    {
        bool isSetup = triggerTargets != null;
        if (usesCustomActionTarget)
        {
            isSetup = isSetup && actionTargets != null;
        }

        return isSetup;
    }
}

public class GameEventTriggerConnection
{
    public Action<GameEventType> eventAction;
    public GameEventType eventTriggerType;
    public Character triggerTarget;

    public GameEventTriggerConnection(GameEventType _eventTriggerType, Character _triggerTarget)
    {
        eventTriggerType = _eventTriggerType;
        triggerTarget = _triggerTarget;
    }

    public GameEventTriggerConnection(ref Action<GameEventType> _eventAction, GameEventType _eventTriggerType, Character _triggerTarget)
    {
        eventAction = _eventAction;
        eventTriggerType = _eventTriggerType;
        triggerTarget = _triggerTarget;
    }

    #region operators
    public static bool operator ==(GameEventTriggerConnection _triggerConnection, GameEventTriggerConnection _otherTriggerConnection)
    {
        return _triggerConnection.eventTriggerType == _otherTriggerConnection.eventTriggerType && _triggerConnection.triggerTarget == _otherTriggerConnection.triggerTarget;
    }
    public static bool operator ==(GameEventTriggerConnection _triggerConnection, GameEventType _eventType)
    {
        return _triggerConnection.eventTriggerType == _eventType;
    }

    public static bool operator !=(GameEventTriggerConnection _triggerConnection, GameEventTriggerConnection _otherTriggerConnection)
    {
        return _triggerConnection.eventTriggerType != _otherTriggerConnection.eventTriggerType || _triggerConnection.triggerTarget != _otherTriggerConnection.triggerTarget;
    }
    public static bool operator !=(GameEventTriggerConnection _triggerConnection, GameEventType _eventType)
    {
        return _triggerConnection.eventTriggerType != _eventType;
    }
    #endregion

    #region overrides
    public override bool Equals(object _other)
    {
        if (_other is GameEventTriggerConnection)
        {
            return this == (GameEventTriggerConnection)_other;
        }

        if (_other is GameEventType)
        {
            return this == (GameEventType)_other;
        }

        return base.Equals(_other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
