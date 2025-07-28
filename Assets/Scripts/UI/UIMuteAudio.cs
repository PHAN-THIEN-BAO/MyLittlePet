////using UnityEngine;
////using UnityEngine.UI;
////using UnityEngine.Audio;

////public class UIMuteAudio : MonoBehaviour
////{
////    [SerializeField] private Button muteButton;
////    [SerializeField] private Sprite soundOnSprite;
////    [SerializeField] private Sprite soundOffSprite;
////    [SerializeField] private AudioMixer audioMixer; // Optional AudioMixer reference
////    [SerializeField] private string volumeParameter = "MasterVolume"; // Parameter name in AudioMixer

////    private bool isMuted = false;
////    private Image buttonImage;
////    private float previousVolume = 0f;

////    void Start()
////    {
////        // Auto-detect button component if not assigned
////        if (muteButton == null)
////            muteButton = GetComponent<Button>();

////        // Get image component for sprite swapping
////        buttonImage = muteButton.GetComponent<Image>();

////        // Register click handler
////        muteButton.onClick.AddListener(ToggleMute);

////        // Restore previous mute state if available
////        LoadMuteState();

////        // Set initial button appearance
////        UpdateButtonImage();
////    }

////    void ToggleMute()
////    {
////        isMuted = !isMuted;

////        // Mute/unmute all audio sources in the scene
////        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
////        foreach (AudioSource source in allAudioSources)
////        {
////            if (isMuted)
////                source.volume = 0f;
////            else
////                source.volume = 1f;
////        }

////        // Handle AudioMixer or AudioListener as appropriate
////        if (isMuted)
////        {
////            // Store current volume before muting
////            if (audioMixer != null)
////            {
////                audioMixer.GetFloat(volumeParameter, out previousVolume);
////                audioMixer.SetFloat(volumeParameter, -80f); // -80dB is effectively silent
////                Debug.Log("Muting audio mixer: " + volumeParameter + " set to -80dB");
////            }
////            else
////            {
////                previousVolume = AudioListener.volume;
////                AudioListener.volume = 0f;
////                Debug.Log("Setting AudioListener.volume to 0");
////            }
////        }
////        else
////        {
////            // Restore previous volume
////            if (audioMixer != null)
////            {
////                audioMixer.SetFloat(volumeParameter, previousVolume);
////                Debug.Log("Unmuting audio mixer: " + volumeParameter + " set to " + previousVolume);
////            }
////            else
////            {
////                AudioListener.volume = previousVolume;
////                Debug.Log("Setting AudioListener.volume to " + previousVolume);
////            }
////        }

////        // Save state for future sessions
////        SaveMuteState();

////        // Update button visual state
////        UpdateButtonImage();
////    }

////    void UpdateButtonImage()
////    {
////        // Change button sprite based on mute state
////        if (buttonImage != null && soundOnSprite != null && soundOffSprite != null)
////        {
////            buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
////        }
////    }

////    // Save mute state to player preferences
////    void SaveMuteState()
////    {
////        PlayerPrefs.SetInt("AudioMuted", isMuted ? 0 : 1);
////        PlayerPrefs.SetFloat("PreviousVolume", previousVolume);
////        PlayerPrefs.Save();
////    }

////    // Load mute state from player preferences
////    void LoadMuteState()
////    {
////        if (PlayerPrefs.HasKey("AudioMuted"))
////        {
////            isMuted = PlayerPrefs.GetInt("AudioMuted") == 1;
////            previousVolume = PlayerPrefs.GetFloat("PreviousVolume");

////            // Apply loaded mute state
////            if (isMuted)
////            {
////                if (audioMixer != null)
////                    audioMixer.SetFloat(volumeParameter, -80f);
////                else
////                    AudioListener.volume = 0f;
////            }
////        }
////    }
////}
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Audio;

//public class UIMuteAudio : MonoBehaviour
//{
//    [SerializeField] private Button muteButton;
//    [SerializeField] private Sprite soundOnSprite;
//    [SerializeField] private Sprite soundOffSprite;
//    [SerializeField] private AudioMixer audioMixer; // Optional AudioMixer reference
//    [SerializeField] private string volumeParameter = "MasterVolume"; // Parameter name in AudioMixer

//    private bool isMuted = false;
//    private Image buttonImage;
//    private float previousVolume = 1f; // Default to full volume

//    void Start()
//    {
//        // Auto-detect button component if not assigned
//        if (muteButton == null)
//            muteButton = GetComponent<Button>();

//        // Get image component for sprite swapping
//        buttonImage = muteButton.GetComponent<Image>();

//        // Register click handler
//        muteButton.onClick.AddListener(ToggleMute);

//        // Always start with audio enabled, only apply mute if explicitly saved
//        LoadMuteState();

//        // Set initial button appearance
//        UpdateButtonImage();
//    }

//    void ToggleMute()
//    {
//        isMuted = !isMuted;

//        // Mute/unmute all audio sources in the scene
//        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
//        foreach (AudioSource source in allAudioSources)
//        {
//            if (isMuted)
//                source.volume = 0f;
//            else
//                source.volume = 1f;
//        }

