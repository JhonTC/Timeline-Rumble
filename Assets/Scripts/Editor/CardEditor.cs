using Codice.Client.BaseCommands;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    Card card;
    List<string> effectDescriptions = new List<string>();
    GUIStyle cardStyle;
    const string space = " ";

    private void OnEnable()
    {
        card = (Card)target;
    }

    public override void OnInspectorGUI()
    {
        cardStyle = EditorStyles.wordWrappedLabel;

        CheckForEffectsIncompatability();

        EditorGUILayout.LabelField("Card description", EditorStyles.boldLabel);
        CreateCardDescription(cardStyle, out string fullDescription, out string[] effectDescriptions);
        EditorGUILayout.LabelField(fullDescription, cardStyle);

        EditorGUI.BeginDisabledGroup(DoDescriptionsMatch(card, fullDescription, effectDescriptions));
        if (GUILayout.Button("Save description to Card")) 
        {
            ApplyNewDescriptions(card, fullDescription, effectDescriptions);
            EditorUtility.SetDirty(card);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Card details", EditorStyles.boldLabel);
        base.OnInspectorGUI();
    }

    public bool DoDescriptionsMatch(Card card, string fullDescription, string[] effectDescriptions)
    {
        if (card.description != fullDescription) return false;

        for (int i = 0; i < card.effects.Length; i++)
        { 
            if (card.effects[i].description != effectDescriptions[i])
            {
                return false;
            }
        }

        return true;
    }

    public void ApplyNewDescriptions(Card card, string fullDescription, string[] effectDescriptions)
    {
        card.description = fullDescription;
        for (int i = 0; i < card.effects.Length; i++)
        {
            card.effects[i].description = effectDescriptions[i];
        }
    }

    private void CheckForEffectsIncompatability()
    {
        bool setDirty = false;

        Effect[] effects = card.effects;
        if (effects == null) return;

        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].trigger.triggerType == GameEventType.CreatureDeath &&
                !effects[i].action.useCustomActionTarget)
            {
                effects[i].action.useCustomActionTarget = true;
                setDirty = true;
            }

            /*if (effects[i].trigger.triggerTargetType == CharacterType.Null)
            {
                effects[i].trigger.triggerTargetType = CharacterType.Self;
                setDirty = true;
            }*/

            if (effects[i].trigger.triggerTargetType != null)
            {
                if (effects[i].trigger.triggerTargetType == CharacterType.Self &&
                    effects[i].trigger.targetQuantityType != TargetQuantityType.One)
                {
                    effects[i].trigger.targetQuantityType = TargetQuantityType.One;
                    setDirty = true;
                }
            }

            if (effects[i].action.actionTargetConfig.value.actionTargetType != null)
            {
                if (effects[i].action.useCustomActionTarget &&
                    effects[i].action.actionTargetConfig.value.actionTargetType == CharacterType.Self &&
                    effects[i].action.actionTargetConfig.value.targetQuantityType != TargetQuantityType.One)
                {
                    effects[i].action.actionTargetConfig.value.targetQuantityType = TargetQuantityType.One;
                    setDirty = true;
                }
            }
            

            /*if (!effects[i].action.useCustomActionTarget && 
                effects[i].action.actionTargetConfig.value.actionTargetType != CharacterType.Null)
            {
                effects[i].action.actionTargetConfig.value.actionTargetType = CharacterType.Null;
                setDirty = true;
            }

            if (effects[i].action.useCustomActionTarget && 
                effects[i].action.actionTargetConfig.value.actionTargetType == CharacterType.Null)
            {
                //effects[i].action.useCustomActionTarget = false;
                effects[i].action.actionTargetConfig.value.actionTargetType = CharacterType.Self;
                setDirty = true;
            }*/

            /*if (effects[i].action.useCustomActionTarget &&
                effects[i].action.actionTargetConfig.value.actionTargetType == effects[i].trigger.triggerTargetType)
            {
                effects[i].action.useCustomActionTarget = false;
                if (effects[i].action.actionTargetConfig.value.actionTargetType != EffectTargetType.Null)
                {
                    effects[i].action.actionTargetConfig.value.actionTargetType = EffectTargetType.Null;
                } else
                {
                    effects[i].action.actionTargetConfig.value.actionTargetType = EffectTargetType.Self;
                }

                setDirty = true;
            }*/
        }

        if (setDirty)
        {
            EditorUtility.SetDirty(card);
        }
    }

    private void CreateCardDescription(GUIStyle cardStyle, out string fullDescription, out string[] effectDescriptions)
    {
        StringBuilder sb = new StringBuilder();

        Effect[] effects = card.effects;
        if (effects == null)
        {
            fullDescription = "Card has no effects";
            effectDescriptions = null;
            return;
        }

        effectDescriptions = new string[effects.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            string effectDescription = CreateEffectDescription(effects[i]);
            effectDescriptions[i] = effectDescription;

            sb.Append(effectDescription);

            if (effects.Length > 0)
            {
                sb.Append('\n');
            }
        }
        
        if (sb.Length == 0)
        {
            sb.Append("Card has no effects");
        }

        fullDescription = CapitalizeAfter(sb, new[] { '.', ':', '?', '!' });

        CardRarity rarity = card.rarity;
        cardStyle.normal.textColor = rarity != null ? rarity.colour : Color.white;
    }

    private string CreateEffectDescription(Effect effect)
    {
        StringBuilder sb = new StringBuilder();

        string targetSentence = GetTargetSentence(effect.trigger, out string effectActionPrefix);
        sb.Append(targetSentence);
        if (targetSentence != string.Empty)
        {
            sb.Append(". ");
        }

        EffectTrigger effectTrigger = effect.trigger;
        sb.Append(GetTriggerSentence(effectTrigger, effect.trigger.triggerTargetType));

        sb.Append(", ");

        EffectAction effectAction = effect.action;
        if (effectAction != null)
        {
            string triggerTargetTypeText = string.Empty;
            string actionTargetTypeText = string.Empty;
            if (effectAction.useCustomActionTarget)
            {
                triggerTargetTypeText = GetTargetTypeString(effectTrigger.triggerTargetType, effectTrigger.targetQuantityType);
                actionTargetTypeText = GetTargetTypeString(effectAction.actionTargetConfig.value.actionTargetType, effectAction.actionTargetConfig.value.targetQuantityType, true);
            }
            else
            {
                actionTargetTypeText = GetTargetTypeString(effectTrigger.triggerTargetType, effectTrigger.targetQuantityType);
            }

            if (effectAction.scriptable != null)
            {
                TargetQuantityType targetQuantityType = effectAction.useCustomActionTarget ? effectAction.actionTargetConfig.value.targetQuantityType : effectTrigger.targetQuantityType;
                sb.Append(effectAction.scriptable.GetActionSentence(triggerTargetTypeText, actionTargetTypeText, targetQuantityType));
            }
        }
        else
        {
            sb.Append("nothing happens!");
        }

        string description = CapitalizeAfter(sb, new[] { '.', ':', '?', '!' });
        return description;
    }

    private string GetTargetTypeString(CharacterTypeDetails _effectTargetType, TargetQuantityType _targetQuantityType, bool useCustomActionTarget = false)
    {
        if (_effectTargetType == null) return "No Target Type Selected";

        string prefix = string.Empty;
        string targetTypeString = "target";
        string suffix = "creature";

        if (useCustomActionTarget)
        {
            switch (_effectTargetType.characterType)
            {
                case CharacterType.Any:
                    prefix = "a";
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
        }

        if (targetTypeString != "target")
        {
            targetTypeString = $"<color=#{ColorUtility.ToHtmlStringRGB(_effectTargetType.colour)}>{targetTypeString}</color>";
        }

        if (_effectTargetType == CharacterType.Self)
        {
            targetTypeString = "your";
        }

        switch (_targetQuantityType)
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

        if (!useCustomActionTarget)
        {
            prefix = string.Empty;
        }

        string[] phraseParts = new string[3] { prefix, targetTypeString, suffix };
        string finalPhrase = string.Empty;
        for (int i = 0; i < phraseParts.Length; i++)
        {
            if (finalPhrase != string.Empty && phraseParts[i] != string.Empty)
            {
                finalPhrase += space;
            }

            if (phraseParts[i] != string.Empty)
            {
                finalPhrase += phraseParts[i];
            }

        }

        return finalPhrase;
    }


    private string GetTargetSentence(EffectTrigger _trigger, out string _effectActionPrefix, string _suffix = "")
    {
        _effectActionPrefix = "target creature";

        if (_trigger.triggerTargetType == null) return "No Target Type Selected";

        string quantitySuffix = string.Empty;
        string strJoiner = string.Empty;
        string strTargetType = string.Empty;
        string strTargetQuantity = string.Empty;

        switch (_trigger.triggerTargetType.characterType)
        {
            case CharacterType.Any:
                strJoiner = "any";
                break;
            case CharacterType.Self:
                _effectActionPrefix = "your creature";
                return string.Empty;
            case CharacterType.Friendly:
                strJoiner = "a";
                strTargetType = "friendly";
                break;
            case CharacterType.Enemy:
                strJoiner = "an";
                strTargetType = "enemy";
                break;
            case CharacterType.Ally:
                strJoiner = "an";
                strTargetType = "allied";
                break;
        }

        switch (_trigger.targetQuantityType)
        {
            case TargetQuantityType.Two:
                if (strJoiner != "any")
                {
                    strJoiner = string.Empty;
                }
                strTargetQuantity = "two";
                quantitySuffix = "s";
                break;
            case TargetQuantityType.All:
                _effectActionPrefix = "all creatures";
                return string.Empty;
        }

        if (strJoiner != string.Empty)
        {
            strJoiner += space;
        }

        if (strTargetQuantity != string.Empty)
        {
            strTargetQuantity += space;
        }

        if (strTargetType != string.Empty)
        {
            strTargetType = $"<color=#{ColorUtility.ToHtmlStringRGB(_trigger.triggerTargetType.colour)}>{strTargetType}</color>{space}";
        }

        string targetSentence = $"Target {strJoiner}{strTargetQuantity}{strTargetType}creature{quantitySuffix}{_suffix}";

        return targetSentence;
    }

    private string GetTriggerSentence(EffectTrigger _trigger, CharacterTypeDetails _effectTargetType)
    {
        if (_trigger.triggerTargetType == null) return "No Target Type Selected";

        switch (_trigger.triggerType)
        {
            case GameEventType.TurnStart:
                return "at the start of each turn";
            case GameEventType.RoundStart:
                return "at the start of each round";
            case GameEventType.CreatureDeath:
                string targetTypeString = "target";
                if (_effectTargetType == CharacterType.Self)
                {
                    targetTypeString = "your";
                }
                return $"when {targetTypeString} creature dies";
            case GameEventType.CardPlayed:
                return "when played";
        }

        return _trigger.ToString().ToUpper();
    }

    public string CapitalizeAfter(StringBuilder sb, IEnumerable<char> chars)
    {
        var charsHash = new HashSet<char>(chars);
        for (int i = 0; i < sb.Length - 2; i++)
        {
            if (charsHash.Contains(sb[i]) && sb[i + 1] == ' ')
            {
                sb[i + 2] = char.ToUpper(sb[i + 2]);
            }

            if (i == 0 && !charsHash.Contains(sb[i]))
            {
                sb[i] = char.ToUpper(sb[i]);
            }
        }

        return sb.ToString();
    }
}
