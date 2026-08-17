using System;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;

    private AudioSource Source;
    
    [Range(0f,1f)]
    public float volume = 0.7f;
    [Range(0.5f,1.5f)]
    public float pitch = 1f;

    public void SetSource(AudioSource _source)
    {
        Source = _source;
        Source.clip = Clip;
    }

    public void Play()
    {
        Source.volume = volume;
        Source.pitch = pitch;
        Source.Play();
    }
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [SerializeField]private Sound[] sounds;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one AudioManager in the scene.");
        }
        else
        {
            instance = this;
        }
        
    }

    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject go = new GameObject("Sound_ " + i + "_"+ sounds[i].Name);
            go.transform.SetParent(this.transform);
            sounds[i].SetSource(go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(String _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].Name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        //no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list :" + _name);
    }

}
