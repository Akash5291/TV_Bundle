using System;
using UnityEngine;

public class ActionContainer
{
    public static Action onSplashScreenAnimComplete;

    public static Action onShowPairingScreenUI;
    public static Action onClientConnected;
    public static Action onClientDisconnected;
    //public static Action onQuitGame;

    public static Action<Sprite, SerializableClasses.BundleGameData> onGameSelectToPlay;

    public static Action onStartGame;
}
