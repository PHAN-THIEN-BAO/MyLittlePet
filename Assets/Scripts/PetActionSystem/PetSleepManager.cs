using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSleepManager : MonoBehaviour
{
    [Header("Sleep Settings")]
    [SerializeField] private float defaultSleepDuration = 10f;
    [SerializeField] private bool showSleepVisuals = true;
    [SerializeField] private GameObject sleepEffectPrefab;
    [SerializeField] private int expRewardPerSleep = 5;
    [SerializeField] private bool autoWakeUpOnFullEnergy = true;
    
    [Header("Energy Recovery Settings")]
    [SerializeField] private bool enableEnergyRecovery = true;
    [SerializeField] private float energyUpdateInterval = 1f;
    [SerializeField] private int energyPerInterval = 2;
    [SerializeField] private int maxEnergyValue = 100;
    [SerializeField] private bool showEnergyUpdates = true;
    
    [Header("Movement Blocking")]
    [SerializeField] private bool blockMovementDuringSleep = true;
    [SerializeField] private bool blockActionsDuringSleep = true;
    
    public static PetSleepManager Instance { get; private set; }
    
    private Dictionary<int, SleepingPetData> sleepingPets = new Dictionary<int, SleepingPetData>();
    
    public System.Action<int> OnPetStartSleep;
    public System.Action<int> OnPetWakeUp;
    public System.Action<int, int> OnPetEnergyUpdated;
    
    private struct SleepingPetData
    {
        public GameObject petObject;
        public Coroutine sleepCoroutine;
        public Coroutine energyRecoveryCoroutine;
        public GameObject sleepEffect;
        public List<MonoBehaviour> disabledComponents;
        public float sleepStartTime;
        public float sleepDuration;
        public bool expAwarded;
        public int totalEnergyGained;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PutPetToSleep(int playerPetID, float duration = 0f)
    {
        if (duration <= 0f)
            duration = defaultSleepDuration;

        GameObject petObject = FindPetById(playerPetID);
        if (petObject == null)
        {
            Debug.LogWarning($"Pet with ID {playerPetID} not found in scene!");
            return;
        }

        if (sleepingPets.ContainsKey(playerPetID))
        {
            Debug.LogWarning($"Pet {playerPetID} is already sleeping!");
            return;
        }

        AddExperienceForSleep();

        StartCoroutine(SleepPetCoroutine(playerPetID, petObject, duration));
    }

    private void AddExperienceForSleep()
    {
        PlayerLevel playerLevel = GameObject.Find("Player").GetComponent<PlayerLevel>();
        if (playerLevel != null)
        {
            playerLevel.AddExp(expRewardPerSleep);
            Debug.Log($"Added {expRewardPerSleep} experience for putting pet to sleep");
        }
        else
        {
            Debug.LogWarning("PlayerLevel component not found on Player GameObject");
        }
    }

    public void WakePetUp(int petID)
    {
        if (sleepingPets.ContainsKey(petID))
        {
            SleepingPetData sleepData = sleepingPets[petID];
            
            if (sleepData.sleepCoroutine != null)
                StopCoroutine(sleepData.sleepCoroutine);
            
            if (sleepData.energyRecoveryCoroutine != null)
                StopCoroutine(sleepData.energyRecoveryCoroutine);
            
            WakeUpPetInternal(petID);
        }
        else
        {
            Debug.LogWarning($"Pet {petID} is not currently sleeping!");
        }
    }

    public bool IsPetSleeping(int playerPetID)
    {
        return sleepingPets.ContainsKey(playerPetID);
    }

    public float GetRemainingSleepTime(int playerPetID)
    {
        if (sleepingPets.ContainsKey(playerPetID))
        {
            SleepingPetData sleepData = sleepingPets[playerPetID];
            float elapsedTime = Time.time - sleepData.sleepStartTime;
            return Mathf.Max(0f, sleepData.sleepDuration - elapsedTime);
        }
        return 0f;
    }

    public int GetTotalEnergyGained(int playerPetID)
    {
        if (sleepingPets.ContainsKey(playerPetID))
        {
            return sleepingPets[playerPetID].totalEnergyGained;
        }
        return 0;
    }

    private IEnumerator SleepPetCoroutine(int playerPetID, GameObject petObject, float duration)
    {
        Debug.Log($"?? Pet {playerPetID} is going to sleep for {duration} seconds");

        List<MonoBehaviour> disabledComponents = DisablePetComponents(petObject);
        
        GameObject sleepEffect = null;
        if (showSleepVisuals && sleepEffectPrefab != null)
        {
            sleepEffect = Instantiate(sleepEffectPrefab, petObject.transform.position + Vector3.up * 2f, Quaternion.identity);
            sleepEffect.transform.SetParent(petObject.transform);
        }

        SleepingPetData sleepData = new SleepingPetData
        {
            petObject = petObject,
            sleepCoroutine = null,
            energyRecoveryCoroutine = null,
            sleepEffect = sleepEffect,
            disabledComponents = disabledComponents,
            sleepStartTime = Time.time,
            sleepDuration = duration,
            expAwarded = true,
            totalEnergyGained = 0
        };
        
        sleepingPets[playerPetID] = sleepData;

        Animator animator = petObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", 0);
        }

        OnPetStartSleep?.Invoke(playerPetID);

        if (enableEnergyRecovery)
        {
            sleepData.energyRecoveryCoroutine = StartCoroutine(EnergyRecoveryCoroutine(playerPetID));
            sleepingPets[playerPetID] = sleepData;
        }

        yield return new WaitForSeconds(duration);

        WakeUpPetInternal(playerPetID);
    }

    private IEnumerator EnergyRecoveryCoroutine(int playerPetID)
    {
        if (showEnergyUpdates)
            Debug.Log($"? Starting energy recovery for pet {playerPetID}");

        while (sleepingPets.ContainsKey(playerPetID))
        {
            yield return new WaitForSeconds(energyUpdateInterval);

            if (!sleepingPets.ContainsKey(playerPetID))
                break;

            PlayerPet petData = APIPlayerPet.GetPlayerPetById(playerPetID);
            if (petData != null)
            {
                string[] statusValues = petData.status.Split('%');
                if (statusValues.Length >= 3)
                {
                    int.TryParse(statusValues[0], out int hunger);
                    int.TryParse(statusValues[1], out int happiness);
                    int.TryParse(statusValues[2], out int currentEnergy);

                    int newEnergy = Mathf.Min(currentEnergy + energyPerInterval, maxEnergyValue);

                    if (newEnergy != currentEnergy)
                    {
                        string newStatus = $"{hunger}%{happiness}%{newEnergy}";
                        petData.status = newStatus;

                        StartCoroutine(UpdatePetStatusInDatabase(petData));

                        var sleepData = sleepingPets[playerPetID];
                        sleepData.totalEnergyGained += (newEnergy - currentEnergy);
                        sleepingPets[playerPetID] = sleepData;

                        OnPetEnergyUpdated?.Invoke(playerPetID, newEnergy);

                        UpdatePetInfoUI(playerPetID, newStatus);

                        if (showEnergyUpdates)
                        {
                            Debug.Log($"? Pet {playerPetID} energy: {currentEnergy} ? {newEnergy} (Total gained: {sleepData.totalEnergyGained})");
                        }

                        if (newEnergy >= maxEnergyValue)
                        {
                            if (autoWakeUpOnFullEnergy)
                            {
                                if (showEnergyUpdates)
                                    Debug.Log($"? Pet {playerPetID} reached max energy! Auto-waking up...");
                                
                                WakeUpPetInternal(playerPetID);
                                yield break;
                            }
                            else
                            {
                                if (showEnergyUpdates)
                                    Debug.Log($"? Pet {playerPetID} reached max energy! Stopping energy recovery (auto wake up disabled).");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Invalid pet status format for pet {playerPetID}: {petData.status}");
                }
            }
            else
            {
                Debug.LogWarning($"Could not find pet data for pet {playerPetID}");
            }
        }

        if (showEnergyUpdates)
            Debug.Log($"? Energy recovery completed for pet {playerPetID}");
    }

    private IEnumerator UpdatePetStatusInDatabase(PlayerPet petData)
    {
        yield return APIPlayerPet.UpdatePlayerPetCoroutine(petData, success =>
        {
            if (!success)
            {
                Debug.LogError($"Failed to update pet {petData.playerPetID} status in database during sleep");
            }
        });
    }

    private void UpdatePetInfoUI(int sleepingPetID, string newStatus)
    {
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null && petInfoManager.IsPanelActive())
        {
            var (currentlyDisplayedPetId, _) = petInfoManager.GetCurrentPetAndPlayerId();
            
            if (currentlyDisplayedPetId == sleepingPetID)
            {
                try
                {
                    var field = typeof(PetInfoUIManager).GetField("currentPetDetails", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        var currentPetDetails = (PlayerPet)field.GetValue(petInfoManager);
                        if (currentPetDetails != null && currentPetDetails.playerPetID == sleepingPetID)
                        {
                            currentPetDetails.status = newStatus;
                            field.SetValue(petInfoManager, currentPetDetails);
                            Debug.Log($"?? Updated UI for sleeping pet {sleepingPetID}: {newStatus}");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Failed to update PetInfoUIManager local data: {ex.Message}");
                }

                var statusBarManager = petInfoManager.statusBarManager;
                if (statusBarManager != null)
                {
                    statusBarManager.UpdatePetStatus(newStatus);
                }
            }
            else
            {
                if (showEnergyUpdates)
                    Debug.Log($"?? Skipping UI update - displayed pet {currentlyDisplayedPetId} != sleeping pet {sleepingPetID}");
            }
        }
    }

    private void WakeUpPetInternal(int playerPetID)
    {
        if (!sleepingPets.ContainsKey(playerPetID))
            return;

        SleepingPetData sleepData = sleepingPets[playerPetID];
        
        Debug.Log($"?? Pet {playerPetID} is waking up (Total energy gained: {sleepData.totalEnergyGained})");

        if (sleepData.energyRecoveryCoroutine != null)
            StopCoroutine(sleepData.energyRecoveryCoroutine);

        EnablePetComponents(sleepData.disabledComponents);

        if (sleepData.sleepEffect != null)
            Destroy(sleepData.sleepEffect);

        sleepingPets.Remove(playerPetID);

        OnPetWakeUp?.Invoke(playerPetID);
    }

    private List<MonoBehaviour> DisablePetComponents(GameObject petObject)
    {
        List<MonoBehaviour> disabledComponents = new List<MonoBehaviour>();

        if (blockMovementDuringSleep)
        {

            NPCRandomMovement npcMovement = petObject.GetComponent<NPCRandomMovement>();
            if (npcMovement != null && npcMovement.enabled)
            {
                npcMovement.enabled = false;
                disabledComponents.Add(npcMovement);
            }

            foreach (var component in petObject.GetComponents<MonoBehaviour>())
            {
                if (component == npcMovement) continue;

                string typeName = component.GetType().Name.ToLower();
                if ((typeName.Contains("movement") || typeName.Contains("move")) && component.enabled)
                {
                    Debug.Log($"Disabling movement on: {petObject.name}, component: {component.GetType().Name}");
                    component.enabled = false;
                    disabledComponents.Add(component);
                }
            }
        }

        return disabledComponents;
    }

    private void EnablePetComponents(List<MonoBehaviour> components)
    {
        foreach (var component in components)
        {
            if (component != null)
                component.enabled = true;
        }
    }

    private GameObject FindPetById(int playerPetID)
    {
        Debug.Log($"Looking for pet with playerPetID={playerPetID}");
        PetDataHolder[] petHolders = FindObjectsOfType<PetDataHolder>();
        foreach (var holder in petHolders)
        {
            if (holder.petData != null)
                Debug.Log($"Found pet: playerPetID={holder.petData.playerPetID}, petID={holder.petData.petID}");
        }

        foreach (var holder in petHolders)
        {
            if (holder.petData != null && holder.petData.playerPetID == playerPetID)
            {
                return holder.gameObject;
            }
        }

        PetClickHandler[] clickHandlers = FindObjectsOfType<PetClickHandler>();
        foreach (var handler in clickHandlers)
        {
            PetDataHolder dataHolder = handler.GetComponent<PetDataHolder>();
            if (dataHolder != null && dataHolder.petData != null && dataHolder.petData.playerPetID == playerPetID)
            {
                return handler.gameObject;
            }
        }

        return null;
    }

    public bool ShouldBlockAction(int playerPetID, PetAction.ActionType actionType)
    {
        if (!blockActionsDuringSleep || !IsPetSleeping(playerPetID))
            return false;

        return actionType != PetAction.ActionType.Sleep;
    }

    public bool ShouldBlockEnergyDecay(int playerPetID)
    {
        return IsPetSleeping(playerPetID);
    }

    public string GetSleepStatus(int playerPetID)
    {
        if (!IsPetSleeping(playerPetID))
            return "Awake";

        var sleepData = sleepingPets[playerPetID];
        float remainingTime = GetRemainingSleepTime(playerPetID);
        return $"Sleeping - {remainingTime:F1}s remaining, {sleepData.totalEnergyGained} energy gained";
    }

    public List<int> GetSleepingPetIds()
    {
        return new List<int>(sleepingPets.Keys);
    }

    public void LogAllSleepingPets()
    {
        if (sleepingPets.Count == 0)
        {
            Debug.Log("?? No pets are currently sleeping");
            return;
        }

        Debug.Log($"?? Currently sleeping pets ({sleepingPets.Count}):");
        foreach (var kvp in sleepingPets)
        {
            int petID = kvp.Key;
            var sleepData = kvp.Value;
            float remainingTime = GetRemainingSleepTime(petID);
            Debug.Log($"  - Pet {petID}: {remainingTime:F1}s remaining, {sleepData.totalEnergyGained} energy gained");
        }
    }

    public void ConfigureEnergyRecovery(float updateInterval, int energyPerInterval, int maxEnergy, bool enabled = true)
    {
        this.energyUpdateInterval = updateInterval;
        this.energyPerInterval = energyPerInterval;
        this.maxEnergyValue = maxEnergy;
        this.enableEnergyRecovery = enabled;
        
        Debug.Log($"? Energy recovery configured: {energyPerInterval} energy every {updateInterval}s (max: {maxEnergy})");
    }

    public float GetEnergyRecoveryRate()
    {
        if (!enableEnergyRecovery || energyUpdateInterval <= 0) return 0f;
        return energyPerInterval / energyUpdateInterval;
    }

    public void SetAutoWakeUpOnFullEnergy(bool enabled)
    {
        autoWakeUpOnFullEnergy = enabled;
        Debug.Log($"?? Auto wake up on full energy: {(enabled ? "Enabled" : "Disabled")}");
    }

    public bool WakeUpPetForFeeding(int playerPetID)
    {
        if (IsPetSleeping(playerPetID))
        {
            Debug.Log($"?? Pet {playerPetID} is sleeping - waking up for feeding");
            WakePetUp(playerPetID);
            return true;
        }
        return false;
    }

    public bool WakeUpPetForCareAction(int playerPetID, PetAction.ActionType actionType)
    {
        bool requiresAwakePet = actionType == PetAction.ActionType.Feed || 
                               actionType == PetAction.ActionType.Play;

        if (requiresAwakePet && IsPetSleeping(playerPetID))
        {
            Debug.Log($"?? Pet {playerPetID} is sleeping - waking up for {actionType} action");
            WakePetUp(playerPetID);
            return true;
        }
        return false;
    }

    public bool ShouldWakeUpForAction(int playerPetID, PetAction.ActionType actionType)
    {
        if (!IsPetSleeping(playerPetID))
            return false;

        switch (actionType)
        {
            case PetAction.ActionType.Feed:
            case PetAction.ActionType.Play:
                return true;

            case PetAction.ActionType.Sleep:
                return false;

            case PetAction.ActionType.CareForAll:
                return true;

            default:
                return false;
        }
    }

    private void OnDestroy()
    {
        List<int> sleepingPetIds = new List<int>(sleepingPets.Keys);
        foreach (int playerPetID in sleepingPetIds)
        {
            WakePetUp(playerPetID);
        }
    }
}