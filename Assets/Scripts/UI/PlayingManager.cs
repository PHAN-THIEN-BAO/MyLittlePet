using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayingManager : MonoBehaviour
{
    [Header("Mini-Game Panel References")]
    public GameObject miniGamePanel;
    public Button closeButton;
    public Button startMiniGameButton;
    public TMP_Text miniGameTitle;
    public TMP_Text miniGameDescription;

    [Header("Mini-Game Settings")]
    [SerializeField] private string miniGameSceneName = "Mini_Game_1";
    [SerializeField] private int defaultHappinessReward = 20;
    [SerializeField] private int winBonusReward = 10;
    [SerializeField] private int expRewardPerPlay = 15;
    [SerializeField] private int expWinBonus = 5;

    [Header("Dependency Check Settings")]
    [Tooltip("Check pet status dependencies before playing")]
    public bool enableDependencyCheck = true;

    private int currentPlayerId;
    private PetInfoUIManager petInfoManager;

    void Start()
    {
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMiniGamePanel);
            
        if (startMiniGameButton != null)
        {
            startMiniGameButton.onClick.RemoveAllListeners();
            
            startMiniGameButton.onClick.AddListener(StartMiniGameWithManualUpdate);
        }

        if (miniGamePanel != null)
            miniGamePanel.SetActive(false);
    }

    public void ShowPlayingPanel(int playerId)
    {
        if (enableDependencyCheck && petInfoManager != null)
        {
            var blockReason = petInfoManager.CanPerformAction(PetAction.ActionType.Play);
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                string message = petInfoManager.GetBlockReasonMessage(blockReason, PetAction.ActionType.Play);
                Debug.LogWarning($"Cannot show mini-game panel: {message}");
                ShowPlayingBlockedMessage(message);
                return;
            }
        }

        currentPlayerId = playerId;
        ShowMiniGamePanel();
    }

    private void ShowMiniGamePanel()
    {
        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(true);
            
            if (miniGameTitle != null)
                miniGameTitle.text = "?? Mini Game Time!";
                
            if (miniGameDescription != null)
                miniGameDescription.text = "Play a fun mini-game to make your pet happy!\nYou'll gain happiness whether you win or lose!";
                
            Debug.Log("?? Mini-game panel opened!");
        }
    }

    public void CloseMiniGamePanel()
    {
        if (miniGamePanel != null)
            miniGamePanel.SetActive(false);
    }

    private void StartMiniGame()
    {
        Debug.Log("?? Starting mini-game...");
        
        PlayerPrefs.SetInt("CurrentPlayerId", currentPlayerId);
        PlayerPrefs.SetInt("MiniGameHappinessReward", defaultHappinessReward);
        PlayerPrefs.SetInt("MiniGameWinBonus", winBonusReward);
        
        CloseMiniGamePanel();
        
        LoadMiniGameScene();
    }

    private void StartMiniGameWithManualUpdate()
    {
        Debug.Log("?? Starting mini-game with manual status update...");
        
        if (petInfoManager != null)
        {
            petInfoManager.OnPlayButtonClickedWithHistory();
            Debug.Log("? Pet status updated immediately");
        }
        
        PlayerPrefs.SetInt("ManualStatusUpdate", 1);
        
        StartMiniGame();
    }

    private void LoadMiniGameScene()
    {
        ASyncLoading asyncLoader = FindObjectOfType<ASyncLoading>();
        if (asyncLoader != null)
        {
            asyncLoader.LoadScene(miniGameSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(miniGameSceneName);
        }
    }

    private void ShowPlayingBlockedMessage(string message)
    {
        Debug.LogWarning($"?? MINI-GAME BLOCKED: {message}");
        StartCoroutine(ShowTemporaryMessage(message));
    }

    private IEnumerator ShowTemporaryMessage(string message)
    {
        GameObject messagePanel = new GameObject("MiniGameBlockedMessage");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            messagePanel.transform.SetParent(canvas.transform, false);

            Image bg = messagePanel.AddComponent<Image>();
            bg.color = new Color(1f, 0.8f, 0.2f, 0.8f);

            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(messagePanel.transform, false);
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = message;
            text.color = Color.black;
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;

            RectTransform rect = messagePanel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
            rect.anchoredPosition = Vector2.zero;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(380, 80);
            textRect.anchoredPosition = Vector2.zero;

            yield return new WaitForSeconds(3f);

            if (messagePanel != null)
                Destroy(messagePanel);
        }
    }

    void OnEnable()
    {
        Debug.Log("?? PlayingManager OnEnable called");

        if (petInfoManager == null)
        {
            petInfoManager = FindObjectOfType<PetInfoUIManager>();
            if (petInfoManager == null)
            {
                Debug.LogWarning("?? PetInfoManager not found in OnEnable");
                return;
            }
        }

        bool manualUpdate = PlayerPrefs.GetInt("ManualStatusUpdate", 0) == 1;
        
        if (manualUpdate && petInfoManager != null)
        {
            Debug.Log("?? Manual update detected, refreshing UI...");
            StartCoroutine(DelayedRefreshUI());
        }
        
        if (PlayerPrefs.GetInt("MiniGameCompleted", 0) == 1)
        {
            bool won = PlayerPrefs.GetInt("MiniGameWon", 1) == 1;
            int baseReward = PlayerPrefs.GetInt("MiniGameHappinessReward", defaultHappinessReward);
            int winBonus = PlayerPrefs.GetInt("MiniGameWinBonus", winBonusReward);
            
            int totalReward = won ? baseReward + winBonus : baseReward;
            
            if (!manualUpdate)
            {
                OnMiniGameCompleted(won, totalReward);
            }
            else
            {
                Debug.Log("?? Skipping auto status update (manual update was used)");
                
                AddExperienceForPlaying(won);
                RecordPlayingHistory();
                
                if (petInfoManager != null)
                {
                    string message = won ? 
                        $"?? You won the mini-game!" : 
                        $"?? Good effort playing the mini-game!";
                    petInfoManager.ShowStatusMessage(message, won ? Color.green : Color.yellow);
                }
            }
            
            PlayerPrefs.DeleteKey("MiniGameCompleted");
            PlayerPrefs.DeleteKey("MiniGameWon");
            PlayerPrefs.DeleteKey("MiniGameHappinessReward");
            PlayerPrefs.DeleteKey("MiniGameWinBonus");
            PlayerPrefs.DeleteKey("CurrentPlayerId");
            PlayerPrefs.DeleteKey("ManualStatusUpdate");
        }
        else if (manualUpdate)
        {
            Debug.Log("?? Manual update detected, clearing flag");
            PlayerPrefs.DeleteKey("ManualStatusUpdate");
        }
    }

    private IEnumerator DelayedRefreshUI()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        try
        {
            RefreshPetStatusUI();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"? Error in delayed UI refresh: {ex.Message}");
        }
    }

    private void AddExperienceForPlaying(bool won)
    {
        PlayerLevel playerLevel = GameObject.Find("Player").GetComponent<PlayerLevel>();
        if (playerLevel != null)
        {
            int totalExp = expRewardPerPlay + (won ? expWinBonus : 0);
            playerLevel.AddExp(totalExp);
            Debug.Log($"Added {totalExp} experience for playing mini-game (Won: {won})");
        }
        else
        {
            Debug.LogWarning("PlayerLevel component not found on Player GameObject");
        }
    }

    public void OnMiniGameCompleted(bool won, int happinessAmount)
    {
        if (petInfoManager != null)
        {
            petInfoManager.UpdatePetStatus(1, happinessAmount);
            
            string message = won ? 
                $"?? You won the mini-game! Pet happiness +{happinessAmount}" : 
                $"?? Good effort! Pet happiness +{happinessAmount}";
                
            petInfoManager.ShowStatusMessage(message, won ? Color.green : Color.yellow);
            
            RecordPlayingHistory();
            
            Debug.Log($"?? Mini-game result: Won={won}, Happiness=+{happinessAmount}");
        }
    }
    
    private void RecordPlayingHistory()
    {
        if (CareHistoryRecorder.Instance != null)
        {
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser != null)
            {
                var pets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
                if (pets != null && pets.Count > 0)
                {
                    int playerPetId = pets[0].playerPetID;
                    CareHistoryRecorder.Instance.RecordPlayingHistory(playerPetId, currentUser.id);
                }
            }
        }
    }

    private void RefreshPetStatusUI()
    {
        try
        {
            if (petInfoManager == null)
            {
                Debug.LogWarning("?? PetInfoManager is null, cannot refresh UI");
                return;
            }

            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser == null)
            {
                Debug.LogWarning("?? Current user is null, cannot refresh UI");
                return;
            }

            var pets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
            if (pets == null || pets.Count == 0)
            {
                Debug.LogWarning("?? No pets found for current user, cannot refresh UI");
                return;
            }

            PlayerPet currentPet = pets[0];
            if (currentPet == null)
            {
                Debug.LogWarning("?? Current pet is null, cannot refresh UI");
                return;
            }
            
            try
            {
                if (petInfoManager.IsPanelActive())
                {
                    petInfoManager.ToggleInfoPanel(currentPet.playerPetID);
                    Debug.Log($"?? Refreshed pet info panel for pet {currentPet.playerPetID}");
                }
            }
            catch (System.Exception panelEx)
            {
                Debug.LogError($"? Error refreshing pet info panel: {panelEx.Message}");
            }
            
            try
            {
                if (petInfoManager.statusBarManager != null)
                {
                    petInfoManager.statusBarManager.UpdatePetStatus(currentPet.status);
                    Debug.Log($"?? Refreshed status bars: {currentPet.status}");
                }
                else
                {
                    Debug.LogWarning("?? StatusBarManager is null, cannot update status bars");
                }
            }
            catch (System.Exception statusEx)
            {
                Debug.LogError($"? Error updating status bars: {statusEx.Message}");
            }
            
            Debug.Log($"? Pet status UI refreshed successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"? Failed to refresh pet status UI: {ex.Message}");
            Debug.LogError($"? Stack trace: {ex.StackTrace}");
        }
    }

    public void ClosePlayingPanel()
    {
        CloseMiniGamePanel();
    }
}