using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPetStatusManager : MonoBehaviour
{
    [Header("Global Decay Settings")]
    [Tooltip("Time in seconds between status decay updates")]
    public float decayInterval = 300f;
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
    
    public static GlobalPetStatusManager Instance { get; private set; }
    
    private Dictionary<int, Coroutine> petDecayCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, PlayerPet> cachedPetData = new Dictionary<int, PlayerPet>();
    
    public System.Action<int, string> OnPetStatusDecayed;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("?? GlobalPetStatusManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        StartCoroutine(InitializePetDecaySystem());
    }
    
    private IEnumerator InitializePetDecaySystem()
    {
        yield return new WaitForSeconds(1f);
        
        if (!enableDecay)
        {
            Debug.Log("?? Global pet decay system is disabled");
            yield break;
        }
        
        DiscoverAllPets();
        
        StartCoroutine(PeriodicPetDiscovery());
        
        Debug.Log($"?? Global pet decay system started - checking every {decayInterval}s");
    }
    
    private void DiscoverAllPets()
    {
        PetDataHolder[] petHolders = FindObjectsOfType<PetDataHolder>();
        
        foreach (var holder in petHolders)
        {
            if (holder.petData != null && holder.petData.playerPetID > 0)
            {
                RegisterPetForDecay(holder.petData.playerPetID);
            }
        }
        
        StartCoroutine(DiscoverUserPets());
    }
    
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
                        Debug.Log($"?? Discovered {userPets.Count} pets for user {currentUser.id}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to discover user pets: {ex.Message}");
            }
        }
    }
    
    private IEnumerator PeriodicPetDiscovery()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(60f);
            DiscoverAllPets();
        }
    }
    
    public void RegisterPetForDecay(int playerPetID)
    {
        if (petDecayCoroutines.ContainsKey(playerPetID))
        {
            if (showDecayLogs)
                Debug.Log($"?? Pet {playerPetID} already registered for decay");
            return;
        }
        
        Coroutine decayCoroutine = StartCoroutine(PetDecayCoroutine(playerPetID));
        petDecayCoroutines[playerPetID] = decayCoroutine;
        
        if (showDecayLogs)
            Debug.Log($"?? Registered pet {playerPetID} for global decay monitoring");
    }
    
    public void UnregisterPetFromDecay(int playerPetID)
    {
        if (petDecayCoroutines.ContainsKey(playerPetID))
        {
            StopCoroutine(petDecayCoroutines[playerPetID]);
            petDecayCoroutines.Remove(playerPetID);
            cachedPetData.Remove(playerPetID);
            
            if (showDecayLogs)
                Debug.Log($"?? Unregistered pet {playerPetID} from decay monitoring");
        }
    }
    
    private IEnumerator PetDecayCoroutine(int playerPetID)
    {
        yield return new WaitForSeconds(decayInterval);
        
        while (enabled)
        {
            if (!enableDecay)
            {
                yield return new WaitForSeconds(decayInterval);
                continue;
            }
            
            bool isPetSleeping = PetSleepManager.Instance != null && 
                               PetSleepManager.Instance.IsPetSleeping(playerPetID);
            
            if (isPetSleeping)
            {
                if (showDecayLogs)
                    Debug.Log($"?? Pet {playerPetID} is sleeping - skipping all decay to prevent database conflicts");
                
                yield return new WaitForSeconds(decayInterval);
                continue;
            }
            
            InvalidatePetCache(playerPetID);
            
            PlayerPet petData = GetPetData(playerPetID);
            if (petData != null)
            {
                bool wasDecayed = ProcessPetDecay(petData, isPetSleeping);
                
                if (wasDecayed)
                {
                    yield return StartCoroutine(UpdatePetInDatabase(petData));
                    
                    cachedPetData[playerPetID] = petData;
                    
                    OnPetStatusDecayed?.Invoke(playerPetID, petData.status);
                    
                    UpdatePetInfoUIIfActive(playerPetID, petData.status);
                }
            }
            
            yield return new WaitForSeconds(decayInterval);
        }
    }
    
    private bool ProcessPetDecay(PlayerPet petData, bool isPetSleeping)
    {
        string[] statusValues = petData.status.Split('%');
        if (statusValues.Length < 3) return false;
        
        bool wasDecayed = false;
        
        if (int.TryParse(statusValues[0], out int hunger) &&
            int.TryParse(statusValues[1], out int happiness) &&
            int.TryParse(statusValues[2], out int energy))
        {
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
                        Debug.Log($"?? Pet {petData.playerPetID} global decay (sleeping - energy preserved): {petData.status}");
                    }
                    else
                    {
                        Debug.Log($"?? Pet {petData.playerPetID} global decay: {petData.status}");
                    }
                }
            }
        }
        
        return wasDecayed;
    }
    
    private PlayerPet GetPetData(int playerPetID)
    {
        if (cachedPetData.ContainsKey(playerPetID))
        {
            return cachedPetData[playerPetID];
        }
        
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
    
    private IEnumerator UpdatePetInDatabase(PlayerPet petData)
    {
        yield return APIPlayerPet.UpdatePlayerPetCoroutine(petData, success =>
        {
            if (!success)
            {
                Debug.LogError($"?? Failed to update pet {petData.playerPetID} in database during global decay");
            }
        });
    }
    
    private void UpdatePetInfoUIIfActive(int playerPetID, string newStatus)
    {
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null && petInfoManager.IsPanelActive())
        {
            var (currentPetId, _) = petInfoManager.GetCurrentPetAndPlayerId();
            if (currentPetId == playerPetID)
            {
                if (petInfoManager.statusBarManager != null)
                {
                    petInfoManager.statusBarManager.UpdatePetStatus(newStatus);
                }
                
                if (showDecayLogs)
                    Debug.Log($"?? Updated PetInfoUIManager for pet {playerPetID}");
            }
        }
    }
    
    public List<int> GetMonitoredPets()
    {
        return new List<int>(petDecayCoroutines.Keys);
    }
    
    public void RefreshPetDataCache()
    {
        cachedPetData.Clear();
        Debug.Log("?? Pet data cache refreshed");
    }
    
    private void OnDestroy()
    {
        foreach (var coroutine in petDecayCoroutines.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        petDecayCoroutines.Clear();
        cachedPetData.Clear();
    }
    
    private void InvalidatePetCache(int playerPetID)
    {
        if (cachedPetData.ContainsKey(playerPetID))
        {
            cachedPetData.Remove(playerPetID);
            if (showDecayLogs)
                Debug.Log($"?? Invalidated cache for pet {playerPetID}");
        }
    }
}