using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IView, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Action<CardView> onCardViewSelected;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image typeIconImage;
    [SerializeField] private Image rarityBackgroundImage;
    [SerializeField] private Image coverImage;
    
    public GameObject container;

    [HideInInspector] public float defaultHeight;
    [HideInInspector] public Player owner;
    [HideInInspector] public bool isFocussed;
    [HideInInspector] public bool isInHand;
    [HideInInspector] public Vector3 initialLocalScale;
    [HideInInspector] public Vector3 offset;
    [HideInInspector] public float scaleRelativeToParent;
    [HideInInspector] public bool animateOpen;

    [HideInInspector] public CurvedHorizontalLayout horizontalLayout;
    [HideInInspector] public Card card;

    private bool isHovering = false;

    private void Start()
    {
        horizontalLayout = transform.parent.GetComponent<CurvedHorizontalLayout>();
    }

    public void RenderView(object _value, object _owner)
    {
        card = _value as Card;
        owner = _owner as Player;

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

    private void Update()
    {
        if (isHovering)
        {
            /*RectTransform containerTransform = (RectTransform)container.transform;
            Vector2 difference = (Input.mousePosition - containerTransform.position) / containerTransform.sizeDelta;
            difference *= transform.up;
            difference *= 20f;

            containerTransform.localRotation = Quaternion.Euler(difference.y, difference.x, 0);*/
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isInHand)
        {
            isHovering = true;

            if (horizontalLayout != null)
            {
                horizontalLayout.SetFocusedTransform(container.transform);
                horizontalLayout.focusedContainer.OnPointerClick += OnPointerClick;
                horizontalLayout.focusedContainer.OnPointerExit += OnPointerExit;
            }

            LeanTween.scale(container, Vector3.one * 1.4f, 0.15f).setEase(LeanTweenType.easeInOutSine);

            //RectTransform rectTransform = (RectTransform)transform;
            //Vector3 newPos = rectTransform.up * rectTransform.rect.height / 3f;
            //LeanTween.moveLocal(container, newPos, 0.15f).setEase(LeanTweenType.easeInOutSine);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isInHand)
        {
            isHovering = false;

            if (horizontalLayout != null)
            {
                horizontalLayout.SetFocusedTransform(null);
                horizontalLayout.focusedContainer.OnPointerClick -= OnPointerClick;
                horizontalLayout.focusedContainer.OnPointerExit -= OnPointerExit;
            }

            LeanTween.scale(container, Vector3.one, 0.15f).setEase(LeanTweenType.easeInOutSine);
            LeanTween.rotateLocal(container, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInOutSine);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isInHand && !owner.deck.isPlayingCard)
        {
            if (card.cost <= owner.character.mana)
            {
                isHovering = false;

                if (horizontalLayout != null)
                {
                    horizontalLayout.SetFocusedTransform(null);
                    horizontalLayout.focusedContainer.OnPointerClick -= OnPointerClick;
                    horizontalLayout.focusedContainer.OnPointerExit -= OnPointerExit;
                }

                onCardViewSelected?.Invoke(this);
            }
        }
    }

    public void OnPlayerManaChanged(int mana, int maxMana)
    {
        if (card.cost > mana)
        {
            costText.color = Color.red;
        } else
        {
            costText.color = Color.white;
        }
    }
}
