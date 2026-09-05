using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LudoSearchingPlayer : MonoBehaviour
{
    [SerializeField] TMP_Text text_info;

    private void OnEnable()
    {
        StartCoroutine(searchPlayer());
    }

    IEnumerator searchPlayer()
    {
        text_info.text = "Connecting to Server..";
        yield return new WaitForSeconds(2f);
        text_info.text = "Searching players...";
        yield return new WaitForSeconds(3f);
        text_info.text = "Preparing Game...";
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Ludo_Game");
    }
}
