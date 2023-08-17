using Carp.Process;
using System;
using UnityEngine;

public class DisplayCardProcess : AbstractProcess
{
    private CardView cardView;

    private Card card;
    private Player owner;

    public Action<CardView> onCardViewCreated;

    private RectTransform cardViewParent;
    private bool isInHand = false;
    private float scaleRelativeToParent;
    private Vector3 position;
    private bool animateOpen = true;

    public Vector3 deckPosition;
    public bool useDeckPosition;

    public DisplayCardProcess(Card _card, Player _player, RectTransform _cardViewParent) : base(false, 0)
    {
        card = _card;
        owner = _player;
        cardViewParent = _cardViewParent;
        scaleRelativeToParent = 0.5f;
        position = Camera.main.WorldToScreenPoint(_player.character.gameObject.transform.position);
    }
    public DisplayCardProcess(CardView _cardView, Player _player, RectTransform _cardViewParent) : base(false, 0)
    {
        cardView = _cardView;
        //_cardView.container.SetActive(false);

        card = _cardView.card;
        owner = _player;
        cardViewParent = _cardViewParent;
        scaleRelativeToParent = 0.5f;
        position = _cardView.transform.position;
        animateOpen = false;

        //UnityEngine.Object.Destroy(_cardView.gameObject);
    }

    public DisplayCardProcess(Card _card, Player _player, bool _isInHand, float _scaleRelativeToParent, RectTransform _cardViewParent, Vector3 _position, bool _animateOpen = true) : base(false, 0)
    {
        card = _card;
        owner = _player;
        cardViewParent = _cardViewParent;
        isInHand = _isInHand;
        scaleRelativeToParent = _scaleRelativeToParent;
        position = _position;
        animateOpen = _animateOpen;
    }

    public override void InvokeProcess()
    {
        SpawnCard(cardView == null, !isInHand);
    }

    private void SpawnCard(bool createCardView = true, bool displayInstantly = true)
    {
        Vector3 offset = Vector3.up * (cardViewParent.rect.height / 7.5f);
        if (!isInHand && animateOpen)
        {
            position += offset;
        }

        if (createCardView)
        {
            if (!PrefabStore.GetPrefabOfType(out CardView prefab)) return; //todo: report errror

            cardView = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, cardViewParent);
        }
        else
        {
            cardView.transform.SetParent(cardViewParent.transform, true);
        }

        RectTransform cardViewTransform = (RectTransform)cardView.transform;

        float scale;
        if (isInHand)
        {
            scale = (cardViewParent.rect.height * scaleRelativeToParent) / cardViewTransform.rect.height;
        }
        else
        {
            if (!animateOpen)
            {
                position = Camera.main.WorldToScreenPoint(owner.character.gameObject.transform.position);
            }

            float verticalSpace = position.y;
            scale = verticalSpace / cardViewTransform.rect.height;
        }

        if (animateOpen)
        {
            cardView.container.transform.localScale = Vector3.zero;
        }

        if (createCardView)
        {
            cardView.transform.localScale = Vector3.one * scale;
        }

        cardView.defaultHeight = cardViewTransform.rect.height;
        cardView.initialLocalScale = Vector3.one * scale;
        cardView.offset = offset;
        cardView.isInHand = isInHand;
        cardView.scaleRelativeToParent = scaleRelativeToParent;
        cardView.animateOpen = animateOpen;

        if (displayInstantly)
        {
            DisplayCard();
        }
    }

    public void DisplayCard()
    {
        cardView.RenderView(card, owner);

        if (!isInHand)
        {
            if (!animateOpen)
            {
                Vector3 newPosition = Camera.main.WorldToScreenPoint(owner.character.gameObject.transform.position);
                newPosition += cardView.offset;

                LeanTween.scale(cardView.gameObject, cardView.initialLocalScale, 0.4f).setEase(LeanTweenType.easeInOutSine);
                LeanTween.scale(cardView.container, Vector3.one, 0.4f).setEase(LeanTweenType.easeInOutSine);
                LeanTween.move(cardView.gameObject, newPosition, 0.4f).setEase(LeanTweenType.easeInOutSine);
                LeanTween.rotate(cardView.gameObject, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInOutSine);
            }
        }

        if (animateOpen)
        {
            if (useDeckPosition)
            {
                cardView.container.transform.position = deckPosition;
            }

            LTDescr showCard = LeanTween.scale(cardView.container, Vector3.one * 0.2f, 0.1f);
            showCard.setOnComplete(() =>
            {
                LeanTween.scale(cardView.container, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutSine);
                if (useDeckPosition)
                {
                    LeanTween.moveLocal(cardView.container, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInOutSine);
                }
            }).setEase(LeanTweenType.easeInSine);
        }

        onCardViewCreated?.Invoke(cardView);
    }

    public void RemoveCardView()
    {
        if (cardView != null)
        {
            LTDescr scaleTween = LeanTween.scale(cardView.container, Vector3.zero, 0.5f);
            scaleTween.setOnComplete(() =>
            {
                UnityEngine.Object.Destroy(cardView.gameObject);
            });
            scaleTween.setEase(LeanTweenType.easeInOutSine);
        }
    }
}
