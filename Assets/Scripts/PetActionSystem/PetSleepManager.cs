using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages pet sleeping state, including movement blocking and action restrictions
/// </summary>
public class PetSleepManager : MonoBehaviour
{
    [Header("Sleep Settings")]
    [SerializeField] private float defaultSleepDuration = 10f;
    [SerializeField] private bool showSleepVisuals = true;
    [SerializeField] private GameObject sleepEffectPrefab;
    [SerializeField] private int expRewardPerSleep = 5; // Experience gained per sleep action
    
    [Header("Movement Blocking")]
    [SerializeField] private bool blockMovementDuringSleep = true;
    [SerializeField] private bool blockActionsDuringSleep = true;
    
    // Singleton instance
    public static PetSleepManager Instance { get; private set; }
    
    // Track sleeping pets
    private Dictionary<int, SleepingPetData> sleepingPets = new Dictionary<int, SleepingPetData>();
    
    // Events
    public System.Action<int> OnPetStartSleep;
    public System.Action<int> OnPetWakeUp;
    
    private struct SleepingPetData
    {
        public GameObject petObject;
        public Coroutine sleepCoroutine;
        public GameObject sleepEffect;
        public List<MonoBehaviour> disabledComponents;
        public float sleepStartTime;
        public float sleepDuration;
        public bool expAwarded; // Track if experience has been awarded for this sleep
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

    /// <summary>
    /// Put pet to sleep for specified duration
    /// </summary>
    public void PutPetToSleep(int playerPetID, float duration = 0f)
    {
        // Use default duration if none specified
        if (duration <= 0f)
            duration = defaultSleepDuration;

        // Find pet in scene
        GameObject petObject = FindPetById(playerPetID);
        if (petObject == null)
        {
            Debug.LogWarning($"Pet with ID {playerPetID} not found in scene!");
            return;
        }

        // Check if pet is already sleeping
        if (sleepingPets.ContainsKey(playerPetID))
        {
            Debug.LogWarning($"Pet {playerPetID} is already sleeping!");
            return;
        }

        // Add experience for putting pet to sleep
        AddExperienceForSleep();

        StartCoroutine(SleepPetCoroutine(playerPetID, petObject, duration));
    }

    /// <summary>
    /// Adds experience to the player when putting a pet to sleep
    /// </summary>
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

    /// <summary>
    /// Wake up pet immediately
    /// </summary>
    public void WakePetUp(int petID)
    {
        if (sleepingPets.ContainsKey(petID))
        {
            SleepingPetData sleepData = sleepingPets[petID];
            
            // Stop sleep coroutine
            if (sleepData.sleepCoroutine != null)
                StopCoroutine(sleepData.sleepCoroutine);
            
            // Wake up pet
            WakeUpPetInternal(petID);
        }
        else
        {
            Debug.LogWarning($"Pet {petID} is not currently sleeping!");
        }
    }

    /// <summary>
    /// Check if pet is currently sleeping
    /// </summary>
    public bool IsPetSleeping(int playerPetID)
    {
        return sleepingPets.ContainsKey(playerPetID);
    }

    /// <summary>
    /// Get remaining sleep time for pet
    /// </summary>
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

    private IEnumerator SleepPetCoroutine(int playerPetID, GameObject petObject, float duration)
    {
        Debug.Log($"🛌 Pet {playerPetID} is going to sleep for {duration} seconds");

        // Disable movement and other components
        List<MonoBehaviour> disabledComponents = DisablePetComponents(petObject);
        
        // Create sleep visual effect
        GameObject sleepEffect = null;
        if (showSleepVisuals && sleepEffectPrefab != null)
        {
            sleepEffect = Instantiate(sleepEffectPrefab, petObject.transform.position + Vector3.up * 2f, Quaternion.identity);
            sleepEffect.transform.SetParent(petObject.transform);
        }

        // Store sleep data
        SleepingPetData sleepData = new SleepingPetData
        {
            petObject = petObject,
            sleepCoroutine = null, // Will be set after this method
            sleepEffect = sleepEffect,
            disabledComponents = disabledComponents,
            sleepStartTime = Time.time,
            sleepDuration = duration,
            expAwarded = true // Experience already awarded when sleep started
        };
        
        sleepingPets[playerPetID] = sleepData;

        // Reset movement animation parameters
        Animator animator = petObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", 0);
        }

        // Fire sleep started event
        OnPetStartSleep?.Invoke(playerPetID);

        // Wait for sleep duration
        yield return new WaitForSeconds(duration);

        // Wake up pet
        WakeUpPetInternal(playerPetID);
    }

    private void WakeUpPetInternal(int playerPetID)
    {
        if (!sleepingPets.ContainsKey(playerPetID))
            return;

        SleepingPetData sleepData = sleepingPets[playerPetID];
        
        Debug.Log($"😴 Pet {playerPetID} is waking up");

        // Re-enable components
        EnablePetComponents(sleepData.disabledComponents);

        // Remove sleep visual effect
        if (sleepData.sleepEffect != null)
            Destroy(sleepData.sleepEffect);

        // Remove from sleeping pets
        sleepingPets.Remove(playerPetID);

        // Fire wake up event
        OnPetWakeUp?.Invoke(playerPetID);
    }

    private List<MonoBehaviour> DisablePetComponents(GameObject petObject)
    {
        List<MonoBehaviour> disabledComponents = new List<MonoBehaviour>();

        if (blockMovementDuringSleep)
        {
            // Only disable movement scripts attached directly to this pet GameObject

            // Disable NPCRandomMovement if exists
            NPCRandomMovement npcMovement = petObject.GetComponent<NPCRandomMovement>();
            if (npcMovement != null && npcMovement.enabled)
            {
                npcMovement.enabled = false;
                disabledComponents.Add(npcMovement);
            }

            // Disable other movement-related scripts (by name) attached directly to petObject
            foreach (var component in petObject.GetComponents<MonoBehaviour>())
            {
                if (component == npcMovement) continue; // Already handled

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

        // Try to find pet using PetDataHolder
        foreach (var holder in petHolders)
        {
            if (holder.petData != null && holder.petData.playerPetID == playerPetID)
            {
                return holder.gameObject;
            }
        }

        // Fallback: try to find by PetClickHandler
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

    /// <summary>
    /// Check if any actions should be blocked for sleeping pet
    /// </summary>
    public bool ShouldBlockAction(int playerPetID, PetAction.ActionType actionType)
    {
        if (!blockActionsDuringSleep || !IsPetSleeping(playerPetID))
            return false;

        // Block all actions except wake up during sleep
        return actionType != PetAction.ActionType.Sleep;
    }

    /// <summary>
    /// Get all currently sleeping pets
    /// </summary>
    public List<int> GetSleepingPetIds()
    {
        return new List<int>(sleepingPets.Keys);
    }

    private void OnDestroy()
    {
        // Wake up all pets when manager is destroyed
        List<int> sleepingPetIds = new List<int>(sleepingPets.Keys);
        foreach (int playerPetID in sleepingPetIds)
        {
            WakePetUp(playerPetID);
        }
    }
}