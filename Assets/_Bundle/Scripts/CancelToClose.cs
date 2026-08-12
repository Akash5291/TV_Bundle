using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// Place on whatever UI element receives focus first inside a closeable
// panel so the remote's Back/Cancel button dismisses it, matching Android
// TV conventions. onCancel is wired per-instance in the Inspector, so the
// same component works for any panel's own close method.
public class CancelToClose : MonoBehaviour, ICancelHandler
{
    [SerializeField] UnityEvent onCancel;

    public void OnCancel(BaseEventData eventData)
    {
        onCancel?.Invoke();
    }
}
