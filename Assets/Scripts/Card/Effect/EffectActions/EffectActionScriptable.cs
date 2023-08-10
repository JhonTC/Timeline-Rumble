using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectActionScriptable : ScriptableObject
{
    public virtual string GetActionSentence(string _strTriggerTargetValue, string _strActionTargetValue, TargetQuantityType _targetQuantityType) { return null; }
                                                                   
    public virtual void OnActionTriggered(Character _triggerTarget, Character[] _actionTargets = null, bool useActionTarget = false) { }
}
