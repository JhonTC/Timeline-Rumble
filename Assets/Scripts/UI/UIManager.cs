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

    public void DisplayCard(Card _card, Player _player)
    {
        Vector3 position = Camera.main.WorldToScreenPoint(_player.character.gameObject.transform.position);
        position += Vector3.up * 100f;

        CardView cardview = Instantiate(cardViewPrefab, position, Quaternion.identity, cardDisplayParent);
        cardview.transform.localScale = Vector3.zero;
        cardview.RenderView(_card);

        activeCardViews.Add(_card, cardview);

        LeanTween.scale(cardview.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutSine);
    }

    public void RemoveCardView(Card _card)
    {
        if (activeCardViews.ContainsKey(_card))
        {
            LTDescr scaleTween = LeanTween.scale(activeCardViews[_card].gameObject, Vector3.zero, 0.5f);
            scaleTween.setOnComplete(() =>
            {     
                Destroy(activeCardViews[_card].gameObject);
                activeCardViews.Remove(_card);
            });
            scaleTween.setEase(LeanTweenType.easeInOutSine);
        }
    } 
}
