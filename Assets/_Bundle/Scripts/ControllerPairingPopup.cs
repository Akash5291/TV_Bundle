using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Drives the "Download Controller App" QR popup: opens from the top banner
// button and restores whatever had focus before the popup opened so the
// user doesn't lose their place when it closes.
public class ControllerPairingPopup : MonoBehaviour
{
    [SerializeField] GameObject popupRoot;
    [SerializeField] GameObject firstSelected;
    [SerializeField] Image qrImage;
    [SerializeField] TMP_Text instructionsText;
    [SerializeField] BackgroundFocusLock backgroundLock;

    GameObject previouslySelected;

    void Awake()
    {
        if (popupRoot != null) popupRoot.SetActive(false);
    }

    public void Open()
    {
        if (popupRoot == null) return;

        previouslySelected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        popupRoot.SetActive(true);
        backgroundLock?.SetLocked(true);

        var target = firstSelected != null ? firstSelected : popupRoot;
        EventSystem.current?.SetSelectedGameObject(target);
    }

    public void Close()
    {
        if (popupRoot == null) return;

        popupRoot.SetActive(false);
        backgroundLock?.SetLocked(false);
        EventSystem.current?.SetSelectedGameObject(previouslySelected);
    }

    public void SetQrCode(Sprite sprite)
    {
        if (qrImage != null) qrImage.sprite = sprite;
    }

    public void SetInstructions(string text)
    {
        if (instructionsText != null) instructionsText.text = text;
    }
}
