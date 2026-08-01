using UnityEngine;
using UnityEngine.EventSystems;

// Place on whatever UI element receives focus first inside a popup so the
// remote's Back/Cancel button dismisses it, matching Android TV conventions.
public class CancelToClose : MonoBehaviour, ICancelHandler
{
    [SerializeField] ControllerPairingPopup popup;

    public void OnCancel(BaseEventData eventData)
    {
        popup?.Close();
    }
}
