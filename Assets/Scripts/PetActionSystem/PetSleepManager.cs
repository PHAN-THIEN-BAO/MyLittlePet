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
    [Header("Movement Blocking")]
    [SerializeField] private bool blockMovementDuringSleep = true;
    [SerializeField] private bool blockActionsDuringSleep = true;
    public static PetSleepManager Instance { get; private set; }
    private Dictionary<int, SleepingPetData> sleepingPets = new Dictionary<int, SleepingPetData>();
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
        public bool expAwarded;
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
            sleepEffect = sleepEffect,
            disabledComponents = disabledComponents,
            sleepStartTime = Time.time,
            sleepDuration = duration,
            expAwarded = true
        };
        sleepingPets[playerPetID] = sleepData;
        Animator animator = petObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", 0);
        }
        OnPetStartSleep?.Invoke(playerPetID);
        yield return new WaitForSeconds(duration);
        WakeUpPetInternal(playerPetID);
    }
    private void WakeUpPetInternal(int playerPetID)
    {
        if (!sleepingPets.ContainsKey(playerPetID))
            return;
        SleepingPetData sleepData = sleepingPets[playerPetID];
        Debug.Log($"?? Pet {playerPetID} is waking up");
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
    public List<int> GetSleepingPetIds()
    {
        return new List<int>(sleepingPets.Keys);
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