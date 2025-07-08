using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class AchievementProgress : MonoBehaviour
{
    [SerializeField] public List<GameObject> AchievementPet;
    [SerializeField] public List<GameObject> AchievementCoin;

    public void SetProgess()
    {
        // Get user information
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("Failed to load player information.");
            return;
        }

        // === Handle Pet Progress ===
        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(user.id);
        int currentPets = playerPets != null ? playerPets.Count : 0;

        // Update each pet progress text (if any)
        if (AchievementPet != null)
        {
            for (int i = 0; i < AchievementPet.Count; i++)
            {
                TMP_Text progressText = AchievementPet[i].transform.Find("Progress_Detail").GetComponent<TMP_Text>();
                if (progressText == null) continue;

                int targetValue = 5; // Default

                // Keep the existing target value if text exists
                if (!string.IsNullOrEmpty(progressText.text))
                {
                    string[] parts = progressText.text.Split('/');
                    if (parts.Length == 2)
                    {
                        // Preserve the original target part exactly as it is
                        progressText.text = $"{currentPets}/{parts[1]}";
                    }
                    else
                    {
                        // Fallback if format is unexpected
                        targetValue = ExtractLastNumber(progressText.text);
                        if (targetValue == 0) targetValue = 5;
                        progressText.text = $"{currentPets}/{targetValue}";
                    }
                }
                else
                {
                    // Only reaches here for new/empty elements or incorrect format
                    progressText.text = $"{currentPets}/{targetValue}";
                }
            }
        }

        // === Handle Coin Progress ===
        int currentCoins = user.coin;

        if (AchievementCoin != null)
        {
            for (int i = 0; i < AchievementCoin.Count; i++)
            {
                TMP_Text progressText = AchievementCoin[i].transform.Find("Progress_Detail").GetComponent<TMP_Text>();
                if (progressText == null) continue;

                // Keep the existing target value if text exists
                if (!string.IsNullOrEmpty(progressText.text))
                {
                    string[] parts = progressText.text.Split('/');
                    string currentDisplay;
                    if (parts.Length == 2)
                    {
                        // Format current coins with suffix as needed
                        currentDisplay = FormatNumberWithSuffix(currentCoins);

                        // Preserve the original target part exactly as it is
                        progressText.text = $"{currentDisplay}/{parts[1]}";
                    }
                    else
                    {
                        // Fallback handling for unexpected format
                        int targetValue = ExtractLastNumber(progressText.text);
                        if (targetValue == 0) targetValue = 1000;

                        currentDisplay = FormatNumberWithSuffix(currentCoins);
                        string targetDisplay = FormatNumberWithSuffix(targetValue);
                        progressText.text = $"{currentDisplay}/{targetDisplay}";
                    }
                }
                else
                {
                    // Default for new/empty elements
                    string currentDisplay = FormatNumberWithSuffix(currentCoins);
                    progressText.text = $"{currentDisplay}/1k";
                }
            }
        }

        // Check if achievements are completed after updating progress
        CheckCompleted();
    }

    /// <summary>
    /// Checks if achievements are completed and activates the Ready_Collected_Button accordingly
    /// </summary>
    public void CheckCompleted()
    {
        // Check Pet achievements
        if (AchievementPet != null)
        {
            for (int i = 0; i < AchievementPet.Count; i++)
            {
                GameObject achievement = AchievementPet[i];

                // Skip if this achievement has already been collected
                GameObject collectedButton = achievement.transform.Find("Collected_Button")?.gameObject;
                if (collectedButton != null && collectedButton.activeSelf)
                {
                    continue;
                }

                // Get progress text
                TMP_Text progressText = achievement.transform.Find("Progress_Detail")?.GetComponent<TMP_Text>();
                if (progressText == null) continue;

                // Check if current progress is greater than or equal to target
                int currentValue = ExtractFirstNumber(progressText.text);
                int targetValue = ExtractLastNumber(progressText.text);

                // If completed, show the Ready_Collected_Button
                GameObject readyButton = achievement.transform.Find("Ready_Collected_Button")?.gameObject;
                GameObject notCollectedButton = achievement.transform.Find("Not_Collected_Button")?.gameObject;
                if (readyButton != null)
                {
                    readyButton.SetActive(currentValue >= targetValue);
                    notCollectedButton.SetActive(!(currentValue >= targetValue));
                    TMP_Text achievementId = achievement.transform.Find("Achievement_Id")?.GetComponent<TMP_Text>();
                    if (currentValue >= targetValue && APIPlayerAchievement.AddAchievement(int.Parse(achievementId.text)))
                    {
                        Debug.Log($"Achievement added successfully.");
                    }
                }
            }
        }

        // Check Coin achievements
        if (AchievementCoin != null)
        {
            for (int i = 0; i < AchievementCoin.Count; i++)
            {
                GameObject achievement = AchievementCoin[i];

                // Skip if this achievement has already been collected
                GameObject collectedButton = achievement.transform.Find("Collected_Button")?.gameObject;
                if (collectedButton != null && collectedButton.activeSelf)
                {
                    continue;
                }

                // Get progress text
                TMP_Text progressText = achievement.transform.Find("Progress_Detail")?.GetComponent<TMP_Text>();
                if (progressText == null) continue;

                // Check if current progress is greater than or equal to target
                int currentValue = ExtractFirstNumber(progressText.text);
                int targetValue = ExtractLastNumber(progressText.text);

                // If completed, show the Ready_Collected_Button
                GameObject readyButton = achievement.transform.Find("Ready_Collected_Button")?.gameObject;
                GameObject notCollectedButton = achievement.transform.Find("Not_Collected_Button")?.gameObject;
                if (readyButton != null)
                {
                    readyButton.SetActive(currentValue >= targetValue);
                    notCollectedButton.SetActive(!(currentValue >= targetValue));
                    TMP_Text achievementId = achievement.transform.Find("Achievement_Id")?.GetComponent<TMP_Text>();
                    if (currentValue >= targetValue && APIPlayerAchievement.AddAchievement(int.Parse(achievementId.text)))
                    {
                        Debug.Log($"Achievement added successfully.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Formats a number with 'k' suffix for thousands
    /// </summary>
    /// <param name="number">Number to format</param>
    /// <returns>Formatted string with 'k' suffix if applicable</returns>
    private string FormatNumberWithSuffix(int number)
    {
        if (number >= 1000)
        {
            float thousands = number / 1000f;

            // if the number is exactly a multiple of 1000, return without decimal places
            if (number % 1000 == 0)
                return $"{thousands:0}k";

            // otherwise, return with one decimal place
            return $"{thousands:0.0}k";
        }

        // if the number is less than 1000, return it as is
        return number.ToString();
    }

    /// <summary>
    /// Extracts the first number from a string formatted as "number/number" or "number/numberk"
    /// </summary>
    /// <param name="progressString">String in format "X/Y" where X and Y are numbers, and Y may include 'k' suffix</param>
    /// <returns>The first number (X) as an integer, or 0 if the format is invalid</returns>
    private int ExtractFirstNumber(string progressString)
    {
        if (string.IsNullOrEmpty(progressString))
            return 0;

        // Split the string by '/' character
        string[] parts = progressString.Split('/');

        // Check if the format is correct
        if (parts.Length != 2)
            return 0;

        // Try to parse the first part as an integer
        if (float.TryParse(parts[0], out float firstNumber))
        {
            // Check if there's a 'k' suffix
            if (parts[0].Contains("k") || parts[0].Contains("K"))
            {
                firstNumber *= 1000; // Convert thousands to actual number
            }
            return (int)firstNumber;
        }

        // Return 0 if parsing failed
        return 0;
    }

    /// <summary>
    /// Extracts the second number from a string formatted as "number/number" or "number/numberk"
    /// </summary>
    /// <param name="progressString">String in format "X/Y" where X and Y are numbers, and Y may include 'k' suffix</param>
    /// <returns>The second number (Y) as an integer, or 0 if the format is invalid</returns>
    private int ExtractLastNumber(string progressString)
    {
        if (string.IsNullOrEmpty(progressString))
            return 0;

        // Split the string by '/' character
        string[] parts = progressString.Split('/');

        // Check if the format is correct
        if (parts.Length != 2)
            return 0;

        string lastPart = parts[1].Trim();

        // Check if there's a 'k' suffix
        bool hasKSuffix = lastPart.EndsWith("k") || lastPart.EndsWith("K");

        if (hasKSuffix)
        {
            // Remove the 'k' suffix
            lastPart = lastPart.Substring(0, lastPart.Length - 1);
        }

        // Try to parse the second part as a float (to handle decimal values like 2.5)
        if (float.TryParse(lastPart, out float lastNumber))
        {
            // If it had a 'k' suffix, multiply by 1000
            if (hasKSuffix)
            {
                lastNumber *= 1000;
            }
            return (int)lastNumber;
        }

        // Return 0 if parsing failed
        return 0;
    }

    /// <summary>
    /// Returns the number of pets owned by the player
    /// </summary>
    public int getNumberOfPlayerPets()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(user.id);
        return playerPets != null ? playerPets.Count : 0;
    }

    /// <summary>
    /// Returns the number of coins owned by the player
    /// </summary>
    public int getNumberOfPlayerCoins()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        return user.coin;
    }





}
