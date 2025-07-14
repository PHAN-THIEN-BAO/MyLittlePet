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
    [SerializeField] private int expRewardPerPlay = 15; // Experience gained per mini-game
    [SerializeField] private int expWinBonus = 5; // Extra experience for winning

    [Header("Dependency Check Settings")]
    [Tooltip("Check pet status dependencies before playing")]
    public bool enableDependencyCheck = true;

    private int currentPlayerId;
    private PetInfoUIManager petInfoManager;

    void Start()
    {
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        
        // Setup buttons
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMiniGamePanel);
            
        if (startMiniGameButton != null)
            startMiniGameButton.onClick.AddListener(StartMiniGame);

        // Hide panel by default
        if (miniGamePanel != null)
            miniGamePanel.SetActive(false);
    }

    public void ShowPlayingPanel(int playerId)
    {
        // CHECK DEPENDENCY BEFORE SHOWING PANEL
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

        // HIỂN THỊ MINI-GAME PANEL
        currentPlayerId = playerId;
        ShowMiniGamePanel();
    }

    private void ShowMiniGamePanel()
    {
        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(true);
            
            // Update UI text
            if (miniGameTitle != null)
                miniGameTitle.text = "🎮 Mini Game Time!";
                
            if (miniGameDescription != null)
                miniGameDescription.text = "Play a fun mini-game to make your pet happy!\nYou'll gain happiness whether you win or lose!";
                
            Debug.Log("🎮 Mini-game panel opened!");
        }
    }

    public void CloseMiniGamePanel()
    {
        if (miniGamePanel != null)
            miniGamePanel.SetActive(false);
    }

    private void StartMiniGame()
    {
        Debug.Log("🎮 Starting mini-game...");
        
        // Lưu thông tin để sử dụng sau mini-game
        PlayerPrefs.SetInt("CurrentPlayerId", currentPlayerId);
        PlayerPrefs.SetInt("MiniGameHappinessReward", defaultHappinessReward);
        PlayerPrefs.SetInt("MiniGameWinBonus", winBonusReward);
        
        // Đóng panel
        CloseMiniGamePanel();
        
        // Load mini-game scene
        LoadMiniGameScene();
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
        Debug.LogWarning($"🚫 MINI-GAME BLOCKED: {message}");
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
        // Kiểm tra kết quả mini-game khi trở về scene chính
        if (PlayerPrefs.GetInt("MiniGameCompleted", 0) == 1)
        {
            bool won = PlayerPrefs.GetInt("MiniGameWon", 1) == 1;
            int baseReward = PlayerPrefs.GetInt("MiniGameHappinessReward", defaultHappinessReward);
            int winBonus = PlayerPrefs.GetInt("MiniGameWinBonus", winBonusReward);
            
            int totalReward = won ? baseReward + winBonus : baseReward;
            
            // Tăng happiness cho pet
            OnMiniGameCompleted(won, totalReward);
            
            // ADD EXPERIENCE FOR PLAYING MINI-GAME
            AddExperienceForPlaying(won);
            
            // Clear PlayerPrefs
            PlayerPrefs.DeleteKey("MiniGameCompleted");
            PlayerPrefs.DeleteKey("MiniGameWon");
            PlayerPrefs.DeleteKey("MiniGameHappinessReward");
            PlayerPrefs.DeleteKey("MiniGameWinBonus");
            PlayerPrefs.DeleteKey("CurrentPlayerId");
        }
    }

    /// <summary>
    /// Adds experience to the player when playing a mini-game
    /// </summary>
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
            petInfoManager.UpdatePetStatus(1, happinessAmount); // statusIndex 1 = happiness
            
            string message = won ? 
                $"🎉 You won the mini-game! Pet happiness +{happinessAmount}" : 
                $"😊 Good effort! Pet happiness +{happinessAmount}";
                
            petInfoManager.ShowStatusMessage(message, won ? Color.green : Color.yellow);
            
            Debug.Log($"🎮 Mini-game result: Won={won}, Happiness=+{happinessAmount}");
        }
    }

    public void ClosePlayingPanel()
    {
        CloseMiniGamePanel();
    }
}
