using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(RectTransform))]
public class CurvedHorizontalLayout : MonoBehaviour
{
    [SerializeField] private float spacing;
    [SerializeField] private float archAngle = 50f;

    [SerializeField] private bool useChildWidth;

    [SerializeField] private int maxChildCount = 8;

    private int lastChildCount;

    private RectTransform rectTransform;

    public HorizontalLayoutFocusView focusedContainer;

    public Transform activeFocusedTransform;
    public RectTransform activeFocusedOriginalParent;

    public Vector2 childPivot = new Vector2(0.05f, 0.05f);

    private void Start()
    {
        rectTransform = (RectTransform)transform;

        lastChildCount = transform.childCount;

        GameObject focusedContainerObject = new GameObject("FocusedContainer");
        focusedContainerObject.transform.SetParent(transform);
        focusedContainerObject.AddComponent<RectTransform>();
        focusedContainer = focusedContainerObject.AddComponent<HorizontalLayoutFocusView>();
    }

    private void Update()
    {
        if (lastChildCount != transform.childCount)
        {
            RenderView();

            lastChildCount = transform.childCount;
        }
    }

    public void SetFocusedTransform(Transform _activeFocusedTransform)
    {
        if (activeFocusedTransform != null)
        {
            activeFocusedTransform.SetParent(activeFocusedOriginalParent);
        }

        if (_activeFocusedTransform != null)
        {
            activeFocusedTransform = _activeFocusedTransform;
            activeFocusedOriginalParent = (RectTransform)activeFocusedTransform.parent;

            RectTransform focusedContainerTransform = (RectTransform)focusedContainer.transform;
            focusedContainerTransform.pivot = activeFocusedOriginalParent.pivot;
            focusedContainerTransform.position = activeFocusedOriginalParent.position;
            focusedContainerTransform.rotation = activeFocusedOriginalParent.rotation;
            focusedContainerTransform.localScale = activeFocusedOriginalParent.localScale;

            activeFocusedTransform.SetParent(focusedContainer.transform);
        }
    }

    public void RenderView()
    {
        focusedContainer.transform.SetAsLastSibling();

        float width = 0;

        RectTransform[] childTransforms = GetChildTransforms();

        for (int i = 0; i < childTransforms.Length; i++)
        {
            childTransforms[i].pivot = childPivot;

            if (useChildWidth)
            {
                width += childTransforms[i].rect.width;
            }
        }

        width += spacing * (childTransforms.Length - 1);

        float halfwidth = width / 2f;
        float currentX = -halfwidth;

        for (int i = 0; i < childTransforms.Length; i++)
        {
            if (useChildWidth)
            {
                currentX += childTransforms[i].rect.width / 2f;
            }
            if (i != 0)
            {
                currentX += spacing;
            }

            childTransforms[i].localPosition = Vector3.right * currentX;
        }

        float difference = 0f;
        float halfChildCount = (transform.childCount - 2) / 2f;
        if (halfChildCount > 0)
        {
            difference = archAngle / halfChildCount;
        }

        float currentAngle = 0;
        if (transform.childCount > 0)
        {
            currentAngle = -halfChildCount * difference;
        }

        for (int i = 0; i < childTransforms.Length; i++)
        {
            childTransforms[i].Rotate(Vector3.forward, currentAngle);

            childTransforms[i].transform.localPosition += childTransforms[i].transform.up * -Mathf.Abs(currentAngle * 2f); ;

            currentAngle += difference;
        }
    }

    private RectTransform[] GetChildTransforms()
    {
        List<RectTransform> returnList = new List<RectTransform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform childTransform = (RectTransform)transform.GetChild(i);

            if (childTransform.gameObject != focusedContainer.gameObject)
            {
                returnList.Add(childTransform);
            }
        }

        return returnList.ToArray();
    }
}
