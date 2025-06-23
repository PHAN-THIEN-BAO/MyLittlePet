using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip firstAudioClip;      // play once at the start
    public AudioClip infiniteAudioClip;   // play infinitely after the first clip
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;
    private bool playedFirst = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    void Start()
    {
        Play();
    }

    public void Play()
    {
        playedFirst = false;
        if (firstAudioClip != null)
        {
            audioSource.clip = firstAudioClip;
            audioSource.loop = false;
            audioSource.volume = volume;
            audioSource.Play();
            playedFirst = true;
        }
        else if (infiniteAudioClip != null)
        {
            PlayInfinite();
        }
    }

    void Update()
    {
        // if the first audio clip has finished playing, switch to the infinite audio clip
        if (playedFirst && !audioSource.isPlaying)
        {
            PlayInfinite();
            playedFirst = false;
        }
    }

    private void PlayInfinite()
    {
        if (infiniteAudioClip != null)
        {
            audioSource.clip = infiniteAudioClip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    public void Stop()
    {
        audioSource.Stop();
        playedFirst = false;
    }
}
