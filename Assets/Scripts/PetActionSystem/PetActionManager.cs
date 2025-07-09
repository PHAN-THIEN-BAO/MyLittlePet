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
    
    // Action storage
    private Dictionary<string, PetAction> allActions = new Dictionary<string, PetAction>();
    private Queue<PetAction> actionQueue = new Queue<PetAction>();
    private List<PetAction> executingActions = new List<PetAction>();
    private List<PetAction> completedActions = new List<PetAction>();
    
    // Dependency tracking
    private Dictionary<string, HashSet<string>> dependencyGraph = new Dictionary<string, HashSet<string>>();
    private Dictionary<string, int> inDegree = new Dictionary<string, int>();
    
    // Reference to PetInfoUIManager
    private PetInfoUIManager petInfoManager;
    
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
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager == null)
        {
            Debug.LogWarning("PetInfoUIManager not found! Some actions may not work properly.");
        }
    }

    private void Update()
    {
        ProcessActionQueue();
        UpdateExecutingActions();
    }

    // Add action with automatic topological sorting
    public void AddAction(PetAction action)
    {
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

        // Re-queue actions in topological order
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

    // Build dependency graph for topological sorting
    private void BuildDependencyGraph()
    {
        dependencyGraph.Clear();
        inDegree.Clear();

        // Initialize
        foreach (var action in allActions.Values)
        {
            dependencyGraph[action.actionId] = new HashSet<string>();
            inDegree[action.actionId] = 0;
        }

        // Build edges
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

    // Topological sorting using Kahn's algorithm
    private List<PetAction> TopologicalSort()
    {
        var result = new List<PetAction>();
        var queue = new Queue<string>();
        var tempInDegree = new Dictionary<string, int>(inDegree);

        // Add all nodes with no incoming edges
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

            // Remove edges from current node
            foreach (string neighbor in dependencyGraph[currentId])
            {
                tempInDegree[neighbor]--;
                if (tempInDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Check for circular dependencies
        if (result.Count != allActions.Count)
        {
            return null; // Circular dependency detected
        }

        // Sort by priority within topological order
        return result.OrderBy(a => GetTopologicalIndex(a, result))
                    .ThenByDescending(a => (int)a.priority)
                    .ToList();
    }

    private int GetTopologicalIndex(PetAction action, List<PetAction> topologicalOrder)
    {
        return topologicalOrder.FindIndex(a => a.actionId == action.actionId);
    }

    // Process the action queue
    private void ProcessActionQueue()
    {
        while (actionQueue.Count > 0 && executingActions.Count < maxConcurrentActions)
        {
            PetAction nextAction = actionQueue.Dequeue();
            
            // Check if dependencies are met
            if (AreDependenciesMet(nextAction))
            {
                StartCoroutine(ExecuteAction(nextAction));
            }
            else
            {
                // Re-queue if dependencies not met
                actionQueue.Enqueue(nextAction);
                break; // Avoid infinite loop
            }
        }
    }

    // Check if all dependencies are completed
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

    // Execute action coroutine
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

        // Execute the actual action based on type
        yield return StartCoroutine(ExecuteActionByType(action));

        // Mark as completed
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

    // Execute specific action types
    private IEnumerator ExecuteActionByType(PetAction action)
    {
        if (petInfoManager == null)
        {
            Debug.LogWarning("PetInfoUIManager not found! Cannot execute pet actions.");
            yield break;
        }

        switch (action.type)
        {
            case PetAction.ActionType.Feed:
                int feedAmount = action.GetParameter("amount", petInfoManager.feedIncreaseAmount);
                petInfoManager.UpdatePetStatus(0, feedAmount); // 0 = hunger
                yield return new WaitForSeconds(0.5f);
                break;

            case PetAction.ActionType.Play:
                int playAmount = action.GetParameter("amount", petInfoManager.playIncreaseAmount);
                petInfoManager.UpdatePetStatus(1, playAmount); // 1 = happiness
                yield return new WaitForSeconds(1.0f);
                break;

            case PetAction.ActionType.Sleep:
                int sleepAmount = action.GetParameter("amount", petInfoManager.sleepIncreaseAmount);
                petInfoManager.UpdatePetStatus(2, sleepAmount); // 2 = energy
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

            case PetAction.ActionType.StatusDecay:
                // This would be handled by the existing decay system
                yield return new WaitForSeconds(0.1f);
                break;

            case PetAction.ActionType.UpdateDatabase:
                yield return StartCoroutine(petInfoManager.UpdatePetInDatabase());
                break;

            case PetAction.ActionType.LevelUp:
                // Implement level up logic here
                yield return new WaitForSeconds(1.0f);
                break;

            default:
                yield return new WaitForSeconds(0.1f);
                break;
        }
    }

    // Update executing actions
    private void UpdateExecutingActions()
    {
        for (int i = executingActions.Count - 1; i >= 0; i--)
        {
            PetAction action = executingActions[i];
            
            // Check for timeout or other failure conditions
            if (Time.time - action.executionTime > 10f) // 10 second timeout
            {
                Debug.LogWarning($"Action {action.actionId} timed out!");
                action.OnActionFailed?.Invoke(action);
                
                action.isExecuting = false;
                executingActions.RemoveAt(i);
            }
        }
    }

    // Convenience methods for adding common actions
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

    public void PetSleep(int amount = 0, string dependsOn = null)
    {
        var action = new PetAction($"sleep_{Time.time}", PetAction.ActionType.Sleep, PetAction.ActionPriority.Normal);
        if (amount > 0) action.SetParameter("amount", amount);
        if (!string.IsNullOrEmpty(dependsOn)) action.AddDependency(dependsOn);
        AddAction(action);
    }

    public void CareForAll(string dependsOn = null)
    {
        var action = new PetAction($"careall_{Time.time}", PetAction.ActionType.CareForAll, PetAction.ActionPriority.High);
        if (!string.IsNullOrEmpty(dependsOn)) action.AddDependency(dependsOn);
        AddAction(action);
    }

    // Clear all actions
    public void ClearAllActions()
    {
        allActions.Clear();
        actionQueue.Clear();
        executingActions.Clear();
        completedActions.Clear();
        dependencyGraph.Clear();
        inDegree.Clear();
    }

    // Get action status
    public bool IsActionCompleted(string actionId)
    {
        return allActions.ContainsKey(actionId) && allActions[actionId].isCompleted;
    }

    public bool IsActionExecuting(string actionId)
    {
        return allActions.ContainsKey(actionId) && allActions[actionId].isExecuting;
    }
}