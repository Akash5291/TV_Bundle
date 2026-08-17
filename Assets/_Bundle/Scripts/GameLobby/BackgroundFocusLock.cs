using UnityEngine;

// Prevents D-pad Automatic navigation from leaking into background UI while
// a full-screen modal (QR popup, game detail) is open. Modals in this scene
// are siblings of the main content rather than something that structurally
// isolates it, so without this, Unity's navigation candidate search - which
// only looks at screen position, not what's drawn on top - can jump focus
// straight into hidden buttons underneath. Unity's Selectable.IsInteractable()
// respects CanvasGroup.interactable on ancestors, which is enough on its own
// to exclude those buttons from navigation; no raycast/visual blocking needed.
public class BackgroundFocusLock : MonoBehaviour
{
    [SerializeField] CanvasGroup[] backgroundGroups;

    public void SetLocked(bool locked)
    {
        foreach (var group in backgroundGroups)
        {
            if (group != null) group.interactable = !locked;
        }
    }
}
