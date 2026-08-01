using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

[Serializable]
public class LanguageData
{
    public List<LanguageInfo> transition = new List<LanguageInfo>();
}

[Serializable]
public class LanguageInfo
{
    public string lan;
    public string step1_header;
    public string step1_title;
    public string step1_detail;
    public string step2_header;
    public string step2_title;
    public string step2_detail;
}

public class LanguageSelector : MonoBehaviour
{
    public static LanguageSelector Instance = null;

    public bool dataReceived = false;

    [SerializeField] TMP_Text step_1;
    [SerializeField] TMP_Text step_1_title;
    [SerializeField] TMP_Text step_1_info;

    [SerializeField] TMP_Text step_2;
    [SerializeField] TMP_Text step_2_title;
    [SerializeField] TMP_Text step_2_info;

    [SerializeField] LanguageData data;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void onGetTranslatorText()
    {
        StartCoroutine(getTranslator());
    }

    IEnumerator getTranslator()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(APIManager.Instance.base_url + "multi_language/multi_language_info.txt"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("getTranslator error: " + www.error);
            }
            else
            {
                //Debug.Log("Translated info: " + www.downloadHandler.text);
                data = JsonUtility.FromJson<LanguageData>(www.downloadHandler.text);

                updateIntruction(0);
                dataReceived = true;
            }
        }
    }

    public void updateIntruction(int currentLan)
    {
        step_1.text = data.transition[currentLan].step1_header;
        step_2.text = data.transition[currentLan].step2_header;

        step_1_title.text = data.transition[currentLan].step1_title;
        step_2_title.text = data.transition[currentLan].step2_title;

        step_1_info.text = data.transition[currentLan].step1_detail;
        step_2_info.text = data.transition[currentLan].step2_detail;


        if (currentLan == 1)
        {
            step_1_title.fontSize = 23f;
        }
        else if (currentLan == 5)
        {
            step_1_title.fontSize = 18f;
            step_1_info.fontSize = 15f;

            step_2_title.fontSize = 18f;
            step_2_info.fontSize = 15f;
        }
        else if (currentLan == 7)
        {
            step_1_title.fontSize = 24f;
        }
        else
        {
            step_1_title.fontSize = 25f;
            step_1_info.fontSize = 20f;

            step_2_title.fontSize = 25f;
            step_2_info.fontSize = 20f;
        }
    }
    
}
