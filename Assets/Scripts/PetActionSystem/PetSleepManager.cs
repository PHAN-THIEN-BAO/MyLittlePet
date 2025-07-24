using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PetSleepManager : MonoBehaviour
{
    [Header("Sleep Settings")]
    [SerializeField] private float defaultSleepDuration = 10f;
    [SerializeField] private bool showSleepVisuals = true;
    [SerializeField] private GameObject sleepEffectPrefab;
    [SerializeField] private int expRewardPerSleep = 5; // Experience gained per sleep action
    [SerializeField] private bool autoWakeUpOnFullEnergy = true; // ← NEW: Auto wake up option
    
    [Header("Energy Recovery Settings")]
    [SerializeField] private bool enableEnergyRecovery = true;
    [SerializeField] private float energyUpdateInterval = 1f; // Update energy every X seconds
    [SerializeField] private int energyPerInterval = 2; // Energy gained per interval
    [SerializeField] private int maxEnergyValue = 100; // Maximum energy value
    [SerializeField] private bool showEnergyUpdates = true; // Show debug messages for energy updates
    
    [Header("Movement Blocking")]
    [SerializeField] private bool blockMovementDuringSleep = true;
    [SerializeField] private bool blockActionsDuringSleep = true;
    public static PetSleepManager Instance { get; private set; }
    private Dictionary<int, SleepingPetData> sleepingPets = new Dictionary<int, SleepingPetData>();
    public System.Action<int> OnPetStartSleep;
    public System.Action<int> OnPetWakeUp;
    public System.Action<int, int> OnPetEnergyUpdated; // New event for energy updates (petID, newEnergyValue)
    
    private struct SleepingPetData
    {
        public GameObject petObject;
        public Coroutine sleepCoroutine;
        public Coroutine energyRecoveryCoroutine; // New: track energy recovery coroutine
        public GameObject sleepEffect;
        public List<MonoBehaviour> disabledComponents;
        public float sleepStartTime;
        public float sleepDuration;
        public bool expAwarded; // Track if experience has been awarded for this sleep
        public int totalEnergyGained; // Track total energy gained during sleep
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
            
            // Stop energy recovery coroutine
            if (sleepData.energyRecoveryCoroutine != null)
                StopCoroutine(sleepData.energyRecoveryCoroutine);
            
            // Wake up pet
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

    /// <summary>
    /// Get total energy gained during current sleep session
    /// </summary>
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
            sleepCoroutine = null, // Will be set after this method
            energyRecoveryCoroutine = null, // Will be set when starting energy recovery
            sleepEffect = sleepEffect,
            disabledComponents = disabledComponents,
            sleepStartTime = Time.time,
            sleepDuration = duration,
            expAwarded = true, // Experience already awarded when sleep started
            totalEnergyGained = 0 // Initialize energy counter
        };
        sleepingPets[playerPetID] = sleepData;
        Animator animator = petObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", 0);
        }
        OnPetStartSleep?.Invoke(playerPetID);

        // Start energy recovery coroutine if enabled
        if (enableEnergyRecovery)
        {
            sleepData.energyRecoveryCoroutine = StartCoroutine(EnergyRecoveryCoroutine(playerPetID));
            sleepingPets[playerPetID] = sleepData; // Update the stored data
        }

        // Wait for sleep duration
        yield return new WaitForSeconds(duration);
        WakeUpPetInternal(playerPetID);
    }

    /// <summary>
    /// Coroutine that gradually increases pet's energy during sleep
    /// </summary>
    private IEnumerator EnergyRecoveryCoroutine(int playerPetID)
    {
        if (showEnergyUpdates)
            Debug.Log($"⚡ Starting energy recovery for pet {playerPetID}");

        while (sleepingPets.ContainsKey(playerPetID))
        {
            yield return new WaitForSeconds(energyUpdateInterval);

            // Check if pet is still sleeping
            if (!sleepingPets.ContainsKey(playerPetID))
                break;

            // Get current pet data from API
            PlayerPet petData = APIPlayerPet.GetPlayerPetById(playerPetID);
            if (petData != null)
            {
                // Parse current status
                string[] statusValues = petData.status.Split('%');
                if (statusValues.Length >= 3)
                {
                    int.TryParse(statusValues[0], out int hunger);
                    int.TryParse(statusValues[1], out int happiness);
                    int.TryParse(statusValues[2], out int currentEnergy);

                    // Calculate new energy value
                    int newEnergy = Mathf.Min(currentEnergy + energyPerInterval, maxEnergyValue);

                    // Only update if energy actually changed
                    if (newEnergy != currentEnergy)
                    {
                        // Update pet status
                        string newStatus = $"{hunger}%{happiness}%{newEnergy}";
                        petData.status = newStatus;

                        // Update in database
                        StartCoroutine(UpdatePetStatusInDatabase(petData));

                        // Update total energy gained
                        var sleepData = sleepingPets[playerPetID];
                        sleepData.totalEnergyGained += (newEnergy - currentEnergy);
                        sleepingPets[playerPetID] = sleepData;

                        // Fire energy update event
                        OnPetEnergyUpdated?.Invoke(playerPetID, newEnergy);

                        // ========== FIX: TRUYỀN playerPetID VÀO UpdatePetInfoUI ==========
                        UpdatePetInfoUI(playerPetID, newStatus);

                        if (showEnergyUpdates)
                        {
                            Debug.Log($"⚡ Pet {playerPetID} energy: {currentEnergy} → {newEnergy} (Total gained: {sleepData.totalEnergyGained})");
                        }

                        // ========== AUTO WAKE UP WHEN ENERGY IS FULL ==========
                        if (newEnergy >= maxEnergyValue)
                        {
                            if (autoWakeUpOnFullEnergy)
                            {
                                if (showEnergyUpdates)
                                    Debug.Log($"⚡ Pet {playerPetID} reached max energy! Auto-waking up...");
                                
                                // Wake up the pet immediately
                                WakeUpPetInternal(playerPetID);
                                yield break; // Exit coroutine completely
                            }
                            else
                            {
                                if (showEnergyUpdates)
                                    Debug.Log($"⚡ Pet {playerPetID} reached max energy! Stopping energy recovery (auto wake up disabled).");
                                break; // Only stop recovery, pet continues sleeping
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
            Debug.Log($"⚡ Energy recovery completed for pet {playerPetID}");
    }

    /// <summary>
    /// Update pet status in database
    /// </summary>
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

    /// <summary>
    /// Update PetInfoUIManager if it's available and showing the SPECIFIC pet
    /// </summary>
    private void UpdatePetInfoUI(int sleepingPetID, string newStatus)
    {
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null && petInfoManager.IsPanelActive())
        {
            // ========== CHỈ UPDATE NẾU UI ĐANG HIỂN THỊ PET ĐANG NGỦ ==========
            var (currentlyDisplayedPetId, _) = petInfoManager.GetCurrentPetAndPlayerId();
            
            if (currentlyDisplayedPetId == sleepingPetID)
            {
                try
                {
                    // Update local data safely
                    var field = typeof(PetInfoUIManager).GetField("currentPetDetails", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        var currentPetDetails = (PlayerPet)field.GetValue(petInfoManager);
                        if (currentPetDetails != null && currentPetDetails.playerPetID == sleepingPetID)
                        {
                            currentPetDetails.status = newStatus;
                            field.SetValue(petInfoManager, currentPetDetails);
                            Debug.Log($"🔄 Updated UI for sleeping pet {sleepingPetID}: {newStatus}");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Failed to update PetInfoUIManager local data: {ex.Message}");
                }

                // Update status bar UI
                var statusBarManager = petInfoManager.statusBarManager;
                if (statusBarManager != null)
                {
                    statusBarManager.UpdatePetStatus(newStatus);
                }
            }
            else
            {
                if (showEnergyUpdates)
                    Debug.Log($"🔄 Skipping UI update - displayed pet {currentlyDisplayedPetId} != sleeping pet {sleepingPetID}");
            }
        }
    }

    private void WakeUpPetInternal(int playerPetID)
    {
        if (!sleepingPets.ContainsKey(playerPetID))
            return;
        SleepingPetData sleepData = sleepingPets[playerPetID];
        
        Debug.Log($"😴 Pet {playerPetID} is waking up (Total energy gained: {sleepData.totalEnergyGained})");

        // Stop energy recovery coroutine if still running
        if (sleepData.energyRecoveryCoroutine != null)
            StopCoroutine(sleepData.energyRecoveryCoroutine);

        // Re-enable components
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

    /// <summary>
    /// Check if energy decay should be blocked for sleeping pet
    /// </summary>
    public bool ShouldBlockEnergyDecay(int playerPetID)
    {
        return IsPetSleeping(playerPetID);
    }

    /// <summary>
    /// Get sleep status for debugging
    /// </summary>
    public string GetSleepStatus(int playerPetID)
    {
        if (!IsPetSleeping(playerPetID))
            return "Awake";

        var sleepData = sleepingPets[playerPetID];
        float remainingTime = GetRemainingSleepTime(playerPetID);
        return $"Sleeping - {remainingTime:F1}s remaining, {sleepData.totalEnergyGained} energy gained";
    }

    /// <summary>
    /// Get all currently sleeping pets
    /// </summary>
    public List<int> GetSleepingPetIds()
    {
        return new List<int>(sleepingPets.Keys);
    }

    /// <summary>
    /// Get detailed sleep info for all sleeping pets
    /// </summary>
    public void LogAllSleepingPets()
    {
        if (sleepingPets.Count == 0)
        {
            Debug.Log("🛌 No pets are currently sleeping");
            return;
        }

        Debug.Log($"🛌 Currently sleeping pets ({sleepingPets.Count}):");
        foreach (var kvp in sleepingPets)
        {
            int petID = kvp.Key;
            var sleepData = kvp.Value;
            float remainingTime = GetRemainingSleepTime(petID);
            Debug.Log($"  - Pet {petID}: {remainingTime:F1}s remaining, {sleepData.totalEnergyGained} energy gained");
        }
    }

    /// <summary>
    /// Configure energy recovery settings at runtime
    /// </summary>
    public void ConfigureEnergyRecovery(float updateInterval, int energyPerInterval, int maxEnergy, bool enabled = true)
    {
        this.energyUpdateInterval = updateInterval;
        this.energyPerInterval = energyPerInterval;
        this.maxEnergyValue = maxEnergy;
        this.enableEnergyRecovery = enabled;
        
        Debug.Log($"⚡ Energy recovery configured: {energyPerInterval} energy every {updateInterval}s (max: {maxEnergy})");
    }

    /// <summary>
    /// Get current energy recovery rate (energy per second)
    /// </summary>
    public float GetEnergyRecoveryRate()
    {
        if (!enableEnergyRecovery || energyUpdateInterval <= 0) return 0f;
        return energyPerInterval / energyUpdateInterval;
    }

    /// <summary>
    /// Configure auto wake up behavior
    /// </summary>
    public void SetAutoWakeUpOnFullEnergy(bool enabled)
    {
        autoWakeUpOnFullEnergy = enabled;
        Debug.Log($"🛌 Auto wake up on full energy: {(enabled ? "Enabled" : "Disabled")}");
    }

    /// <summary>
    /// Wake up pet if sleeping before performing feeding action
    /// This should be called before any feeding action to ensure pet is awake to eat
    /// </summary>
    public bool WakeUpPetForFeeding(int playerPetID)
    {
        if (IsPetSleeping(playerPetID))
        {
            Debug.Log($"🍎 Pet {playerPetID} is sleeping - waking up for feeding");
            WakePetUp(playerPetID);
            return true; // Pet was sleeping and is now awake
        }
        return false; // Pet was already awake
    }

    /// <summary>
    /// Wake up pet if sleeping before performing any care action
    /// </summary>
    public bool WakeUpPetForCareAction(int playerPetID, PetAction.ActionType actionType)
    {
        // Define which actions require pet to be awake
        bool requiresAwakePet = actionType == PetAction.ActionType.Feed || 
                               actionType == PetAction.ActionType.Play;

        if (requiresAwakePet && IsPetSleeping(playerPetID))
        {
            Debug.Log($"🎯 Pet {playerPetID} is sleeping - waking up for {actionType} action");
            WakePetUp(playerPetID);
            return true; // Pet was sleeping and is now awake
        }
        return false; // Pet was already awake or action doesn't require awakening
    }

    /// <summary>
    /// Check if pet should be woken up for a specific action
    /// </summary>
    public bool ShouldWakeUpForAction(int playerPetID, PetAction.ActionType actionType)
    {
        if (!IsPetSleeping(playerPetID))
            return false; // Pet is already awake

        // Define actions that require pet to be awake
        switch (actionType)
        {
            case PetAction.ActionType.Feed:
            case PetAction.ActionType.Play:
                return true; // These actions require pet to be awake

            case PetAction.ActionType.Sleep:
                return false; // Pet is already sleeping

            case PetAction.ActionType.CareForAll:
                return true; // Complex care might include feeding/playing

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