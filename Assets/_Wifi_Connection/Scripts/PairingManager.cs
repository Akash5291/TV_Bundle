using UnityEngine;

public class PairingManager : MonoBehaviour
{
    void Start()
    {
        onSetup();
    }

    void onSetup()
    {
        ActionContainer.onShowPairingScreenUI?.Invoke();
    }
}
