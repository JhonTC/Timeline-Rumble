using System;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{   
    Null,
    Self,
    Friendly,
    Ally,
    Enemy,
    Any
}

public class Character : MonoBehaviour
{
    public int health = 20;
    public int armour = 0;

    public int maxMana = 5;
    public int mana = 5;

    public bool isDead = false;

    public List<CharacterType> characterTypes;

    public Action<GameEventType> OnDeath;
    public Action<GameEventType> OnRevived;
    public Action<int, int> OnManaChanged;

    public List<PlayCardProcess> activeCardProcesses = new List<PlayCardProcess>();

    public void ChangeHealthByValue(int delta)
    {
        health = Mathf.Clamp(health + delta, 0, int.MaxValue);

        if (health <= 0 && !isDead)
        {
            Debug.Log($"Character {name} dies!");

            isDead = true;

            OnDeath?.Invoke(GameEventType.CreatureDeath);
        }

        if (health > 0 && isDead)
        {
            Debug.Log($"Character {name} is revived!");

            isDead = false;

            //OnRevived?.Invoke(GameEventType.CreatureDeath); need new GameEventType => GameEventType.CreatureRevived
        }
    }

    public void ChangeArmourByValue(int delta)
    {
        armour += delta;

        if (armour < 0)
        {
            armour = 0;
        }
    }

    public void ChangeManaByValue(int valueDelta, int maxDelta = 0)
    {
        mana = Mathf.Clamp(mana + valueDelta, 0, int.MaxValue);
        maxMana = Mathf.Clamp(maxMana + maxDelta, 0, int.MaxValue);

        OnManaChanged?.Invoke(mana, maxMana);
    }

    public void ChangeMaxByValue(int delta)
    {
        mana = Mathf.Clamp(mana + delta, 0, int.MaxValue);

        OnManaChanged?.Invoke(mana, maxMana);
    }

    public bool IsCharacterOfType(CharacterType _characterType)
    {
        return characterTypes.Contains(_characterType);
    }
}
