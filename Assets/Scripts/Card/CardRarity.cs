using UnityEngine;

public enum CardRarityType
{
    Common,
    Rare,
    Legendary,
    Mythic
}

[System.Serializable, CreateAssetMenu(fileName = "new CardRarity", menuName = "Carp/CardRarity")]
public class CardRarity : ScriptableObject
{
    public CardRarityType rarity;
    public Color colour;
    public Sprite icon;

    #region operators
    public static bool operator ==(CardRarity _cardRarity, CardRarityType _rarityType)
    {
        return _cardRarity.rarity == _rarityType;
    }

    public static bool operator !=(CardRarity _cardRarity, CardRarityType _rarityType)
    {
        return _cardRarity.rarity != _rarityType;
    }
    #endregion

    #region overrides
    public override bool Equals(object _other)
    {
        if (_other is CardRarityType)
        {
            return this == (CardRarityType)_other;
        }

        return base.Equals(_other);
    }

    public override string ToString()
    {
        return rarity.ToString();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
