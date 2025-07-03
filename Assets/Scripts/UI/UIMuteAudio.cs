using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIMuteAudio : MonoBehaviour
{
    [SerializeField] private Button muteButton;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private AudioMixer audioMixer; // Optional AudioMixer reference
    [SerializeField] private string volumeParameter = "MasterVolume"; // Parameter name in AudioMixer

    private bool isMuted = false;
    private Image buttonImage;
    private float previousVolume = 0f;

    void Start()
    {
        // Auto-detect button component if not assigned
        if (muteButton == null)
            muteButton = GetComponent<Button>();

        // Get image component for sprite swapping
        buttonImage = muteButton.GetComponent<Image>();

        // Register click handler
        muteButton.onClick.AddListener(ToggleMute);

        // Restore previous mute state if available
        LoadMuteState();

        // Set initial button appearance
        UpdateButtonImage();
    }

    void ToggleMute()
    {
        isMuted = !isMuted;

        // Mute/unmute all audio sources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (isMuted)
                source.volume = 0f;
            else
                source.volume = 1f;
        }

        // Handle AudioMixer or AudioListener as appropriate
        if (isMuted)
        {
            // Store current volume before muting
            if (audioMixer != null)
            {
                audioMixer.GetFloat(volumeParameter, out previousVolume);
                audioMixer.SetFloat(volumeParameter, -80f); // -80dB is effectively silent
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
            // Restore previous volume
            if (audioMixer != null)
            {
                audioMixer.SetFloat(volumeParameter, previousVolume);
                Debug.Log("Unmuting audio mixer: " + volumeParameter + " set to " + previousVolume);
            }
            else
            {
                AudioListener.volume = previousVolume;
                Debug.Log("Setting AudioListener.volume to " + previousVolume);
            }
        }

        // Save state for future sessions
        SaveMuteState();

        // Update button visual state
        UpdateButtonImage();
    }

    void UpdateButtonImage()
    {
        // Change button sprite based on mute state
        if (buttonImage != null && soundOnSprite != null && soundOffSprite != null)
        {
            buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }

    // Save mute state to player preferences
    void SaveMuteState()
    {
        PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0);
        PlayerPrefs.SetFloat("PreviousVolume", previousVolume);
        PlayerPrefs.Save();
    }

    // Load mute state from player preferences
    void LoadMuteState()
    {
        if (PlayerPrefs.HasKey("AudioMuted"))
        {
            isMuted = PlayerPrefs.GetInt("AudioMuted") == 1;
            previousVolume = PlayerPrefs.GetFloat("PreviousVolume");

            // Apply loaded mute state
            if (isMuted)
            {
                if (audioMixer != null)
                    audioMixer.SetFloat(volumeParameter, -80f);
                else
                    AudioListener.volume = 0f;
            }
        }
    }
}