using Carp.Attributes;

public enum TargetQuantityType
{
    One,
    Two,
    All
}

[System.Serializable]
public class Effect
{
    public EffectTrigger trigger;
    public EffectAction action;

    [ReadOnly] public string description;

    public virtual void ActivateEffect(Character[] _triggerTargets, Character[] _actionTargets = null)
    {
        if (trigger.targetQuantityType == TargetQuantityType.One)
        {
            if (_triggerTargets.Length > 0)
            {
                action.scriptable.OnActionTriggered(_triggerTargets[0], _actionTargets, action.useCustomActionTarget);
            }
        } else
        {
            for (int i = 0; i < _triggerTargets.Length; i++)
            {
                action.scriptable.OnActionTriggered(_triggerTargets[i], _actionTargets, action.useCustomActionTarget);
            }
        }
    }

    public int GetTargetQuantityAsInteger(bool isTrigger = true)
    {
        TargetQuantityType targetQuantityType = trigger.targetQuantityType;
        if (!isTrigger)
        {
            targetQuantityType = action.actionTargetConfig.value.targetQuantityType;
        }

        int returnValue = 1;
        switch(targetQuantityType)
        {
            case TargetQuantityType.Two:
                returnValue = 2;
                break;
            case TargetQuantityType.All:
                returnValue = Player.list.Count;
                break;
        }

        return returnValue;
    }

    #region operators
    public static bool operator ==(Effect _effect, GameEventType _triggerType)
    {
        return _effect.trigger.triggerType == _triggerType;
    }

    public static bool operator !=(Effect _effect, GameEventType _triggerType)
    {
        return _effect.trigger.triggerType != _triggerType;
    }
    #endregion

    #region overrides
    public override bool Equals(object _other)
    {
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
