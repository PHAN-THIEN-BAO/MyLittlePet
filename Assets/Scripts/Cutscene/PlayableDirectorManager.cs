using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages Playable Directors to ensure they only play once per user ID
/// </summary>
public class PlayableDirectorManager : MonoBehaviour
{
    [Header("Playable Director Settings")]
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private string cutsceneId; // Unique identifier for this cutscene
    [SerializeField] private bool autoPlayOnStart = true; // Auto check and play on scene load
    [SerializeField] private bool disableAfterPlayed = true; // Có disable GameObject sau khi đã phát không
    [SerializeField] private bool onlyDisablePlayableDirector = true; // Chỉ disable PlayableDirector thay vì cả GameObject

    // Static set to track played cutscenes per user - persistent across scenes
    private static Dictionary<string, HashSet<string>> playedCutscenes = new Dictionary<string, HashSet<string>>();
    private static bool isDataLoaded = false;

    // Track current playing cutscene
    private string currentPlayingUserId = null;
    private bool isPlaying = false;

    private void Awake()
    {
        // Get PlayableDirector if not assigned
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        // Generate unique cutscene ID if not set
        if (string.IsNullOrEmpty(cutsceneId))
        {
            cutsceneId = gameObject.scene.name + "_" + gameObject.name + "_" + transform.GetSiblingIndex();
        }

        // Load played cutscenes data once
        if (!isDataLoaded)
        {
            LoadPlayedCutscenes();
            isDataLoaded = true;
        }

        // Subscribe to scene loaded event to handle scene transitions
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (autoPlayOnStart)
        {
            // Delay để đảm bảo PlayerSpawnManager đã chạy xong
            Invoke(nameof(CheckAndPlayCutscene), 0.2f);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnCutsceneComplete;
        }
    }

    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Save cutscene data when scene changes
        SavePlayedCutscenes();

