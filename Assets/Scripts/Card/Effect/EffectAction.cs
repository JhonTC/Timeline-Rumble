using Carp.Attributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EffectAction
{
    [Toggle("Use Custom Action Target")]
    public ToggleValue<EffectActionConfig> actionTargetConfig;
    public bool useCustomActionTarget 
    {
        get { return actionTargetConfig.enabled; }
        set { actionTargetConfig.enabled = value; }
    }

    public EffectActionScriptable scriptable;
}

[System.Serializable]
public class EffectActionConfig
{
    public TargetQuantityType targetQuantityType = TargetQuantityType.One;
    public CharacterTypeDetails actionTargetType;
}
