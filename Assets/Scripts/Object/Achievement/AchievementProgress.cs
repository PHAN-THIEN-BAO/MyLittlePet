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
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("Failed to load player information.");
            return;
        }

        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(user.id);
        int currentPets = playerPets != null ? playerPets.Count : 0;

        if (AchievementPet != null)
        {
            for (int i = 0; i < AchievementPet.Count; i++)
            {
                TMP_Text progressText = AchievementPet[i].transform.Find("Progress_Detail").GetComponent<TMP_Text>();
                if (progressText == null) continue;

                int targetValue = 5;

                if (!string.IsNullOrEmpty(progressText.text))
                {
                    string[] parts = progressText.text.Split('/');
                    if (parts.Length == 2)
                    {
                        progressText.text = $"{currentPets}/{parts[1]}";
                    }
                    else
                    {
                        targetValue = ExtractLastNumber(progressText.text);
                        if (targetValue == 0) targetValue = 5;
                        progressText.text = $"{currentPets}/{targetValue}";
                    }
                }
                else
                {
                    progressText.text = $"{currentPets}/{targetValue}";
                }
            }
        }

        int currentCoins = user.coin;

        if (AchievementCoin != null)
        {
            for (int i = 0; i < AchievementCoin.Count; i++)
            {
                TMP_Text progressText = AchievementCoin[i].transform.Find("Progress_Detail").GetComponent<TMP_Text>();
                if (progressText == null) continue;

                if (!string.IsNullOrEmpty(progressText.text))
                {
                    string[] parts = progressText.text.Split('/');
                    string currentDisplay;
                    if (parts.Length == 2)
                    {
                        currentDisplay = FormatNumberWithSuffix(currentCoins);

                        progressText.text = $"{currentDisplay}/{parts[1]}";
                    }
                    else
                    {
                        int targetValue = ExtractLastNumber(progressText.text);
                        if (targetValue == 0) targetValue = 1000;

                        currentDisplay = FormatNumberWithSuffix(currentCoins);
                        string targetDisplay = FormatNumberWithSuffix(targetValue);
                        progressText.text = $"{currentDisplay}/{targetDisplay}";
                    }
                }
                else
                {
                    string currentDisplay = FormatNumberWithSuffix(currentCoins);
                    progressText.text = $"{currentDisplay}/1k";
                }
            }
        }

        CheckCompleted();
    }

    public void CheckCompleted()
    {
        if (AchievementPet != null)
        {
            for (int i = 0; i < AchievementPet.Count; i++)
            {
                GameObject achievement = AchievementPet[i];

                GameObject collectedButton = achievement.transform.Find("Collected_Button")?.gameObject;
                if (collectedButton != null && collectedButton.activeSelf)
                {
                    continue;
                }

                TMP_Text progressText = achievement.transform.Find("Progress_Detail")?.GetComponent<TMP_Text>();
                if (progressText == null) continue;

                int currentValue = ExtractFirstNumber(progressText.text);
                int targetValue = ExtractLastNumber(progressText.text);

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

        if (AchievementCoin != null)
        {
            for (int i = 0; i < AchievementCoin.Count; i++)
            {
                GameObject achievement = AchievementCoin[i];

                GameObject collectedButton = achievement.transform.Find("Collected_Button")?.gameObject;
                if (collectedButton != null && collectedButton.activeSelf)
                {
                    continue;
                }

                TMP_Text progressText = achievement.transform.Find("Progress_Detail")?.GetComponent<TMP_Text>();
                Debug.Log($"Progress Text: {progressText?.text}");
                if (progressText == null) continue;

                int currentValue = ExtractFirstNumber(progressText.text);
                int targetValue = ExtractLastNumber(progressText.text);
                //Debug.Log($"Current Value: {currentValue},  /  Target Value: {targetValue}");

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

    private string FormatNumberWithSuffix(int number)
    {
        if (number >= 1000)
        {
            float thousands = number / 1000f;

            if (number % 1000 == 0)
                return $"{thousands:0}k";

            return $"{thousands:0.0}k";
        }

        return number.ToString();
    }

    private int ExtractFirstNumber(string progressString)
    {
        if (string.IsNullOrEmpty(progressString))
            return 0;

        string[] parts = progressString.Split('/');

        if (parts.Length != 2)
        {
            return 0;
        }

        Debug.Log($"ExtractFirstNumber - parts[0]: {parts[0]}");

        string firstPart = parts[0].Trim();

        bool hasKSuffix = firstPart.EndsWith("k") || firstPart.EndsWith("K");

        if (hasKSuffix)
        {
            firstPart = firstPart.Substring(0, firstPart.Length - 1);
        }

        if (float.TryParse(firstPart, out float firstNumber))
        {
            if (hasKSuffix)
            {
                firstNumber *= 1000;
            }
            return (int)firstNumber;
        }

        return 0;
    }

    private int ExtractLastNumber(string progressString)
    {
        if (string.IsNullOrEmpty(progressString))
            return 0;

        string[] parts = progressString.Split('/');

        if (parts.Length != 2)
            return 0;

        string lastPart = parts[1].Trim();

        bool hasKSuffix = lastPart.EndsWith("k") || lastPart.EndsWith("K");

        if (hasKSuffix)
        {
            lastPart = lastPart.Substring(0, lastPart.Length - 1);
        }

        if (float.TryParse(lastPart, out float lastNumber))
        {
            if (hasKSuffix)
            {
                lastNumber *= 1000;
            }
            return (int)lastNumber;
        }

        return 0;
    }

    public int getNumberOfPlayerPets()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(user.id);
        return playerPets != null ? playerPets.Count : 0;
    }

    public int getNumberOfPlayerCoins()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        return user.coin;
    }





}