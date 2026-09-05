using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Drives the left nav drawer: expands on focus, tracks the active section,
// and swaps the visible content panel as the remote's D-pad moves between items.
public class HamburgerManager : MonoBehaviour
{
    [System.Serializable]
    public class MenuSection
    {
        public Selectable menuButton;
        public GameObject selector;
        public GameObject contentPanel;
    }

    [SerializeField] MenuSection[] sections;
    [SerializeField] RectTransform sidebar;
    [SerializeField] float collapsedWidth = 120f;
    [SerializeField] float expandedWidth = 300f;
    [SerializeField] float resizeSpeed = 12f;
    [SerializeField] int defaultSection = 0;

    int activeIndex = -1;

    public bool IsHomeSection => activeIndex == 0;

    void Start()
    {
        SelectSection(defaultSection);

        // Android TV remotes only send D-pad events to whatever is already selected,
        // so without this the very first frame has nothing to move focus onto.
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null
            && sections.Length > 0 && sections[0].menuButton != null)
        {
            EventSystem.current.SetSelectedGameObject(sections[0].menuButton.gameObject);
        }
    }

    bool IsSidebarFocused()
    {
        if (EventSystem.current == null || sidebar == null) return false;
        var selected = EventSystem.current.currentSelectedGameObject;
        return selected != null && selected.transform.IsChildOf(sidebar);
    }

    public void SelectSection(int index)
    {
        if (index < 0 || index >= sections.Length) return;
        activeIndex = index;

        for (int i = 0; i < sections.Length; i++)
        {
            bool active = i == index;
            var section = sections[i];
            if (section.selector != null) section.selector.SetActive(active);
            if (section.contentPanel != null)
            {
                section.contentPanel.SetActive(active);
                if (active) ResetScroll(section.contentPanel);
            }
        }
    }

    // Coming back to a section (e.g. Home) should land at the top, not
    // wherever the user last scrolled to before switching away.
    static void ResetScroll(GameObject panel)
    {
        var scrollRect = panel.GetComponent<ScrollRect>();
        if (scrollRect == null) return;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
        scrollRect.horizontalNormalizedPosition = 0f;
    }
}
