using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectTrigger
{
    public GameEventType triggerType;
    public CharacterType triggerTargetType;
    public TargetQuantityType targetQuantityType;

    #region operators
    public static bool operator ==(EffectTrigger _trigger, GameEventType _triggerType)
    {
        return _trigger.triggerType == _triggerType;
    }

    public static bool operator !=(EffectTrigger _trigger, GameEventType _triggerType)
    {
        return _trigger.triggerType != _triggerType;
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

    public override string ToString()
    {
        return triggerType.ToString();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
