using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "new Prebuilt Deck", menuName = "Carp/Prebuilt Deck")]
public class PrebuiltDeck : ScriptableObject
{
    public Card[] cards;
}
