using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource EffectAudioSource;
    [SerializeField] private AudioClip backGroundClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip coinClip;

    
    void Start()
    {
        PlayBackgroundMusic();
    }

    

    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.clip = backGroundClip;
        backgroundAudioSource.Play();
    }
    public void PlaycoinSound()
    {
        EffectAudioSource.PlayOneShot(coinClip);
    }
    public void PlayJumpSound()
    {
        EffectAudioSource.PlayOneShot(jumpClip);
    }
}