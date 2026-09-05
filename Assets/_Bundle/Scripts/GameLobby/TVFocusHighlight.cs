using UnityEngine;
using UnityEngine.EventSystems;

// Attach to any Selectable (game card, menu item, button) that should show
// the design's "Selector" frame/glow while focused by the TV remote.
[RequireComponent(typeof(RectTransform))]
public class TVFocusHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] GameObject selector;
    [SerializeField] float focusedScale = 1.08f;
    [SerializeField] float scaleSpeed = 12f;

    RectTransform rect;
    Vector3 baseScale;
    Vector3 targetScale;

    void Awake()
    {
        rect = (RectTransform)transform;
        baseScale = rect.localScale;
        targetScale = baseScale;

        if (selector == null)
        {
            var found = transform.Find("Selector");
            if (found == null && transform.parent != null) found = transform.parent.Find("Selector");
            if (found != null) selector = found.gameObject;
        }

        if (selector != null) selector.SetActive(false);
    }

    void Update()
    {
        if (rect.localScale != targetScale)
            rect.localScale = Vector3.Lerp(rect.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (selector != null) selector.SetActive(true);
        targetScale = baseScale * focusedScale;
        EventSystem.current?.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (selector != null) selector.SetActive(false);
        targetScale = baseScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //EventSystem.current?.SetSelectedGameObject(gameObject);
    }
}
