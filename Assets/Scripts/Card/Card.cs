using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "new Card", menuName = "Carp/Card")]
public class Card : ScriptableObject
{
    public int cost;
    public CardRarity rarity;
    public Effect[] effects;
    [HideInInspector] public string description;
    public Sprite coverImage;
    public Sprite typeImage;

    #region operators
    public static bool operator ==(Card _card, CardRarityType _rarityType)
    {
        return _card.rarity == _rarityType;
    }

    public static bool operator !=(Card _card, CardRarityType _rarityType)
    {
        return _card.rarity != _rarityType;
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
        return name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
