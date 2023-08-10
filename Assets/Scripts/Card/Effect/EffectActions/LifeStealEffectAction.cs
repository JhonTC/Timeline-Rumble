using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "new LifeStealEffectAction", menuName = "Carp/Effect Action/LifeSteal")]
public class LifeStealEffectAction : EffectActionScriptable
{
    [SerializeField] private int lifeStealValue;

    public override string GetActionSentence(string _strTriggerTargetValue, string _strActionTargetValue, TargetQuantityType _targetQuantityType)
    {
        string quantitySuffix = string.Empty;
        if (_targetQuantityType == TargetQuantityType.One)
        {
            quantitySuffix = "s";
        }

        //return $"{_strTriggerTargetValue} lose{quantitySuffix} {lifeStealValue} health and {_strActionTargetValue} gain{quantitySuffix} that much";
        return $"{_strTriggerTargetValue} steal{quantitySuffix} {lifeStealValue} health from {_strActionTargetValue}";
    }

    public override void OnActionTriggered(Character _triggerTarget, Character[] _actionTargets = null, bool useActionTarget = false)
    {
        int combinedLifestealValue = 0;
        foreach (Character actionTarget in _actionTargets)
        {
            actionTarget.ChangeHealthByValue(-lifeStealValue);
            combinedLifestealValue += lifeStealValue;

            Debug.Log(GetActionSentence(_triggerTarget.name, actionTarget.name, TargetQuantityType.One));
        }

        _triggerTarget?.ChangeHealthByValue(combinedLifestealValue);
    }
}
