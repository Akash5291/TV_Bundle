using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Home is the app's landing screen with nowhere further back to go, so
// pressing Back there should exit like a normal Android root activity.
// An open popup takes priority and is left to close itself (see
// CancelToClose) rather than also quitting the app on the same press.
public class AppExitOnBack : MonoBehaviour
{
    [SerializeField] InputActionReference cancelAction;
    [SerializeField] HamburgerManager hamburgerManager;
    [SerializeField] GameObject popupRoot;

    void OnEnable()
    {
        cancelAction?.action.Enable();
    }

    void OnDisable()
    {
        cancelAction?.action.Disable();
    }

    void Update()
    {
        if (cancelAction == null || !cancelAction.action.WasPerformedThisFrame()) return;
        if (popupRoot != null && popupRoot.activeSelf) return;
        if (hamburgerManager != null && !hamburgerManager.IsHomeSection) return;

        if (SceneManager.GetActiveScene().buildIndex == 1)
            Quit();
        else
            SceneManager.LoadSceneAsync("GameListing");
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
