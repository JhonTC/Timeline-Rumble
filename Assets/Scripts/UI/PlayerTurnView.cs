using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnView : MonoBehaviour, IView
{
    public RectTransform handViewTransform;
    public Button endTurnButton;

    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text deckCountText;
    [SerializeField] private TMP_Text discardCountText;

    public Vector3 deckPosition => deckCountText.transform.position;
    public Vector3 discardPilePosition => discardCountText.transform.position;

    public void RenderView(object _value, object _owner = null)
    {
    }

    public void SetManaText(int manaValue, int maxValue)
    {
        manaText.text = $"{manaValue}/{maxValue}";

        if (manaValue > 0)
        {
            manaText.color = Color.white;
        } else
        {
            manaText.color = Color.red;
        }
    }

    public void SetDeckCountText(int deckCount)
    {
        deckCountText.text = deckCount.ToString();
    }

    public void SetDiscardCountText(int discardCount)
    {
        discardCountText.text = discardCount.ToString();
    }
}