//        // Handle AudioMixer or AudioListener as appropriate
//        if (isMuted)
//        {
//            // Store current volume before muting
//            if (audioMixer != null)
//            {
//                audioMixer.GetFloat(volumeParameter, out previousVolume);
//                audioMixer.SetFloat(volumeParameter, -80f); // -80dB is effectively silent
//                Debug.Log("Muting audio mixer: " + volumeParameter + " set to -80dB");
//            }
//            else
//            {
//                previousVolume = AudioListener.volume;
//                AudioListener.volume = 0f;
//                Debug.Log("Setting AudioListener.volume to 0");
//            }
//        }
//        else
//        {
//            // Restore previous volume
//            if (audioMixer != null)
//            {
//                audioMixer.SetFloat(volumeParameter, previousVolume);
//                Debug.Log("Unmuting audio mixer: " + volumeParameter + " set to " + previousVolume);
//            }
//            else
//            {
//                AudioListener.volume = previousVolume > 0 ? previousVolume : 1f;
//                Debug.Log("Setting AudioListener.volume to " + AudioListener.volume);
//            }
//        }

//        // Save state for future sessions
//        SaveMuteState();

//        // Update button visual state
//        UpdateButtonImage();
//    }

//    void UpdateButtonImage()
//    {
//        // Change button sprite based on mute state
//        if (buttonImage != null && soundOnSprite != null && soundOffSprite != null)
//        {
//            buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
//        }
//    }

//    // Save mute state to player preferences
//    void SaveMuteState()
//    {
//        PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0); // 1 when muted, 0 when not muted
//        PlayerPrefs.SetFloat("PreviousVolume", previousVolume);
//        PlayerPrefs.Save();
//    }

//    // Load mute state from player preferences
//    void LoadMuteState()
//    {
//        if (PlayerPrefs.HasKey("AudioMuted"))
//        {
//            isMuted = PlayerPrefs.GetInt("AudioMuted") == 1; // 1 means muted
//            previousVolume = PlayerPrefs.GetFloat("PreviousVolume", 1f);

//            // Apply loaded mute state
//            if (isMuted)
//            {
//                if (audioMixer != null)
//                    audioMixer.SetFloat(volumeParameter, -80f);
//                else
//                    AudioListener.volume = 0f;
//            }
//            else
//            {
//                // Ensure audio is on by default
//                if (audioMixer != null)
//                    audioMixer.SetFloat(volumeParameter, previousVolume);
//                else
//                    AudioListener.volume = previousVolume > 0 ? previousVolume : 1f;
//            }
//        }
//        else
//        {
//            // No saved state - default to unmuted
//            isMuted = false;

//            // Ensure audio is enabled
//            if (audioMixer != null)
//                audioMixer.SetFloat(volumeParameter, 0f);
//            else
//                AudioListener.volume = 1f;

//            Debug.Log("No saved audio state - defaulting to audio enabled");
//        }
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class UIMuteAudio : MonoBehaviour
{
    [SerializeField] private Button muteButton;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private AudioMixer audioMixer; // Optional AudioMixer reference
    [SerializeField] private string volumeParameter = "MasterVolume"; // Parameter name in AudioMixer

    private bool isMuted = false;
    private Image buttonImage;
    private float previousVolume = 1f; // Default to full volume

    // Store original volumes for all audio sources
    private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();

    void Start()
    {
        // Auto-detect button component if not assigned
        if (muteButton == null)
            muteButton = GetComponent<Button>();

        // Get image component for sprite swapping
        buttonImage = muteButton.GetComponent<Image>();

        // Store original volumes of all audio sources at start
        StoreOriginalVolumes();

        // Register click handler
        muteButton.onClick.AddListener(ToggleMute);

        // Always start with audio enabled, only apply mute if explicitly saved
        LoadMuteState();

        // Set initial button appearance
        UpdateButtonImage();
    }

    void StoreOriginalVolumes()
    {
        // Store the original volume of all audio sources
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

        // Check for any new audio sources that weren't present at start
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (!originalVolumes.ContainsKey(source))
            {
                originalVolumes.Add(source, source.volume);
            }

            // Mute/unmute based on the stored original volume
            if (isMuted)
                source.volume = 0f;
            else
                source.volume = originalVolumes[source]; // Restore original volume
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
                AudioListener.volume = previousVolume > 0 ? previousVolume : 1f;
                Debug.Log("Setting AudioListener.volume to " + AudioListener.volume);
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
        PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0); // 1 when muted, 0 when not muted
        PlayerPrefs.SetFloat("PreviousVolume", previousVolume);
        PlayerPrefs.Save();
    }

    // Load mute state from player preferences
    void LoadMuteState()
    {
        // Store current volumes before potentially changing them
        StoreOriginalVolumes();

        if (PlayerPrefs.HasKey("AudioMuted"))
        {
            isMuted = PlayerPrefs.GetInt("AudioMuted") == 1; // 1 means muted
            previousVolume = PlayerPrefs.GetFloat("PreviousVolume", 1f);

            // Apply loaded mute state
            if (isMuted)
            {
                // Apply mute to all audio sources
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
                // Ensure audio is on by default with original volumes
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
            // No saved state - default to unmuted with original volumes
            isMuted = false;

            // No need to modify audio source volumes as they are already at their original values

            // Ensure audio is enabled
            if (audioMixer != null)
                audioMixer.SetFloat(volumeParameter, 0f);
            else
                AudioListener.volume = 1f;

            Debug.Log("No saved audio state - defaulting to audio enabled with original volumes");
        }
    }

    // When new objects with audio sources are created at runtime
    void OnEnable()
    {
        // Subscribe to scene loaded event to capture audio sources in new scenes
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe when disabled
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // After a new scene loads, store original volumes for any new audio sources
        StoreOriginalVolumes();

        // Apply current mute state to new audio sources
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