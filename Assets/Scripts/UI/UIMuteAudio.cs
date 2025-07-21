using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
public class UIMuteAudio : MonoBehaviour
{
    [SerializeField] private Button muteButton;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string volumeParameter = "MasterVolume";
    private bool isMuted = false;
    private Image buttonImage;
    private float previousVolume = 1f;
    private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();
    void Start()
    {
        if (muteButton == null)
            muteButton = GetComponent<Button>();
        buttonImage = muteButton.GetComponent<Image>();
        StoreOriginalVolumes();
        muteButton.onClick.AddListener(ToggleMute);
        LoadMuteState();
        UpdateButtonImage();
    }
    void StoreOriginalVolumes()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (!originalVolumes.ContainsKey(source))
            {
                originalVolumes.Add(source, source.volume);
                Debug.Log($"Stored original volume for audio source: {source.gameObject.name} = {source.volume}");
            }
        }
    }
    void ToggleMute()
    {
        isMuted = !isMuted;
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (!originalVolumes.ContainsKey(source))
            {
                originalVolumes.Add(source, source.volume);
            }
            if (isMuted)
                source.volume = 0f;
            else
                source.volume = originalVolumes[source];
        }
        if (isMuted)
        {
            if (audioMixer != null)
            {
                audioMixer.GetFloat(volumeParameter, out previousVolume);
                audioMixer.SetFloat(volumeParameter, -80f);
                Debug.Log("Muting audio mixer: " + volumeParameter + " set to -80dB");
            }
            else
            {
                previousVolume = AudioListener.volume;
                AudioListener.volume = 0f;
                Debug.Log("Setting AudioListener.volume to 0");
            }
        }
        else
        {
            if (audioMixer != null)
            {
                audioMixer.SetFloat(volumeParameter, previousVolume);
                Debug.Log("Unmuting audio mixer: " + volumeParameter + " set to " + previousVolume);
            }
            else
            {
                AudioListener.volume = previousVolume > 0 ? previousVolume : 1f;
                Debug.Log("Setting AudioListener.volume to " + AudioListener.volume);
            }
        }
        SaveMuteState();
        UpdateButtonImage();
    }
    void UpdateButtonImage()
    {
        if (buttonImage != null && soundOnSprite != null && soundOffSprite != null)
        {
            buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }
    void SaveMuteState()
    {
        PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0);
        PlayerPrefs.SetFloat("PreviousVolume", previousVolume);
        PlayerPrefs.Save();
    }
    void LoadMuteState()
    {
        StoreOriginalVolumes();
        if (PlayerPrefs.HasKey("AudioMuted"))
        {
            isMuted = PlayerPrefs.GetInt("AudioMuted") == 1;
            previousVolume = PlayerPrefs.GetFloat("PreviousVolume", 1f);
            if (isMuted)
            {
                AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource source in allAudioSources)
                {
                    source.volume = 0f;
                }
                if (audioMixer != null)
                    audioMixer.SetFloat(volumeParameter, -80f);
                else
                    AudioListener.volume = 0f;
            }
            else
            {
                AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource source in allAudioSources)
                {
                    if (originalVolumes.ContainsKey(source))
                        source.volume = originalVolumes[source];
                }
                if (audioMixer != null)
                    audioMixer.SetFloat(volumeParameter, previousVolume);
                else
                    AudioListener.volume = previousVolume > 0 ? previousVolume : 1f;
            }
        }
        else
        {
            isMuted = false;
            if (audioMixer != null)
                audioMixer.SetFloat(volumeParameter, 0f);
            else
                AudioListener.volume = 1f;
            Debug.Log("No saved audio state - defaulting to audio enabled with original volumes");
        }
    }
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        StoreOriginalVolumes();
        if (isMuted)
        {
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in allAudioSources)
            {
                source.volume = 0f;
            }
        }
    }
}