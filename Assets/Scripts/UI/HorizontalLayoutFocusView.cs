using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalLayoutFocusView : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    public Action<PointerEventData> OnPointerExit;
    public Action<PointerEventData> OnPointerClick;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        OnPointerClick?.Invoke(eventData);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit?.Invoke(eventData);
    }
}
