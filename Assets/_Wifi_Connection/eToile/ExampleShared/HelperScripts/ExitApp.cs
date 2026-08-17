using UnityEngine;

public class ExitApp : MonoBehaviour
{
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            print("Quit app");
        }
    }
}
