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

    public bool isDead = false;

    public List<CharacterType> characterTypes;

    public Action<GameEventType> OnDeath;
    public Action<GameEventType> OnRevived;

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

    public bool IsCharacterOfType(CharacterType _characterType)
    {
        return characterTypes.Contains(_characterType);
    }
}
