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
    [SerializeField] public GameObject cutsceneGameObject;
    public PetController petController;
    private PlayableDirector playableDirector;
    private PlayableDirectorManager directorManager;
    private void Awake()
    {
        if (cutsceneGameObject != null)
        {
            playableDirector = cutsceneGameObject.GetComponent<PlayableDirector>();
            directorManager = cutsceneGameObject.GetComponent<PlayableDirectorManager>();
        }
    }
    private void Start()
    {
        PreventAutoPlayTimeline();
    }
    private void PreventAutoPlayTimeline()
    {
        if (cutsceneGameObject != null)
        {
            cutsceneGameObject.SetActive(false);
            Debug.Log("Timeline GameObject disabled to prevent auto-play on scene start.");
        }
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
    public void CloseSuccessPanelAndStartCutscene()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(false);
            Debug.Log("Success panel closed.");
        }
        StartCutscene();
    }
    public void StartCutscene()
    {
        if (cutsceneGameObject != null)
        {
            if (!cutsceneGameObject.activeInHierarchy)
            {
                cutsceneGameObject.SetActive(true);
                Debug.Log("Cutscene GameObject activated.");
            }
            if (directorManager != null)
            {
                directorManager.ForcePlayCutscene();
                Debug.Log("Cutscene started via PlayableDirectorManager.ForcePlayCutscene().");
            }
            else if (playableDirector != null && playableDirector.playableAsset != null)
            {
                playableDirector.Play();
                Debug.Log("Cutscene started via PlayableDirector.Play().");
            }
            else
            {
                Debug.LogWarning("PlayableDirector ho?c PlayableAsset không du?c tìm th?y trong cutsceneGameObject!");
            }
        }
        else
        {
            Debug.LogError("Cutscene GameObject chua du?c gán trong Inspector!");
        }
    }
    public void CloseSuccessPanel()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(false);
            Debug.Log("Success panel closed.");
        }
    }
}