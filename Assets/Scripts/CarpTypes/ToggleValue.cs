using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToggleValue<T>
{
    [HideInInspector] public bool enabled;
    public T value;
}
