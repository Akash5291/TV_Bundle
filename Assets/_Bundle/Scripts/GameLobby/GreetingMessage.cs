using System;
using TMPro;
using UnityEngine;

public class GreetingMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text greetingText;

    private void Start()
    {
        UpdateGreeting();
    }

    private void UpdateGreeting()
    {
        int hour = DateTime.Now.Hour;

        if (hour >= 5 && hour < 12)
        {
            greetingText.text = "Good morning";
        }
        else if (hour >= 12 && hour < 17)
        {
            greetingText.text = "Good afternoon";
        }
        else if (hour >= 17 && hour < 21)
        {
            greetingText.text = "Good evening";
        }
        else
        {
            greetingText.text = "Good night";
        }
    }
}