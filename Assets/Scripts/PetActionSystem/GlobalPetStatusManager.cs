using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages status decay for all pets in the scene, independent of UI state
/// </summary>
public class GlobalPetStatusManager : MonoBehaviour
{
    [Header("Global Decay Settings")]
    [Tooltip("Time in seconds between status decay updates")]
    public float decayInterval = 300f; // 5 minutes
    [Tooltip("Amount to reduce hunger by on decay")]
    public int hungerDecayAmount = 5;
    [Tooltip("Amount to reduce happiness by on decay")]
    public int happinessDecayAmount = 5;
    [Tooltip("Amount to reduce energy by on decay")]
    public int energyDecayAmount = 5;
    [Tooltip("Minimum status value before decay stops")]
    public int minStatusValue = 10;
    
    [Header("Debug Settings")]
    [Tooltip("Show detailed decay logs")]
    public bool showDecayLogs = true;
    [Tooltip("Enable decay system")]
    public bool enableDecay = true;
    
    // Singleton instance
    public static GlobalPetStatusManager Instance { get; private set; }
    
    // Track all pets and their decay coroutines
    private Dictionary<int, Coroutine> petDecayCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, PlayerPet> cachedPetData = new Dictionary<int, PlayerPet>();
    
    // Events
    public System.Action<int, string> OnPetStatusDecayed; // (petID, newStatus)
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("🌍 GlobalPetStatusManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Auto-discover all pets in scene and start decay
        StartCoroutine(InitializePetDecaySystem());
    }
    
    /// <summary>
    /// Initialize decay system for all pets in the scene
    /// </summary>
    private IEnumerator InitializePetDecaySystem()
    {
        yield return new WaitForSeconds(1f); // Wait for scene to fully load
        
        if (!enableDecay)
        {
            Debug.Log("🌍 Global pet decay system is disabled");
            yield break;
        }
        
        // Find all pets in the scene
        DiscoverAllPets();
        
        // Start periodic discovery of new pets
        StartCoroutine(PeriodicPetDiscovery());
        
        Debug.Log($"🌍 Global pet decay system started - checking every {decayInterval}s");
    }
    
    /// <summary>
    /// Discover all pets currently in the scene
    /// </summary>
    private void DiscoverAllPets()
    {
        // Find pets through PetDataHolder components
        PetDataHolder[] petHolders = FindObjectsOfType<PetDataHolder>();
        
        foreach (var holder in petHolders)
        {
            if (holder.petData != null && holder.petData.playerPetID > 0)
            {
                RegisterPetForDecay(holder.petData.playerPetID);
            }
        }
        
        // Also check current user's pets from API
        StartCoroutine(DiscoverUserPets());
    }
    
