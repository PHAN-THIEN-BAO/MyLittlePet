using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetAction
{
    public enum ActionType
    {
        Feed,
        Play,
        Sleep,
        LevelUp,
        StatusDecay,
        CareForAll,
        UpdateDatabase
    }

    public enum ActionPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }

    public string actionId;
    public ActionType type;
    public ActionPriority priority;
    public float executionTime;
    public float duration;
    public bool isCompleted;
    public bool isExecuting;
    
    // Dependencies - actions that must complete before this one can execute
    public List<string> dependencies;
    
    // Action parameters
    public Dictionary<string, object> parameters;
    
    // Events
    public System.Action<PetAction> OnActionStarted;
    public System.Action<PetAction> OnActionCompleted;
    public System.Action<PetAction> OnActionFailed;

    public PetAction(string id, ActionType actionType, ActionPriority actionPriority = ActionPriority.Normal)
    {
        actionId = id;
        type = actionType;
        priority = actionPriority;
        dependencies = new List<string>();
        parameters = new Dictionary<string, object>();
        isCompleted = false;
        isExecuting = false;
        executionTime = Time.time;
        duration = 0f;
    }

    public void AddDependency(string dependencyId)
    {
        if (!dependencies.Contains(dependencyId))
        {
            dependencies.Add(dependencyId);
        }
    }

    public void SetParameter(string key, object value)
    {
        parameters[key] = value;
    }

    public T GetParameter<T>(string key, T defaultValue = default(T))
    {
        if (parameters.ContainsKey(key) && parameters[key] is T)
        {
            return (T)parameters[key];
        }
        return defaultValue;
    }
}