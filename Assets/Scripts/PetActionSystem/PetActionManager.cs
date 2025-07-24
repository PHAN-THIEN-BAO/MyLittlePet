using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class PetActionManager : MonoBehaviour
{
    [Header("Action Settings")]
    [SerializeField] private float maxConcurrentActions = 3;
    [SerializeField] private bool enableLogging = true;
    
    [Header("Dependency Management")]
    [SerializeField] private bool requirePetInfoManager = true;
    [SerializeField] private float retryFindManagerInterval = 2f;
    
    private Dictionary<string, PetAction> allActions = new Dictionary<string, PetAction>();
    private Queue<PetAction> actionQueue = new Queue<PetAction>();
    private List<PetAction> executingActions = new List<PetAction>();
    private List<PetAction> completedActions = new List<PetAction>();
    
    private Dictionary<string, HashSet<string>> dependencyGraph = new Dictionary<string, HashSet<string>>();
    private Dictionary<string, int> inDegree = new Dictionary<string, int>();
    
    private PetInfoUIManager petInfoManager;
    private Coroutine findManagerCoroutine;
    
    public static PetActionManager Instance { get; private set; }

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

    private void Start()
    {
        FindPetInfoManager();
        
        if (petInfoManager == null && requirePetInfoManager)
        {
            findManagerCoroutine = StartCoroutine(RetryFindPetInfoManager());
        }
    }

    private void Update()
    {
        if (petInfoManager != null || !requirePetInfoManager)
        {
            ProcessActionQueue();
            UpdateExecutingActions();
        }
    }

    private void FindPetInfoManager()
    {
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null)
        {
            if (enableLogging)
                Debug.Log("? PetInfoUIManager found and connected to PetActionManager");
            
            if (findManagerCoroutine != null)
            {
                StopCoroutine(findManagerCoroutine);
                findManagerCoroutine = null;
            }
        }
        else if (requirePetInfoManager)
        {
            Debug.LogWarning("?? PetInfoUIManager not found! PetActionManager will retry finding it.");
        }
    }

    private IEnumerator RetryFindPetInfoManager()
    {
        while (petInfoManager == null)
        {
            yield return new WaitForSeconds(retryFindManagerInterval);
            FindPetInfoManager();
        }
    }

    public void SetPetInfoUIManager(PetInfoUIManager manager)
    {
        petInfoManager = manager;
        if (enableLogging)
            Debug.Log("? PetInfoUIManager manually set for PetActionManager");
    }

    public void AddAction(PetAction action)
    {
        if (requirePetInfoManager && petInfoManager == null && RequiresPetInfoManager(action.type))
        {
            Debug.LogWarning($"Cannot add action {action.actionId} of type {action.type}: PetInfoUIManager not found!");
            return;
        }

        if (allActions.ContainsKey(action.actionId))
        {
            Debug.LogWarning($"Action with ID {action.actionId} already exists!");
            return;
        }

        allActions[action.actionId] = action;
        BuildDependencyGraph();
        
        List<PetAction> sortedActions = TopologicalSort();
        if (sortedActions == null)
        {
            Debug.LogError("Circular dependency detected! Cannot add action: " + action.actionId);
            allActions.Remove(action.actionId);
            return;
        }

        actionQueue.Clear();
        foreach (var sortedAction in sortedActions)
        {
            if (!sortedAction.isCompleted && !sortedAction.isExecuting)
            {
                actionQueue.Enqueue(sortedAction);
            }
        }

        if (enableLogging)
        {
            Debug.Log($"Added action: {action.actionId} (Type: {action.type}, Priority: {action.priority})");
        }
    }

    private bool RequiresPetInfoManager(PetAction.ActionType actionType)
    {
        switch (actionType)
        {
            case PetAction.ActionType.Feed:
            case PetAction.ActionType.Play:
            case PetAction.ActionType.Sleep:
            case PetAction.ActionType.CareForAll:
            case PetAction.ActionType.UpdateDatabase:
                return true;
            case PetAction.ActionType.StatusDecay:
            case PetAction.ActionType.LevelUp:
            default:
                return false;
        }
    }

    private void BuildDependencyGraph()
    {
        dependencyGraph.Clear();
        inDegree.Clear();

        foreach (var action in allActions.Values)
        {
            dependencyGraph[action.actionId] = new HashSet<string>();
            inDegree[action.actionId] = 0;
        }

        foreach (var action in allActions.Values)
        {
            foreach (string dependency in action.dependencies)
            {
                if (allActions.ContainsKey(dependency))
                {
                    dependencyGraph[dependency].Add(action.actionId);
                    inDegree[action.actionId]++;
                }
            }
        }
    }

    private List<PetAction> TopologicalSort()
    {
        var result = new List<PetAction>();
        var queue = new Queue<string>();
        var tempInDegree = new Dictionary<string, int>(inDegree);

        foreach (var kvp in tempInDegree)
        {
            if (kvp.Value == 0)
            {
                queue.Enqueue(kvp.Key);
            }
        }

        while (queue.Count > 0)
        {
            string currentId = queue.Dequeue();
            result.Add(allActions[currentId]);

            foreach (string neighbor in dependencyGraph[currentId])
            {
                tempInDegree[neighbor]--;
                if (tempInDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (result.Count != allActions.Count)
        {
            return null;
        }

        return result.OrderBy(a => GetTopologicalIndex(a, result))
                    .ThenByDescending(a => (int)a.priority)
                    .ToList();
    }

    private int GetTopologicalIndex(PetAction action, List<PetAction> topologicalOrder)
    {
        return topologicalOrder.FindIndex(a => a.actionId == action.actionId);
    }

    private void ProcessActionQueue()
    {
        while (actionQueue.Count > 0 && executingActions.Count < maxConcurrentActions)
        {
            PetAction nextAction = actionQueue.Dequeue();
            
            if (RequiresPetInfoManager(nextAction.type) && petInfoManager == null)
            {
                if (enableLogging)
                    Debug.LogWarning($"Skipping action {nextAction.actionId}: PetInfoUIManager not available");
                continue;
            }
            
            if (AreDependenciesMet(nextAction))
            {
                StartCoroutine(ExecuteAction(nextAction));
            }
            else
            {
                actionQueue.Enqueue(nextAction);
                break;
            }
        }
    }

    private bool AreDependenciesMet(PetAction action)
    {
        foreach (string dependencyId in action.dependencies)
        {
            if (allActions.ContainsKey(dependencyId))
            {
                PetAction dependency = allActions[dependencyId];
                if (!dependency.isCompleted)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private IEnumerator ExecuteAction(PetAction action)
    {
        action.isExecuting = true;
        executingActions.Add(action);
        action.executionTime = Time.time;

        if (enableLogging)
        {
            Debug.Log($"Executing action: {action.actionId} (Type: {action.type})");
        }

        action.OnActionStarted?.Invoke(action);

        yield return StartCoroutine(ExecuteActionByType(action));

        action.isCompleted = true;
        action.isExecuting = false;
        executingActions.Remove(action);
        completedActions.Add(action);

        action.OnActionCompleted?.Invoke(action);

        if (enableLogging)
        {
            Debug.Log($"Completed action: {action.actionId}");
        }
    }

    private IEnumerator ExecuteActionByType(PetAction action)
    {
        if (!RequiresPetInfoManager(action.type))
        {
            switch (action.type)
            {
                case PetAction.ActionType.StatusDecay:
                    yield return new WaitForSeconds(0.1f);
                    break;
                    
                case PetAction.ActionType.LevelUp:
                    yield return new WaitForSeconds(1.0f);
                    break;
                    
                default:
                    yield return new WaitForSeconds(0.1f);
                    break;
            }
            yield break;
        }

        if (petInfoManager == null)
        {
            Debug.LogError($"Cannot execute action {action.actionId}: PetInfoUIManager is required but not found!");
            action.OnActionFailed?.Invoke(action);
            yield break;
        }

        switch (action.type)
        {
            case PetAction.ActionType.Feed:
                int feedAmount = action.GetParameter("amount", petInfoManager.feedIncreaseAmount);
                petInfoManager.UpdatePetStatus(0, feedAmount);
                yield return new WaitForSeconds(0.5f);
                break;

            case PetAction.ActionType.Play:
                int playAmount = action.GetParameter("amount", petInfoManager.playIncreaseAmount);
                petInfoManager.UpdatePetStatus(1, playAmount);
                yield return new WaitForSeconds(1.0f);
                break;

            case PetAction.ActionType.Sleep:
                int sleepAmount = action.GetParameter("amount", petInfoManager.sleepIncreaseAmount);
                
                if (PetSleepManager.Instance != null)
                {
                    int petID = action.GetParameter("petID", -1);
                    float sleepDuration = action.GetParameter("duration", 5f);
                    
                    if (petID != -1)
                    {
                        PetSleepManager.Instance.PutPetToSleep(petID, sleepDuration);
                        yield return new WaitForSeconds(sleepDuration);
                    }
                }
                
                petInfoManager.UpdatePetStatus(2, sleepAmount);
                yield return new WaitForSeconds(2.0f);
                break;

            case PetAction.ActionType.CareForAll:
                if (!petInfoManager.IsHungerAtMax())
                    petInfoManager.UpdatePetStatus(0, petInfoManager.feedIncreaseAmount);
                yield return new WaitForSeconds(0.2f);
                
                if (!petInfoManager.IsHappinessAtMax())
                    petInfoManager.UpdatePetStatus(1, petInfoManager.playIncreaseAmount);
                yield return new WaitForSeconds(0.2f);
                
                if (!petInfoManager.IsEnergyAtMax())
                    petInfoManager.UpdatePetStatus(2, petInfoManager.sleepIncreaseAmount);
                yield return new WaitForSeconds(0.2f);
                break;

            case PetAction.ActionType.UpdateDatabase:
                yield return StartCoroutine(petInfoManager.UpdatePetInDatabase());
                break;

            default:
                yield return new WaitForSeconds(0.1f);
                break;
        }
    }

    private void UpdateExecutingActions()
    {
        for (int i = executingActions.Count - 1; i >= 0; i--)
        {
            PetAction action = executingActions[i];
            
            if (Time.time - action.executionTime > 10f)
            {
                Debug.LogWarning($"Action {action.actionId} timed out!");
                action.OnActionFailed?.Invoke(action);
                
                action.isExecuting = false;
                executingActions.RemoveAt(i);
            }
        }
    }

    public void FeedPet(int amount = 0, string dependsOn = null)
    {
        var action = new PetAction($"feed_{Time.time}", PetAction.ActionType.Feed, PetAction.ActionPriority.Normal);
        if (amount > 0) action.SetParameter("amount", amount);
        if (!string.IsNullOrEmpty(dependsOn)) action.AddDependency(dependsOn);
        AddAction(action);
    }

    public void PlayWithPet(int amount = 0, string dependsOn = null)
    {
        var action = new PetAction($"play_{Time.time}", PetAction.ActionType.Play, PetAction.ActionPriority.Normal);
        if (amount > 0) action.SetParameter("amount", amount);
        if (!string.IsNullOrEmpty(dependsOn)) action.AddDependency(dependsOn);
        AddAction(action);
    }

    public void PetSleep(int playerPetID, int amount = 0, float duration = 5f, string dependsOn = null)
    {
        var action = new PetAction($"sleep_{Time.time}", PetAction.ActionType.Sleep, PetAction.ActionPriority.Normal);
        if (amount > 0) action.SetParameter("amount", amount);
        action.SetParameter("petID", playerPetID);
        action.SetParameter("duration", duration);
        if (!string.IsNullOrEmpty(dependsOn)) action.AddDependency(dependsOn);
        AddAction(action);
    }

    public void PetSleep(int amount = 0, string dependsOn = null)
    {
        var action = new PetAction($"sleep_{Time.time}", PetAction.ActionType.Sleep, PetAction.ActionPriority.Normal);
        if (amount > 0) action.SetParameter("amount", amount);
        
        if (petInfoManager != null)
        {
            var (currentPetId, _) = petInfoManager.GetCurrentPetAndPlayerId();
            if (currentPetId != -1)
            {
                action.SetParameter("petID", currentPetId);
            }
        }
        
        if (!string.IsNullOrEmpty(dependsOn)) action.AddDependency(dependsOn);
        AddAction(action);
    }

    public void CareForAll(string dependsOn = null)
    {
        var action = new PetAction($"careall_{Time.time}", PetAction.ActionType.CareForAll, PetAction.ActionPriority.High);
        if (!string.IsNullOrEmpty(dependsOn)) action.AddDependency(dependsOn);
        AddAction(action);
    }

    public void ClearAllActions()
    {
        allActions.Clear();
        actionQueue.Clear();
        executingActions.Clear();
        completedActions.Clear();
        dependencyGraph.Clear();
        inDegree.Clear();
    }

    public bool IsActionCompleted(string actionId)
    {
        return allActions.ContainsKey(actionId) && allActions[actionId].isCompleted;
    }

    public bool IsActionExecuting(string actionId)
    {
        return allActions.ContainsKey(actionId) && allActions[actionId].isExecuting;
    }
    
    public bool HasPetInfoUIManager()
    {
        return petInfoManager != null;
    }

    private void OnDestroy()
    {
        if (findManagerCoroutine != null)
        {
            StopCoroutine(findManagerCoroutine);
        }
    }
}