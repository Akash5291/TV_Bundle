using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach directly to a ScrollRect (New Arrival / Other Games rows). Unity's
// UI navigation moves focus between items but never scrolls the ScrollRect
// itself, so off-screen cards would be unreachable by remote without this.
[RequireComponent(typeof(ScrollRect))]
public class CarouselAutoScroll : MonoBehaviour
{
    [SerializeField] float padding = 40f;
    [SerializeField] float scrollSpeed = 10f;

    ScrollRect scrollRect;
    RectTransform viewport;
    RectTransform content;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
        content = scrollRect.content;
    }

    void LateUpdate()
    {
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected == null || content == null) return;

        var item = selected.transform as RectTransform;
        if (item == null || !item.IsChildOf(content)) return;

        if (scrollRect.horizontal) ScrollToward(item, horizontal: true);
        if (scrollRect.vertical) ScrollToward(item, horizontal: false);
    }

    void ScrollToward(RectTransform item, bool horizontal)
    {
        var itemCorners = new Vector3[4];
        item.GetWorldCorners(itemCorners);
        var viewCorners = new Vector3[4];
        viewport.GetWorldCorners(viewCorners);

        float delta = horizontal
            ? HorizontalDelta(itemCorners, viewCorners)
            : VerticalDelta(itemCorners, viewCorners);

        if (Mathf.Approximately(delta, 0f)) return;

        var targetPos = content.position;
        if (horizontal) targetPos.x -= delta; else targetPos.y -= delta;
        content.position = Vector3.Lerp(content.position, targetPos, Time.unscaledDeltaTime * scrollSpeed);
    }

    float HorizontalDelta(Vector3[] itemCorners, Vector3[] viewCorners)
    {
        if (itemCorners[0].x < viewCorners[0].x + padding)
            return itemCorners[0].x - (viewCorners[0].x + padding);
        if (itemCorners[2].x > viewCorners[2].x - padding)
            return itemCorners[2].x - (viewCorners[2].x - padding);
        return 0f;
    }

    float VerticalDelta(Vector3[] itemCorners, Vector3[] viewCorners)
    {
        if (itemCorners[2].y > viewCorners[2].y - padding)
            return itemCorners[2].y - (viewCorners[2].y - padding);
        if (itemCorners[0].y < viewCorners[0].y + padding)
            return itemCorners[0].y - (viewCorners[0].y + padding);
        return 0f;
    }
}
