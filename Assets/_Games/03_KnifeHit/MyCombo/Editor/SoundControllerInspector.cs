using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundNinjaKnife))]
[CanEditMultipleObjects]
public class SoundControllerInspector : BaseInspector
{
    public SerializedProperty buttonClips;
    public SerializedProperty otherClips;
    private void OnEnable()
    {
        buttonClips = serializedObject.FindProperty("buttonClips");
        otherClips = serializedObject.FindProperty("otherClips");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        ShowArrayProperty(buttonClips, typeof(SoundNinjaKnife.Button), "Button Clips");
        ShowArrayProperty(otherClips, typeof(SoundNinjaKnife.Others), "Other Clips");

        serializedObject.ApplyModifiedProperties();
    }
}