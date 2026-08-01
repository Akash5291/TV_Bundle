using UnityEngine;

public class DebugLog : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
  Debug.unityLogger.logEnabled = false;
#endif
    }

}
