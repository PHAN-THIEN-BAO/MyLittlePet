using System.Collections.Generic;
using UnityEngine;

public class PetActionScheduler : MonoBehaviour
{
    [Header("Scheduled Action Settings")]
    [SerializeField] private bool enableAutoScheduling = true;
    [SerializeField] private float schedulingInterval = 30f; // 30 seconds
    
    private PetActionManager actionManager;
    private float lastScheduleTime;

    private void Start()
    {
        actionManager = PetActionManager.Instance;
        if (actionManager == null)
        {
            Debug.LogError("PetActionManager not found! Scheduler will not work.");
        }
        
        lastScheduleTime = Time.time;
    }

    private void Update()
    {
        if (enableAutoScheduling && actionManager != null)
        {
            if (Time.time - lastScheduleTime >= schedulingInterval)
            {
                ScheduleMaintenanceActions();
                lastScheduleTime = Time.time;
            }
        }
    }

    // Schedule maintenance actions based on pet status
    private void ScheduleMaintenanceActions()
    {
        var petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager == null) return;

        // Create a sequence of actions with dependencies
        string baseId = $"maintenance_{Time.time}";
        
        // 1. Check status first
        var checkAction = new PetAction($"{baseId}_check", PetAction.ActionType.StatusDecay, PetAction.ActionPriority.High);
        actionManager.AddAction(checkAction);

        // 2. Feed if needed (depends on status check)
        if (!petInfoManager.IsHungerAtMax())
        {
            var feedAction = new PetAction($"{baseId}_feed", PetAction.ActionType.Feed, PetAction.ActionPriority.Normal);
            feedAction.AddDependency(checkAction.actionId);
            actionManager.AddAction(feedAction);
        }

        // 3. Play if needed (depends on status check)
        if (!petInfoManager.IsHappinessAtMax())
        {
            var playAction = new PetAction($"{baseId}_play", PetAction.ActionType.Play, PetAction.ActionPriority.Normal);
            playAction.AddDependency(checkAction.actionId);
            actionManager.AddAction(playAction);
        }

        // 4. Sleep if needed (depends on status check)
        if (!petInfoManager.IsEnergyAtMax())
        {
            var sleepAction = new PetAction($"{baseId}_sleep", PetAction.ActionType.Sleep, PetAction.ActionPriority.Normal);
            sleepAction.AddDependency(checkAction.actionId);
            actionManager.AddAction(sleepAction);
        }

        // 5. Update database (depends on all care actions)
        var updateAction = new PetAction($"{baseId}_update", PetAction.ActionType.UpdateDatabase, PetAction.ActionPriority.Critical);
        updateAction.AddDependency(checkAction.actionId);
        actionManager.AddAction(updateAction);
    }

    // Schedule a complex care sequence
    public void ScheduleCareSequence()
    {
        string sequenceId = $"sequence_{Time.time}";
        
        // Create actions with dependencies to ensure proper order
        var feedAction = new PetAction($"{sequenceId}_feed", PetAction.ActionType.Feed);
        var playAction = new PetAction($"{sequenceId}_play", PetAction.ActionType.Play);
        var sleepAction = new PetAction($"{sequenceId}_sleep", PetAction.ActionType.Sleep);
        var updateAction = new PetAction($"{sequenceId}_update", PetAction.ActionType.UpdateDatabase);
        
        // Set up dependencies: feed -> play -> sleep -> update
        playAction.AddDependency(feedAction.actionId);
        sleepAction.AddDependency(playAction.actionId);
        updateAction.AddDependency(sleepAction.actionId);
        
        // Add all actions (they will be sorted topologically)
        actionManager.AddAction(feedAction);
        actionManager.AddAction(playAction);
        actionManager.AddAction(sleepAction);
        actionManager.AddAction(updateAction);
        
        Debug.Log("Scheduled complete care sequence");
    }
}