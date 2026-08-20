using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    bool sourceFound = false;
    public void B_Play()
    {
        SceneManager.LoadScene("Robot_Run_Gameplay");
    }

    private void Update()
    {
        if (sourceFound)
            return;

        if (BackgroundSoundManager.instance)
        {
            Debug.Log("Finding...");
            BackgroundSoundManager.instance.audioSource.volume = 0.39f;
            sourceFound = true;
        }
    }
}
