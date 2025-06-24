using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class PetStatusManager : MonoBehaviour
{
    [Header("Decay Settings")]
    [Tooltip("How many seconds between each status update")]
    public float updateInterval = 3600f; // Default: 1 hour
    
    [Tooltip("Amount to decrease each status value per update")]
    public float decayAmount = 5f;
    
    [Tooltip("Should the decay run while the game is closed?")]
    public bool decayWhileGameClosed = true;
    
    [Header("Status Decay Rates")]
    [Range(0.1f, 2f)]
    public float hungerDecayMultiplier = 1.0f;
    
    [Range(0.1f, 2f)]
    public float happinessDecayMultiplier = 0.8f;
    
    [Range(0.1f, 2f)]
    public float energyDecayMultiplier = 0.5f;
    
    [Header("Debug Settings")]
    public bool debugMode = false;
    public float debugUpdateInterval = 10f;
    
    // Event for status updates
    public event Action<PlayerPet> OnPetStatusUpdated;
    
    private List<PlayerPet> playerPets = new List<PlayerPet>();
    private bool isUpdating = false;
    private User currentUser;
    
    // Hardcoded player ID
    private const int PLAYER_ID = 3;
    
    // Flag to track if pets have been fetched
    private bool petsFetched = false;
    
    #region Unity Lifecycle
    
    private void Start()
    {
        // Use hardcoded player ID instead of loading from PlayerInfomation
        currentUser = new User { id = PLAYER_ID };
        
        // Fetch pets only once at startup
        FetchPlayerPets();
        
        StartCoroutine(InitializeStatusDecay());
    }
    
    private void OnApplicationQuit()
    {
        if (decayWhileGameClosed && playerPets.Count > 0)
        {
            UpdateAllPetsInDatabase();
        }
    }
    
    #endregion
    
    #region Status Decay Management
    
    private IEnumerator InitializeStatusDecay()
    {
        yield return new WaitForSeconds(2f);
        
        ProcessElapsedTimeDecay();
        StartCoroutine(StatusDecayRoutine());
    }
    
    private IEnumerator StatusDecayRoutine()
    {
        while (true)
        {
            float interval = debugMode ? debugUpdateInterval : updateInterval;
            
            yield return new WaitForSeconds(interval);
            
            if (!isUpdating)
            {
                isUpdating = true;
                
                // REMOVED: Random pet fetching
                // This was causing pets to be fetched repeatedly during gameplay
                // if (UnityEngine.Random.value < 0.2f)
                // {
                //     FetchPlayerPets();
                // }
                
                ApplyStatusDecay();
                UpdateAllPetsInDatabase();
                
                isUpdating = false;
            }
        }
    }
    
    private void ProcessElapsedTimeDecay()
    {
        if (!decayWhileGameClosed || playerPets.Count == 0) return;
        
        foreach (PlayerPet pet in playerPets)
        {
            TimeSpan elapsed = DateTime.Now - pet.lastStatusUpdate;
            int intervals = Mathf.FloorToInt((float)elapsed.TotalSeconds / updateInterval);
            
            if (intervals > 0)
            {
                UpdatePetStatus(pet, decayAmount * intervals);
            }
        }
        
        UpdateAllPetsInDatabase();
    }
    
    private void ApplyStatusDecay()
    {
        foreach (PlayerPet pet in playerPets)
        {
            UpdatePetStatus(pet, decayAmount);
        }
    }
    
    private void UpdatePetStatus(PlayerPet pet, float decayBase)
    {
        if (pet == null || string.IsNullOrEmpty(pet.status)) return;
        
        try
        {
            string[] statuses = pet.status.Split('%');
            
            if (statuses.Length >= 3)
            {
                float hungerValue = float.Parse(statuses[0]);
                float happinessValue = float.Parse(statuses[1]);
                float energyValue = float.Parse(statuses[2]);
                
                hungerValue = Mathf.Max(0, hungerValue - (decayBase * hungerDecayMultiplier));
                happinessValue = Mathf.Max(0, happinessValue - (decayBase * happinessDecayMultiplier));
                energyValue = Mathf.Max(0, energyValue - (decayBase * energyDecayMultiplier));
                
                pet.status = $"{hungerValue}%{happinessValue}%{energyValue}";
                pet.lastStatusUpdate = DateTime.Now;
                
                // Notify listeners about the status update
                OnPetStatusUpdated?.Invoke(pet);
                
                if (debugMode)
                {
                    Debug.Log($"Updated pet {pet.playerPetID} status: {pet.status}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating pet status: {ex.Message}");
        }
    }
    
    #endregion
    
    #region Database Operations
    
    private void FetchPlayerPets()
    {
        if (petsFetched)
        {
            // Skip if we've already fetched pets
            if (debugMode)
            {
                Debug.Log("Skipping pet fetch - already fetched once");
            }
            return;
        }
        
        try
        {
            // Use the constant player ID
            playerPets = APIPlayerPet.GetPetsByPlayerId(PLAYER_ID);
            if (debugMode)
            {
                Debug.Log($"Fetched {playerPets.Count} pets for player ID {PLAYER_ID}");
            }
            
            // Set the flag to indicate we've fetched pets
            petsFetched = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch player pets: {ex.Message}");
        }
    }
    
    // Add a public method if you need to force a refresh
    public void ForceRefreshPets()
    {
        petsFetched = false;
        FetchPlayerPets();
    }
    
    private void UpdateAllPetsInDatabase()
    {
        foreach (PlayerPet pet in playerPets)
        {
            UpdatePetInDatabase(pet);
        }
    }
    
    private void UpdatePetInDatabase(PlayerPet pet)
    {
        try
        {
            // Add debugging to see what's being sent
            Debug.Log($"Attempting to update pet {pet.playerPetID} with status: {pet.status}");
            
            // Create the HTTP request to update the pet in the database
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerPet/{pet.playerPetID}");
            request.Method = "PUT";
            request.ContentType = "application/json";
            
            // Add these lines to handle HTTPS connections if needed
            request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            
            // Convert the pet object to JSON
            string petJson = JsonConvert.SerializeObject(pet);
            
            // Log the JSON being sent for debugging
            Debug.Log($"Sending JSON to API: {petJson}");
            
            byte[] petData = Encoding.UTF8.GetBytes(petJson);
            
            // Set the request content length
            request.ContentLength = petData.Length;
            
            // Write the JSON data to the request stream
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(petData, 0, petData.Length);
            }
            
            // Get the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Check if the update was successful
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    if (debugMode)
                    {
                        Debug.Log($"Successfully updated pet {pet.playerPetID} in database");
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to update pet {pet.playerPetID} in database. Status code: {response.StatusCode}");
                }
            }
        }
        catch (WebException ex)
        {
            // Get detailed error information from the response if available
            if (ex.Response != null)
            {
                using (var errorResponse = (HttpWebResponse)ex.Response)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        Debug.LogError($"Error updating pet {pet.playerPetID} in database: {ex.Message}. Status code: {errorResponse.StatusCode}, Details: {errorText}");
                    }
                }
            }
            else
            {
                Debug.LogError($"Error updating pet {pet.playerPetID} in database: {ex.Message}. Check if the API server is running.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error updating pet {pet.playerPetID} in database: {ex.Message}");
        }
    }
    
    #endregion
    
    #region Public Interaction Methods
    
    public void FeedPet(int petId, float amount)
    {
        ModifyPetStatus(petId, amount, 0, 0);
    }
    
    public void PlayWithPet(int petId, float amount)
    {
        ModifyPetStatus(petId, 0, amount, 0);
    }
    
    public void SleepPet(int petId, float amount)
    {
        ModifyPetStatus(petId, 0, 0, amount);
    }
    
    public PlayerPet GetPetById(int petId)
    {
        return playerPets.Find(p => p.playerPetID == petId);
    }
    
    public void ModifyPetStatus(int petId, float hungerIncrease, float happinessIncrease, float energyIncrease)
    {
        PlayerPet pet = GetPetById(petId);
        
        if (pet != null)
        {
            try
            {
                string[] statuses = pet.status.Split('%');
                
                if (statuses.Length >= 3)
                {
                    float hungerValue = float.Parse(statuses[0]);
                    float happinessValue = float.Parse(statuses[1]);
                    float energyValue = float.Parse(statuses[2]);
                    
                    hungerValue = Mathf.Clamp(hungerValue + hungerIncrease, 0, 100);
                    happinessValue = Mathf.Clamp(happinessValue + happinessIncrease, 0, 100);
                    energyValue = Mathf.Clamp(energyValue + energyIncrease, 0, 100);
                    
                    pet.status = $"{hungerValue}%{happinessValue}%{energyValue}";
                    pet.lastStatusUpdate = DateTime.Now;
                    
                    // Update the database immediately for user actions
                    UpdatePetInDatabase(pet);
                    
                    // Notify listeners
                    OnPetStatusUpdated?.Invoke(pet);
                    
                    if (debugMode)
                    {
                        Debug.Log($"Modified pet {petId} status to: {pet.status}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error modifying pet status: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Pet with ID {petId} not found");
        }
    }
    
    #endregion
}