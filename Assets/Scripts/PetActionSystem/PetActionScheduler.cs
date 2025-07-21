using System.Collections.Generic;
using UnityEngine;
public class PetActionScheduler : MonoBehaviour
{
    [Header("Scheduled Action Settings")]
    [SerializeField] private bool enableAutoScheduling = true;
    [SerializeField] private float schedulingInterval = 30f;
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
    private void ScheduleMaintenanceActions()
    {
        var petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager == null) return;
        string baseId = $"maintenance_{Time.time}";
        var checkAction = new PetAction($"{baseId}_check", PetAction.ActionType.StatusDecay, PetAction.ActionPriority.High);
        actionManager.AddAction(checkAction);
        if (!petInfoManager.IsHungerAtMax())
        {
            var feedAction = new PetAction($"{baseId}_feed", PetAction.ActionType.Feed, PetAction.ActionPriority.Normal);
            feedAction.AddDependency(checkAction.actionId);
            actionManager.AddAction(feedAction);
        }
        if (!petInfoManager.IsHappinessAtMax())
        {
            var playAction = new PetAction($"{baseId}_play", PetAction.ActionType.Play, PetAction.ActionPriority.Normal);
            playAction.AddDependency(checkAction.actionId);
            actionManager.AddAction(playAction);
        }
        if (!petInfoManager.IsEnergyAtMax())
        {
            var sleepAction = new PetAction($"{baseId}_sleep", PetAction.ActionType.Sleep, PetAction.ActionPriority.Normal);
            sleepAction.AddDependency(checkAction.actionId);
            actionManager.AddAction(sleepAction);
        }
        var updateAction = new PetAction($"{baseId}_update", PetAction.ActionType.UpdateDatabase, PetAction.ActionPriority.Critical);
        updateAction.AddDependency(checkAction.actionId);
        actionManager.AddAction(updateAction);
    }
    public void ScheduleCareSequence()
    {
        string sequenceId = $"sequence_{Time.time}";
        var feedAction = new PetAction($"{sequenceId}_feed", PetAction.ActionType.Feed);
        var playAction = new PetAction($"{sequenceId}_play", PetAction.ActionType.Play);
        var sleepAction = new PetAction($"{sequenceId}_sleep", PetAction.ActionType.Sleep);
        var updateAction = new PetAction($"{sequenceId}_update", PetAction.ActionType.UpdateDatabase);
        playAction.AddDependency(feedAction.actionId);
        sleepAction.AddDependency(playAction.actionId);
        updateAction.AddDependency(sleepAction.actionId);
        actionManager.AddAction(feedAction);
        actionManager.AddAction(playAction);
        actionManager.AddAction(sleepAction);
        actionManager.AddAction(updateAction);
        Debug.Log("Scheduled complete care sequence");
    }
}