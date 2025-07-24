using UnityEngine;
using UnityEngine.EventSystems;

public class PetClickHandler : MonoBehaviour
{
    public PetInfoUIManager uiManager;

    [Header("Pet Click Audio Settings")]
    [Tooltip("Enable pet click sound effects")]
    public bool enableClickAudio = true;
    [Tooltip("Enable random pitch variation for pet sounds")]
    public bool randomPitch = true;
    [Tooltip("Volume for pet click sounds")]
    [Range(0f, 1f)]
    public float clickSoundVolume = 0.8f;
    [Tooltip("Array of direct audio clips for each petID (fallback if SoundEffectManager not available)")]
    public AudioClip[] petClickAudioClips;

    private void OnMouseDown()
    {
        Debug.Log("Clicked pet: " + gameObject.name);
        
        var dataHolder = GetComponent<PetDataHolder>();
        if (dataHolder != null && uiManager != null)
        {
            PlayPetClickAudio(dataHolder.petData.petID);

            uiManager.ToggleInfoPanel(dataHolder.petData.playerPetID);
        }
        else
        {
            Debug.LogWarning("PetDataHolder or PetInfoUIManager is not assigned to " + gameObject.name);
        }
    }

    private void PlayPetClickAudio(int petID)
    {
        if (!enableClickAudio) return;

        string petSoundName = $"pet_{petID}_click";
        
        try
        {
            SoundEffectManager.Play(petSoundName, randomPitch);
            Debug.Log($"?? Played pet click sound: {petSoundName}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SoundEffectManager not available or sound '{petSoundName}' not found: {ex.Message}");
            
            PlayPetClickAudioFallback(petID);
        }
    }

    private void PlayPetClickAudioFallback(int petID)
    {
        if (petClickAudioClips == null || petClickAudioClips.Length == 0)
        {
            Debug.LogWarning("No fallback audio clips assigned for pet click sounds");
            return;
        }

        if (petID >= 0 && petID < petClickAudioClips.Length)
        {
            AudioClip clipToPlay = petClickAudioClips[petID];
            if (clipToPlay != null)
            {
                PlayAudioClipDirect(clipToPlay, $"TempPetClickAudio_Pet{petID}");
            }
            else
            {
                Debug.LogWarning($"No audio clip assigned for petID {petID} in fallback array");
                PlayDefaultPetClickSound();
            }
        }
        else
        {
            Debug.LogWarning($"PetID {petID} is out of range for petClickAudioClips array (length: {petClickAudioClips.Length})");
            PlayDefaultPetClickSound();
        }
    }

    private void PlayDefaultPetClickSound()
    {
        if (petClickAudioClips != null && petClickAudioClips.Length > 0 && petClickAudioClips[0] != null)
        {
            PlayAudioClipDirect(petClickAudioClips[0], "TempPetClickAudio_Default");
            Debug.Log("?? Played default pet click sound");
        }
        else
        {
            Debug.LogWarning("No default pet click audio available");
        }
    }

    private void PlayAudioClipDirect(AudioClip audioClip, string tempObjectName)
    {
        if (audioClip == null) return;

        GameObject tempAudioGO = new GameObject(tempObjectName);
        tempAudioGO.transform.position = transform.position;
        
        AudioSource audioSource = tempAudioGO.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = clickSoundVolume;
        audioSource.spatialBlend = 0f;
        
        if (randomPitch)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        }
        
        audioSource.Play();
        
        Destroy(tempAudioGO, audioClip.length + 0.1f);
        
        Debug.Log($"?? Played pet click sound using fallback method: {audioClip.name}");
    }

    [ContextMenu("Test Pet Click Sound")]
    public void TestPetClickSound()
    {
        var dataHolder = GetComponent<PetDataHolder>();
        if (dataHolder != null && dataHolder.petData != null)
        {
            PlayPetClickAudio(dataHolder.petData.petID);
        }
        else
        {
            Debug.LogWarning("Cannot test pet click sound: No PetDataHolder or petData found");
        }
    }

    public void SetPetClickAudioClips(AudioClip[] audioClips)
    {
        petClickAudioClips = audioClips;
        Debug.Log($"Pet click audio clips set: {audioClips?.Length ?? 0} clips assigned");
    }

    public void SetClickAudioEnabled(bool enabled)
    {
        enableClickAudio = enabled;
        Debug.Log($"Pet click audio {(enabled ? "enabled" : "disabled")}");
    }
}