    /// <summary>
    /// Discover current user's pets from API
    /// </summary>
    private IEnumerator DiscoverUserPets()
    {
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser != null)
        {
            yield return new WaitForEndOfFrame();
            
            try
            {
                List<PlayerPet> userPets = APIPlayerPet.GetPlayerPetByPlayerId(currentUser.id);
                if (userPets != null)
                {
                    foreach (var pet in userPets)
                    {
                        RegisterPetForDecay(pet.playerPetID);
                    }
                    
                    if (showDecayLogs)
                        Debug.Log($"🌍 Discovered {userPets.Count} pets for user {currentUser.id}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to discover user pets: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Periodically discover new pets that might be spawned
    /// </summary>
    private IEnumerator PeriodicPetDiscovery()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(60f); // Check every minute for new pets
            DiscoverAllPets();
        }
    }
    
    /// <summary>
    /// Register a pet for decay monitoring
    /// </summary>
    public void RegisterPetForDecay(int playerPetID)
    {
        if (petDecayCoroutines.ContainsKey(playerPetID))
        {
            if (showDecayLogs)
                Debug.Log($"🌍 Pet {playerPetID} already registered for decay");
            return;
        }
        
        // Start decay coroutine for this pet
        Coroutine decayCoroutine = StartCoroutine(PetDecayCoroutine(playerPetID));
        petDecayCoroutines[playerPetID] = decayCoroutine;
        
        if (showDecayLogs)
            Debug.Log($"🌍 Registered pet {playerPetID} for global decay monitoring");
    }
    
    /// <summary>
    /// Unregister a pet from decay monitoring
    /// </summary>
    public void UnregisterPetFromDecay(int playerPetID)
    {
        if (petDecayCoroutines.ContainsKey(playerPetID))
        {
            StopCoroutine(petDecayCoroutines[playerPetID]);
            petDecayCoroutines.Remove(playerPetID);
            cachedPetData.Remove(playerPetID);
            
            if (showDecayLogs)
                Debug.Log($"🌍 Unregistered pet {playerPetID} from decay monitoring");
        }
    }
    
    /// <summary>
    /// Individual pet decay coroutine
    /// </summary>
    private IEnumerator PetDecayCoroutine(int playerPetID)
    {
        // Initial delay before first decay
        yield return new WaitForSeconds(decayInterval);
        
        while (enabled)
        {
            if (!enableDecay)
            {
                yield return new WaitForSeconds(decayInterval);
                continue;
            }
            
            // Check if pet is sleeping
            bool isPetSleeping = PetSleepManager.Instance != null && 
                               PetSleepManager.Instance.IsPetSleeping(playerPetID);
            
            // ========== SKIP DECAY FOR SLEEPING PETS ==========
            if (isPetSleeping)
            {
                if (showDecayLogs)
                    Debug.Log($"🌍 Pet {playerPetID} is sleeping - skipping all decay to prevent database conflicts");
                
                yield return new WaitForSeconds(decayInterval);
                continue;
            }
            
            // ========== CHECK IF PET JUST WOKE UP - INVALIDATE CACHE ==========
            InvalidatePetCache(playerPetID);
            
            // Get current pet data (now fresh from database)
            PlayerPet petData = GetPetData(playerPetID);
            if (petData != null)
            {
                bool wasDecayed = ProcessPetDecay(petData, isPetSleeping);
                
                if (wasDecayed)
                {
                    // Update in database
                    yield return StartCoroutine(UpdatePetInDatabase(petData));
                    
                    // Cache the updated data
                    cachedPetData[playerPetID] = petData;
                    
                    // Fire event for UI updates
                    OnPetStatusDecayed?.Invoke(playerPetID, petData.status);
                    
                    // Update PetInfoUIManager if it's showing this pet
                    UpdatePetInfoUIIfActive(playerPetID, petData.status);
                }
            }
            
            yield return new WaitForSeconds(decayInterval);
        }
    }
    
    /// <summary>
    /// Process decay for a specific pet
    /// </summary>
    private bool ProcessPetDecay(PlayerPet petData, bool isPetSleeping)
    {
        string[] statusValues = petData.status.Split('%');
        if (statusValues.Length < 3) return false;
        
        bool wasDecayed = false;
        
        if (int.TryParse(statusValues[0], out int hunger) &&
            int.TryParse(statusValues[1], out int happiness) &&
            int.TryParse(statusValues[2], out int energy))
        {
            // Always decay hunger and happiness
            if (hunger > minStatusValue)
            {
                hunger -= hungerDecayAmount;
                wasDecayed = true;
            }
            
            if (happiness > minStatusValue)
            {
                happiness -= happinessDecayAmount;
                wasDecayed = true;
            }
            
            // Only decay energy if pet is not sleeping
            if (!isPetSleeping && energy > minStatusValue)
            {
                energy -= energyDecayAmount;
                wasDecayed = true;
            }
            
            if (wasDecayed)
            {
                petData.status = $"{hunger}%{happiness}%{energy}";
                
                if (showDecayLogs)
                {
                    if (isPetSleeping)
                    {
                        Debug.Log($"🌍 Pet {petData.playerPetID} global decay (sleeping - energy preserved): {petData.status}");
                    }
                    else
                    {
                        Debug.Log($"🌍 Pet {petData.playerPetID} global decay: {petData.status}");
                    }
                }
            }
        }
        
        return wasDecayed;
    }
    
    /// <summary>
    /// Get pet data (cached or from API)
    /// </summary>
    private PlayerPet GetPetData(int playerPetID)
    {
        // Try cached data first
        if (cachedPetData.ContainsKey(playerPetID))
        {
            return cachedPetData[playerPetID];
        }
        
        // Get from API
        try
        {
            PlayerPet petData = APIPlayerPet.GetPlayerPetById(playerPetID);
            if (petData != null)
            {
                cachedPetData[playerPetID] = petData;
            }
            return petData;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to get pet data for {playerPetID}: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Update pet in database
    /// </summary>
    private IEnumerator UpdatePetInDatabase(PlayerPet petData)
    {
        yield return APIPlayerPet.UpdatePlayerPetCoroutine(petData, success =>
        {
            if (!success)
            {
                Debug.LogError($"🌍 Failed to update pet {petData.playerPetID} in database during global decay");
            }
        });
    }
    
    /// <summary>
    /// Update PetInfoUIManager if it's showing this pet
    /// </summary>
    private void UpdatePetInfoUIIfActive(int playerPetID, string newStatus)
    {
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null && petInfoManager.IsPanelActive())
        {
            var (currentPetId, _) = petInfoManager.GetCurrentPetAndPlayerId();
            if (currentPetId == playerPetID)
            {
                // Update the UI status bars
                if (petInfoManager.statusBarManager != null)
                {
                    petInfoManager.statusBarManager.UpdatePetStatus(newStatus);
                }
                
                if (showDecayLogs)
                    Debug.Log($"🌍 Updated PetInfoUIManager for pet {playerPetID}");
            }
        }
    }
    
    /// <summary>
    /// Get list of all pets being monitored
    /// </summary>
    public List<int> GetMonitoredPets()
    {
        return new List<int>(petDecayCoroutines.Keys);
    }
    
    /// <summary>
    /// Manual refresh of pet data cache
    /// </summary>
    public void RefreshPetDataCache()
    {
        cachedPetData.Clear();
        Debug.Log("🌍 Pet data cache refreshed");
    }
    
    private void OnDestroy()
    {
        // Stop all decay coroutines
        foreach (var coroutine in petDecayCoroutines.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        petDecayCoroutines.Clear();
        cachedPetData.Clear();
    }
    
    /// <summary>
    /// Invalidate pet cache for a specific pet
    /// </summary>
    private void InvalidatePetCache(int playerPetID)
    {
        if (cachedPetData.ContainsKey(playerPetID))
        {
            cachedPetData.Remove(playerPetID);
            if (showDecayLogs)
                Debug.Log($"🌍 Invalidated cache for pet {playerPetID}");
        }
    }
}