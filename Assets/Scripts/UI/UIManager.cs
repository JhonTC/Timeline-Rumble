using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Dictionary<Card, CardView> activeCardViews = new Dictionary<Card, CardView>();

    public CardView cardViewPrefab;
    public Transform cardDisplayParent;

    private void Awake()
    {
        Instance = this;
    }

    public void DisplayCard(Card _card)
    {
        CardView cardview = Instantiate(cardViewPrefab, cardDisplayParent);
        cardview.RenderView(_card);

        activeCardViews.Add(_card, cardview);
    }

    public void RemoveCardView(Card _card)
    {
        if (activeCardViews.ContainsKey(_card))
        {
            Destroy(activeCardViews[_card].gameObject);
            activeCardViews.Remove(_card);
        }
    } 
}
