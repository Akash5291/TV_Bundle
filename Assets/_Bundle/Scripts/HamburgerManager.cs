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

    void Update()
    {
        UpdateSidebarWidth();
        UpdateActiveSectionFromFocus();
    }

    void UpdateSidebarWidth()
    {
        if (sidebar == null) return;

        float targetWidth = IsSidebarFocused() ? expandedWidth : collapsedWidth;
        var size = sidebar.sizeDelta;
        size.x = Mathf.Lerp(size.x, targetWidth, Time.unscaledDeltaTime * resizeSpeed);
        sidebar.sizeDelta = size;
    }

    void UpdateActiveSectionFromFocus()
    {
        if (EventSystem.current == null) return;
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        for (int i = 0; i < sections.Length; i++)
        {
            var button = sections[i].menuButton;
            if (button != null && button.gameObject == selected && i != activeIndex)
            {
                SelectSection(i);
                break;
            }
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
            if (section.contentPanel != null) section.contentPanel.SetActive(active);
        }
    }
}