        // Re-check cutscene status for the new scene if this GameObject still exists
        if (this != null && gameObject != null && autoPlayOnStart)
        {
            // Delay check to ensure scene is fully loaded and PlayerSpawnManager has run
            Invoke(nameof(CheckAndPlayCutscene), 0.3f);
        }
    }

    /// <summary>
    /// Check if cutscene should play and play it if conditions are met
    /// </summary>
    public void CheckAndPlayCutscene()
    {
        User currentUser = PlayerInfomation.LoadPlayerInfo();

        if (currentUser == null)
        {
            Debug.LogWarning("No user information found. Cannot determine if cutscene should play.");
            return;
        }

        string userId = currentUser.id.ToString();

        if (!HasCutscenePlayed(userId, cutsceneId))
        {
            PlayCutscene(userId);
        }
        else
        {
            Debug.Log($"Cutscene {cutsceneId} has already been played for user {userId}. Skipping.");

            // Thay vì disable toàn bộ GameObject, chỉ disable những gì cần thiết
            if (disableAfterPlayed)
            {
                if (onlyDisablePlayableDirector)
                {
                    // Chỉ disable PlayableDirector component
                    if (playableDirector != null)
                    {
                        playableDirector.enabled = false;
                        Debug.Log($"Disabled PlayableDirector component for {cutsceneId}");
                    }
                }
                else
                {
                    // Disable toàn bộ GameObject (có thể gây vấn đề với player)
                    gameObject.SetActive(false);
                    Debug.Log($"Disabled GameObject for {cutsceneId}");
                }
            }
        }
    }

    /// <summary>
    /// Play the cutscene and mark it as played ONLY when it completes
    /// </summary>
    private void PlayCutscene(string userId)
    {
        if (playableDirector != null && playableDirector.playableAsset != null)
        {
            Debug.Log($"Playing cutscene {cutsceneId} for user {userId}");

            // Store the current user ID and mark as playing
            currentPlayingUserId = userId;
            isPlaying = true;

            // Subscribe to completion event BEFORE playing
            playableDirector.stopped += OnCutsceneComplete;

            // Play the timeline
            playableDirector.Play();
        }
        else
        {
            Debug.LogError("PlayableDirector or PlayableAsset is not assigned!");
        }
    }

    /// <summary>
    /// Called when cutscene completes
    /// </summary>
    private void OnCutsceneComplete(PlayableDirector director)
    {
        Debug.Log($"Cutscene {cutsceneId} completed for user {currentPlayingUserId}");

        // Unsubscribe from event
        playableDirector.stopped -= OnCutsceneComplete;

        // Mark as played ONLY after completion
        if (!string.IsNullOrEmpty(currentPlayingUserId) && isPlaying)
        {
            MarkCutsceneAsPlayed(currentPlayingUserId, cutsceneId);
            Debug.Log($"Marked cutscene {cutsceneId} as completed for user {currentPlayingUserId}");
        }

        // Reset playing state
        currentPlayingUserId = null;
        isPlaying = false;

        // Save immediately after completion
        SavePlayedCutscenes();

        // Optional: Additional logic after cutscene completion
        // For example: Enable UI, trigger next event, etc.
    }

    /// <summary>
    /// Check if a cutscene has been played for a specific user
    /// </summary>
    public static bool HasCutscenePlayed(string userId, string cutsceneId)
    {
        // Ensure data is loaded
        if (!isDataLoaded)
        {
            LoadPlayedCutscenes();
            isDataLoaded = true;
        }

        return playedCutscenes.ContainsKey(userId) &&
               playedCutscenes[userId].Contains(cutsceneId);
    }

    /// <summary>
    /// Mark a cutscene as played for a specific user
    /// </summary>
    public static void MarkCutsceneAsPlayed(string userId, string cutsceneId)
    {
        // Ensure data is loaded
        if (!isDataLoaded)
        {
            LoadPlayedCutscenes();
            isDataLoaded = true;
        }

        if (!playedCutscenes.ContainsKey(userId))
        {
            playedCutscenes[userId] = new HashSet<string>();
        }

        playedCutscenes[userId].Add(cutsceneId);

        Debug.Log($"Marked cutscene {cutsceneId} as played for user {userId}");
    }

    /// <summary>
    /// Check if cutscene is currently playing
    /// </summary>
    public bool IsPlaying()
    {
        return isPlaying && playableDirector != null && playableDirector.state == PlayState.Playing;
    }

    /// <summary>
    /// Get current playing user ID
    /// </summary>
    public string GetCurrentPlayingUserId()
    {
        return currentPlayingUserId;
    }

    /// <summary>
    /// Stop current cutscene (if playing) and mark as played
    /// </summary>
    public void StopAndMarkAsPlayed()
    {
        if (isPlaying && playableDirector != null)
        {
            playableDirector.Stop();

            if (!string.IsNullOrEmpty(currentPlayingUserId))
            {
                MarkCutsceneAsPlayed(currentPlayingUserId, cutsceneId);
                SavePlayedCutscenes();
                Debug.Log($"Stopped and marked cutscene {cutsceneId} as played for user {currentPlayingUserId}");
            }

            currentPlayingUserId = null;
            isPlaying = false;
        }
    }

    /// <summary>
    /// Save played cutscenes to PlayerPrefs for persistence
    /// </summary>
    private static void SavePlayedCutscenes()
    {
        try
        {
            PlayedCutscenesData data = new PlayedCutscenesData();
            data.userCutscenes = new List<UserCutsceneData>();

            foreach (var kvp in playedCutscenes)
            {
                UserCutsceneData userData = new UserCutsceneData();
                userData.userId = kvp.Key;
                userData.playedCutscenes = new List<string>(kvp.Value);
                data.userCutscenes.Add(userData);
            }

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("PlayedCutscenes", json);
            PlayerPrefs.Save();

            Debug.Log("Saved played cutscenes data to PlayerPrefs");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save played cutscenes: " + ex.Message);
        }
    }

    /// <summary>
    /// Load played cutscenes from PlayerPrefs
    /// </summary>
    private static void LoadPlayedCutscenes()
    {
        try
        {
            playedCutscenes.Clear();

            if (PlayerPrefs.HasKey("PlayedCutscenes"))
            {
                string json = PlayerPrefs.GetString("PlayedCutscenes");
                if (!string.IsNullOrEmpty(json))
                {
                    PlayedCutscenesData data = JsonUtility.FromJson<PlayedCutscenesData>(json);

                    if (data != null && data.userCutscenes != null)
                    {
                        foreach (var userData in data.userCutscenes)
                        {
                            if (userData != null && !string.IsNullOrEmpty(userData.userId) && userData.playedCutscenes != null)
                            {
                                playedCutscenes[userData.userId] = new HashSet<string>(userData.playedCutscenes);
                            }
                        }
                    }
                }

                Debug.Log("Loaded played cutscenes data from PlayerPrefs");
            }
            else
            {
                Debug.Log("No saved cutscene data found");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load played cutscenes: " + ex.Message);
            playedCutscenes.Clear();
        }
    }

    /// <summary>
    /// Reset all cutscene data for a specific user (useful for testing)
    /// </summary>
    public static void ResetUserCutscenes(string userId)
    {
        if (playedCutscenes.ContainsKey(userId))
        {
            playedCutscenes[userId].Clear();
            SavePlayedCutscenes();
            Debug.Log($"Reset all cutscenes for user {userId}");
        }
    }

    /// <summary>
    /// Reset all cutscene data (useful for testing)
    /// </summary>
    public static void ResetAllCutscenes()
    {
        playedCutscenes.Clear();
        PlayerPrefs.DeleteKey("PlayedCutscenes");
        PlayerPrefs.Save();
        isDataLoaded = false;
        Debug.Log("Reset all cutscene data");
    }

    /// <summary>
    /// Force play cutscene regardless of previous state
    /// </summary>
    public void ForcePlayCutscene()
    {
        if (playableDirector != null && playableDirector.playableAsset != null)
        {
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser != null)
            {
                string userId = currentUser.id.ToString();
                Debug.Log($"Force playing cutscene {cutsceneId}");

                currentPlayingUserId = userId;
                isPlaying = true;
                playableDirector.stopped += OnCutsceneComplete;
                playableDirector.Play();
            }
        }
    }

    /// <summary>
    /// Manually save cutscene state (can be called before scene transitions)
    /// </summary>
    public static void ForceSave()
    {
        SavePlayedCutscenes();
    }
}

/// <summary>
/// Data structure for serializing played cutscenes
/// </summary>
[System.Serializable]
public class PlayedCutscenesData
{
    public List<UserCutsceneData> userCutscenes;
}

/// <summary>
/// Data structure for user-specific cutscene data
/// </summary>
[System.Serializable]
public class UserCutsceneData
{
    public string userId;
    public List<string> playedCutscenes;
}