using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayableDirectorManager : MonoBehaviour
{
    [Header("Playable Director Settings")]
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private string cutsceneId;
    [SerializeField] private bool autoPlayOnStart = true;
    [SerializeField] private bool disableAfterPlayed = true;
    [SerializeField] private bool onlyDisablePlayableDirector = true;

    private static Dictionary<string, HashSet<string>> playedCutscenes = new Dictionary<string, HashSet<string>>();
    private static bool isDataLoaded = false;

    private string currentPlayingUserId = null;
    private bool isPlaying = false;

    private void Awake()
    {
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        if (string.IsNullOrEmpty(cutsceneId))
        {
            cutsceneId = gameObject.scene.name + "_" + gameObject.name + "_" + transform.GetSiblingIndex();
        }

        if (!isDataLoaded)
        {
            LoadPlayedCutscenes();
            isDataLoaded = true;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (autoPlayOnStart)
        {
            Invoke(nameof(CheckAndPlayCutscene), 0.2f);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnCutsceneComplete;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SavePlayedCutscenes();

        if (this != null && gameObject != null && autoPlayOnStart)
        {
            Invoke(nameof(CheckAndPlayCutscene), 0.3f);
        }
    }

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

            if (disableAfterPlayed)
            {
                if (onlyDisablePlayableDirector)
                {
                    if (playableDirector != null)
                    {
                        playableDirector.enabled = false;
                        Debug.Log($"Disabled PlayableDirector component for {cutsceneId}");
                    }
                }
                else
                {
                    gameObject.SetActive(false);
                    Debug.Log($"Disabled GameObject for {cutsceneId}");
                }
            }
        }
    }

    private void PlayCutscene(string userId)
    {
        if (playableDirector != null && playableDirector.playableAsset != null)
        {
            Debug.Log($"Playing cutscene {cutsceneId} for user {userId}");

            currentPlayingUserId = userId;
            isPlaying = true;

            playableDirector.stopped += OnCutsceneComplete;

            playableDirector.Play();
        }
        else
        {
            Debug.LogError("PlayableDirector or PlayableAsset is not assigned!");
        }
    }

    private void OnCutsceneComplete(PlayableDirector director)
    {
        Debug.Log($"Cutscene {cutsceneId} completed for user {currentPlayingUserId}");

        playableDirector.stopped -= OnCutsceneComplete;

        if (!string.IsNullOrEmpty(currentPlayingUserId) && isPlaying)
        {
            MarkCutsceneAsPlayed(currentPlayingUserId, cutsceneId);
            Debug.Log($"Marked cutscene {cutsceneId} as completed for user {currentPlayingUserId}");
        }

        currentPlayingUserId = null;
        isPlaying = false;

        SavePlayedCutscenes();

    }

    public static bool HasCutscenePlayed(string userId, string cutsceneId)
    {
        if (!isDataLoaded)
        {
            LoadPlayedCutscenes();
            isDataLoaded = true;
        }

        return playedCutscenes.ContainsKey(userId) &&
               playedCutscenes[userId].Contains(cutsceneId);
    }

    public static void MarkCutsceneAsPlayed(string userId, string cutsceneId)
    {
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

    public bool IsPlaying()
    {
        return isPlaying && playableDirector != null && playableDirector.state == PlayState.Playing;
    }

    public string GetCurrentPlayingUserId()
    {
        return currentPlayingUserId;
    }

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

    public static void ResetUserCutscenes(string userId)
    {
        if (playedCutscenes.ContainsKey(userId))
        {
            playedCutscenes[userId].Clear();
            SavePlayedCutscenes();
            Debug.Log($"Reset all cutscenes for user {userId}");
        }
    }

    public static void ResetAllCutscenes()
    {
        playedCutscenes.Clear();
        PlayerPrefs.DeleteKey("PlayedCutscenes");
        PlayerPrefs.Save();
        isDataLoaded = false;
        Debug.Log("Reset all cutscene data");
    }

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

    public static void ForceSave()
    {
        SavePlayedCutscenes();
    }
}

[System.Serializable]
public class PlayedCutscenesData
{
    public List<UserCutsceneData> userCutscenes;
}

[System.Serializable]
public class UserCutsceneData
{
    public string userId;
    public List<string> playedCutscenes;
}