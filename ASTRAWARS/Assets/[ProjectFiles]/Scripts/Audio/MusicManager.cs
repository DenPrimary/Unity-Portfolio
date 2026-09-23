using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance { get; private set; }

    public AudioClip music;
    public float volume = 0.5f;

    private AudioSource aS;

    void Awake() {
        aS = GetComponent<AudioSource>();
    }

    void Start() {
        if (music != null) { 
            aS.clip = music;
            aS.loop = true;
            aS.volume = volume;
            aS.spatialBlend = 0f;
            aS.Play();
        }
    }
}
