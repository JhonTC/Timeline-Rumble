using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IView
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image typeIconImage;
    [SerializeField] private Image rarityBackgroundImage;
    [SerializeField] private Image coverImage;

    public void RenderView(object _value)
    {
        Card card = _value as Card;
        if (card != null)
        {
            nameText.text = card.name;
            descriptionText.text = card.description;
            costText.text = card.cost.ToString();
            typeIconImage.sprite = card.typeImage;
            rarityBackgroundImage.color = card.rarity.colour;
            coverImage.sprite = card.coverImage;

            gameObject.SetActive(true);
        }
    }
}
