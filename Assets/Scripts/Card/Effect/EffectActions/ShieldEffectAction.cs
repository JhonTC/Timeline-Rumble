using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "new ShieldEffectAction", menuName = "Carp/Effect Action/Shield")]
public class ShieldEffectAction : EffectActionScriptable
{
    [SerializeField] private int shieldValue;

    public override string GetActionSentence(string _strTriggerTargetValue, string _strActionTargetValue, TargetQuantityType _targetQuantityType)
    {
        string quantitySuffix = string.Empty;
        if (_targetQuantityType == TargetQuantityType.One)
        {
            quantitySuffix = "s";
        }

        return $"{_strActionTargetValue} gain{quantitySuffix} {shieldValue} Armour.";
    }

    public override void OnActionTriggered(Character _triggerTarget, Character[] _actionTargets = null, bool useActionTarget = false)
    {
        if (!useActionTarget)
        {
            PerformAction(_triggerTarget);
        }
        else
        {
            foreach (Character actionTarget in _actionTargets)
            {
                PerformAction(actionTarget);
            }
        }
    }

    private void PerformAction(Character _target)
    {
        Debug.Log(GetActionSentence(null, _target.name, TargetQuantityType.One));

        _target?.ChangeArmourByValue(shieldValue);
    }
}
