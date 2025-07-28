using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class ChoosePet : MonoBehaviour
{
    [SerializeField] public TMP_Text petID;
    [SerializeField] public TMP_Text petDefaultName;
    [SerializeField] public TMP_InputField petCustomName;
    [SerializeField] public TMP_Text petType;
    [SerializeField] public TMP_Text description;
    [SerializeField] public GameObject successPanel;
    [SerializeField] public GameObject failPanel;
    [SerializeField] public GameObject cutsceneGameObject; // GameObject chứa PlayableDirector
    public PetController petController; // Kéo PetController trong scene vào Inspector

    private PlayableDirector playableDirector;
    private PlayableDirectorManager directorManager;

    private void Awake()
    {
        // Lấy PlayableDirector từ cutsceneGameObject nếu được gán
        if (cutsceneGameObject != null)
        {
            playableDirector = cutsceneGameObject.GetComponent<PlayableDirector>();
            directorManager = cutsceneGameObject.GetComponent<PlayableDirectorManager>();
        }
    }

    private void Start()
    {
        // Ngăn timeline tự động phát khi scene bắt đầu
        PreventAutoPlayTimeline();
    }

    /// <summary>
    /// Ngăn timeline tự động phát khi scene bắt đầu
    /// </summary>
    private void PreventAutoPlayTimeline()
    {
        if (cutsceneGameObject != null)
        {
            // Tạm thời vô hiệu hóa GameObject chứa timeline
            cutsceneGameObject.SetActive(false);
            Debug.Log("Timeline GameObject disabled to prevent auto-play on scene start.");
        }

        // Nếu có PlayableDirector, đảm bảo nó không phát
        if (playableDirector != null)
        {
            playableDirector.Stop();
            Debug.Log("PlayableDirector stopped to prevent auto-play.");
        }
    }

    public void ChooseAPet()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        PlayerPet playerPet = new PlayerPet();
        playerPet.playerID = user.id;
        Debug.Log("Player ID: " + playerPet.playerID);
        playerPet.petID = int.Parse(petID.text);
        Debug.Log("Pet ID: " + playerPet.petID);
        playerPet.petCustomName = petCustomName.text;
        Debug.Log("Pet Custom Name: " + playerPet.petCustomName);
        if (APIPlayerPet.AddPlayerPet(playerPet))
        {
            Debug.Log("Pet added successfully!");
            successPanel.SetActive(true);

            // Spawn pet ngay lập tức
            if (petController != null)
            {
                petController.SpawnPet(playerPet);
            }
        }
        else
        {
            failPanel.SetActive(true);
            Debug.LogError("Failed to add pet.");
        }
    }

    /// <summary>
    /// Function để đóng success panel và bắt đầu cutscene
    /// Gọi function này từ Button onClick event trên success panel
    /// </summary>
    public void CloseSuccessPanelAndStartCutscene()
    {
        // Đóng success panel
        if (successPanel != null)
        {
            successPanel.SetActive(false);
            Debug.Log("Success panel closed.");
        }

        // Bắt đầu cutscene
        StartCutscene();
    }

    /// <summary>
    /// Function để bắt đầu GameObject chứa PlayableDirector
    /// </summary>
    public void StartCutscene()
    {
        if (cutsceneGameObject != null)
        {
            // Kích hoạt GameObject nếu nó đang bị tắt
            if (!cutsceneGameObject.activeInHierarchy)
            {
                cutsceneGameObject.SetActive(true);
                Debug.Log("Cutscene GameObject activated.");
            }

            // Nếu có PlayableDirectorManager, gọi ForcePlayCutscene
            if (directorManager != null)
            {
                directorManager.ForcePlayCutscene();
                Debug.Log("Cutscene started via PlayableDirectorManager.ForcePlayCutscene().");
            }
            // Nếu chỉ có PlayableDirector, phát timeline trực tiếp
            else if (playableDirector != null && playableDirector.playableAsset != null)
            {
                playableDirector.Play();
                Debug.Log("Cutscene started via PlayableDirector.Play().");
            }
            else
            {
                Debug.LogWarning("PlayableDirector hoặc PlayableAsset không được tìm thấy trong cutsceneGameObject!");
            }
        }
        else
        {
            Debug.LogError("Cutscene GameObject chưa được gán trong Inspector!");
        }
    }

    /// <summary>
    /// Function riêng để chỉ đóng success panel (không bắt đầu cutscene)
    /// </summary>
    public void CloseSuccessPanel()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(false);
            Debug.Log("Success panel closed.");
        }
    }
}
