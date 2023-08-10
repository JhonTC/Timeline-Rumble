using System;
using System.ComponentModel;
using UnityEngine;

public interface IDynamicSelector
{
    public void OnClickHit(RaycastHit hit);
}

public class Selector<T> : IDynamicSelector
{
    public Action<T> OnTypeSelected;

    public Selector()
    {
        Player.LocalPlayer?.selectors.Add(this);
    }

    public virtual void OnClickHit(RaycastHit hit)
    {
        T component = hit.collider.GetComponent<T>();
        if (CanInvoke(component))
        {
            OnTypeSelected?.Invoke(component);
        }
    }

    public virtual bool CanInvoke(T component)
    {
        return component != null;
    }
}

public class CharacterSelector : Selector<Character>
{
    private CharacterType characterTargetType;

    public CharacterSelector(CharacterType _characterTargetType) : base()
    {
        characterTargetType = _characterTargetType;
    }

    public override bool CanInvoke(Character character)
    {
        bool canInvoke = base.CanInvoke(character);
        if (canInvoke)
        {
            canInvoke = character.IsCharacterOfType(characterTargetType);

            if (!canInvoke) 
            {
                Debug.Log($"Character: {character} is not of type: {characterTargetType}");
            }
        }

        return canInvoke;
    }
}
