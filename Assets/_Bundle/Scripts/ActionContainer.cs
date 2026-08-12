using System;
using UnityEngine;
using static SerializableClasses;

public class ActionContainer
{
    public static Action onSplashScreenAnimComplete;
    public static Action<Sprite, BundleGameData> onGameSelectToPlay;
}
