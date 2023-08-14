using Carp.Collections.Generic;
using Carp.Device;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Dictionary<int, CardView> activeCardViews = new Dictionary<int, CardView>();
    private Dictionary<int, CardView> handCardViews = new Dictionary<int, CardView>();

    public RectTransform cardDisplayParent;
    public RectTransform handDisplayParent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DeviceChange.OnOrientationChange += (orientation) => { RedisplayViews(); };
        DeviceChange.OnResolutionChange += (resolution) => { RedisplayViews(); };
    }

    private void RedisplayViews()
    {
        foreach (var cardView in activeCardViews.Values)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(cardView.owner.character.gameObject.transform.position);
            position += Vector3.up * (cardDisplayParent.rect.height / 10f);
            cardView.transform.position = position;

            float verticalSpace = position.y - (cardDisplayParent.rect.height / 10f);
            float scale = verticalSpace / cardView.defaultHeight;
            cardView.initialLocalScale = Vector3.one * scale;

            LeanTween.scale(cardView.gameObject, cardView.initialLocalScale, 0.5f).setEase(LeanTweenType.easeInOutSine);
        }

        foreach (var cardView in handCardViews.Values)
        {
            float scale = (handDisplayParent.rect.height * cardView.scaleRelativeToParent) / cardView.defaultHeight;
            cardView.initialLocalScale = Vector3.one * scale;

            LeanTween.scale(cardView.gameObject, cardView.initialLocalScale, 0.5f).setEase(LeanTweenType.easeInOutSine);
        }
    }

    public void DisplayCard(Card _card, Player _player)
    {
        Vector3 position = Camera.main.WorldToScreenPoint(_player.character.gameObject.transform.position);

        DisplayCard(_card, _player, false, 0.5f, cardDisplayParent, position);
    }
    public void DisplayCard(CardView _cardView, Player _player)
    {
        _cardView.container.SetActive(false);
        DisplayCard(_cardView.card, _player, _cardView.transform.position, false);
    }
    public void DisplayCard(Card _card, Player _player, Vector3 _position, bool _animateOpen = true)
    {
        DisplayCard(_card, _player, false, 0.5f, cardDisplayParent, _position, _animateOpen);
    }
    
    public void DisplayCard(Card _card, Player _player, bool _isInHand, float _scaleRelativeToParent, RectTransform _parent, Vector3 _position, bool _animateOpen = true, Action<CardView> _onCardViewSelected = null)
    {
        Vector3 position = _position;
        Vector3 offset = Vector3.up * (_parent.rect.height / 10f);
        if (!_isInHand && _animateOpen)
        {
            position += offset;
        }

        if (!PrefabStore.GetPrefabOfType(out CardView prefab)) return; //todo: report errror

        CardView cardView = Instantiate(prefab, position, Quaternion.identity, _parent);
        RectTransform cardViewTransform = (RectTransform)cardView.transform;

        float scale;
        if (_isInHand)
        {
            scale = (_parent.rect.height * _scaleRelativeToParent) / cardViewTransform.rect.height;
        } 
        else
        {
            if (!_animateOpen)
            {
                position = Camera.main.WorldToScreenPoint(_player.character.gameObject.transform.position);
            }

            float verticalSpace = position.y;
            scale = verticalSpace / cardViewTransform.rect.height;
        }

        if (_animateOpen)
        {
            cardView.container.transform.localScale = Vector3.zero;
        }
        cardView.transform.localScale = Vector3.one * scale;

        cardView.defaultHeight = cardViewTransform.rect.height;
        cardView.initialLocalScale = Vector3.one * scale;
        cardView.offset = offset;
        cardView.isInHand = _isInHand;
        cardView.scaleRelativeToParent = _scaleRelativeToParent;
        cardView.animateOpen = _animateOpen;
        cardView.onCardViewSelected = _onCardViewSelected;
        cardView.RenderView(_card, _player);

        if (_isInHand)
        {
            handCardViews.Add(_card.GetHashCode(), cardView);
        } 
        else
        {
            activeCardViews.Add(_card.GetHashCode(), cardView);

            if (!_animateOpen)
            {
                Vector3 newPosition = Camera.main.WorldToScreenPoint(_player.character.gameObject.transform.position);
                newPosition += offset;

                LeanTween.scale(cardView.gameObject, cardView.initialLocalScale, 0.15f).setEase(LeanTweenType.easeInOutSine);
                LeanTween.move(cardView.gameObject, newPosition, 0.15f).setEase(LeanTweenType.easeInOutSine);
            }
        }

        if (_animateOpen)
        {
            LeanTween.scale(cardView.container, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutSine);
        }
    }

    public void DisplayPlayerHand(Player _player, Action<CardView> _onCardViewSelected)
    {
        for (int i = 0; i < _player.hand.cards.Count; i++)
        {
            DisplayCard(_player.hand.cards[i], _player, true, 1, handDisplayParent, Vector3.zero, true, _onCardViewSelected);
        }
    }

    public void ClearHandCardViews()
    {
        foreach (CardView cardView in handCardViews.Values)
        {
            Destroy(cardView.gameObject);
        }

        handCardViews.Clear();
    }

    public void RemoveCardView(Card _card)
    {
        int cardHash = _card.GetHashCode();
        if (activeCardViews.ContainsKey(cardHash))
        {
            LTDescr scaleTween = LeanTween.scale(activeCardViews[cardHash].container, Vector3.zero, 0.5f);
            scaleTween.setOnComplete(() =>
            {     
                Destroy(activeCardViews[cardHash].gameObject);
                activeCardViews.Remove(cardHash);
            });
            scaleTween.setEase(LeanTweenType.easeInOutSine);
        }

        if (handCardViews.ContainsKey(cardHash))
        {
            LTDescr scaleTween = LeanTween.scale(handCardViews[cardHash].container, Vector3.zero, 0.5f);
            scaleTween.setOnComplete(() =>
            {
                Destroy(handCardViews[cardHash].gameObject);
                handCardViews.Remove(cardHash);
            });
            scaleTween.setEase(LeanTweenType.easeInOutSine);
        }
    } 
}

[Serializable]
public class CharacterTypeColourPair : Pair<CharacterType, Color> {}

[Serializable]
public class PrefabStore : MonoBehaviour
{
    public static PrefabStore Instance;
    private static Dictionary<Type, object> prefabs;

    [SerializeField]
    private CardView cardView;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }

        if (Instance == this)
        {
            prefabs = new Dictionary<Type, object>()
            {
                { typeof(CardView), cardView }
            };
        }
    }

    public static bool GetPrefabOfType<T>(out T value)
    {
        if (prefabs.ContainsKey(typeof(T)))
        {
            value = (T)prefabs[typeof(T)];
            return true;
        }

        Debug.LogWarning($"No prefab exists of type {typeof(T)}");
        value = default;
        return false;
    }
}