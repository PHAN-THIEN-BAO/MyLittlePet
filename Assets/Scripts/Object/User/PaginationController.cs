using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
public class PaginationController : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform playerContainer;
    [SerializeField] GameObject notFoundPanel;
    [SerializeField] TMP_Text pageText;
    [SerializeField] Button previousButton;
    [SerializeField] Button nextButton;
    private const string USER_LIST_KEY = "SearchPlayer_UserList";
    private const string CURRENT_PAGE_KEY = "SearchPlayer_CurrentPage";
    private const string TOTAL_PAGE_KEY = "SearchPlayer_TotalPage";
    private const string PAGE_SIZE_KEY = "SearchPlayer_PageSize";
    private List<GameObject> playerClones = new List<GameObject>();
    private List<User> userList = new List<User>();
    private int currentPage = 1;
    private int totalPage = 1;
    private int pageSize = 3;
    private void Start()
    {
        Debug.Log($"PaginationController Start - Instance ID: {GetInstanceID()}");
        LoadDataFromPlayerPrefs();
        ShowCurrentPage();
    }
    private void OnEnable()
    {
        Debug.Log("PaginationController OnEnable - Refreshing data");
        RefreshFromPlayerPrefs();
    }
    private void LoadDataFromPlayerPrefs()
    {
        string rawJson = PlayerPrefs.GetString(USER_LIST_KEY, "[]");
        Debug.Log($"Raw JSON from PlayerPrefs: {rawJson}");
        userList = DeserializeUserList(rawJson);
        currentPage = PlayerPrefs.GetInt(CURRENT_PAGE_KEY, 1);
        pageSize = PlayerPrefs.GetInt(PAGE_SIZE_KEY, 3);
        int calculatedTotalPage = (userList.Count + pageSize - 1) / pageSize;
        totalPage = calculatedTotalPage > 0 ? calculatedTotalPage : 1;
        if (totalPage != PlayerPrefs.GetInt(TOTAL_PAGE_KEY, 1))
        {
            PlayerPrefs.SetInt(TOTAL_PAGE_KEY, totalPage);
            PlayerPrefs.Save();
        }
        Debug.Log($"Ðã load t? PlayerPrefs: {userList.Count} users, currentPage={currentPage}, totalPage={totalPage}");
    }
    private List<User> DeserializeUserList(string json)
    {
        List<User> result = new List<User>();
        if (string.IsNullOrEmpty(json) || json == "[]")
        {
            Debug.LogWarning("JSON empty or null");
            return result;
        }
        try
        {
            string processedJson = json.Trim();
            if (processedJson.StartsWith("[") && processedJson.EndsWith("]"))
            {
                processedJson = processedJson.Substring(1, processedJson.Length - 2);
                if (string.IsNullOrEmpty(processedJson))
                {
                    return result;
                }
                List<string> userJsons = new List<string>();
                int braceCount = 0;
                int startIndex = 0;
                for (int i = 0; i < processedJson.Length; i++)
                {
                    char c = processedJson[i];
                    if (c == '{') braceCount++;
                    else if (c == '}')
                    {
                        braceCount--;
                        if (braceCount == 0)
                        {
                            string userJson = processedJson.Substring(startIndex, i - startIndex + 1);
                            userJsons.Add(userJson);
                            startIndex = i + 1;
                            while (startIndex < processedJson.Length &&
                                  (processedJson[startIndex] == ',' || char.IsWhiteSpace(processedJson[startIndex])))
                            {
                                startIndex++;
                            }
                            i = startIndex - 1;
                        }
                    }
                }
                foreach (string userJson in userJsons)
                {
                    try
                    {
                        User user = JsonUtility.FromJson<User>(userJson);
                        if (user != null)
                        {
                            result.Add(user);
                            Debug.Log($"Parsed user: {user.userName}, ID: {user.id}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"L?i parsing user JSON: {e.Message}, JSON: {userJson}");
                    }
                }
            }
            else
            {
                Debug.LogError($"Format JSON không dúng, không ph?i m?ng: {json}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"L?i x? lý JSON: {e.Message}, JSON: {json}");
        }
        Debug.Log($"Ðã parse du?c {result.Count} users t? JSON");
        return result;
    }
    private void ShowCurrentPage()
    {
        ClearPlayerClones();
        if (userList == null || userList.Count == 0)
        {
            if (notFoundPanel != null)
                notFoundPanel.SetActive(true);
            if (playerPrefab != null)
                playerPrefab.SetActive(false);
            if (pageText != null)
                pageText.text = "Page 0/0";
            if (previousButton != null)
                previousButton.gameObject.SetActive(false);
            if (nextButton != null)
                nextButton.gameObject.SetActive(false);
            return;
        }
        if (notFoundPanel != null)
            notFoundPanel.SetActive(false);
        if (playerPrefab != null)
            playerPrefab.SetActive(false);
        if (currentPage < 1) currentPage = 1;
        if (currentPage > totalPage) currentPage = totalPage;
        int startIdx = (currentPage - 1) * pageSize;
        int endIdx = Mathf.Min(startIdx + pageSize, userList.Count);
        Debug.Log($"ShowCurrentPage: T?o player clones t? index {startIdx} d?n {endIdx - 1}");
        for (int i = startIdx; i < endIdx; i++)
        {
            if (i >= 0 && i < userList.Count)
            {
                GameObject playerClone = Instantiate(playerPrefab, playerContainer);
                playerClone.SetActive(true);
                playerClones.Add(playerClone);
                UpdatePlayerInfo(playerClone, userList[i]);
            }
        }
        if (pageText != null)
            pageText.text = $"Page {currentPage}/{totalPage}";
        if (previousButton != null)
            previousButton.gameObject.SetActive(currentPage > 1);
        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPage < totalPage);
        PlayerPrefs.SetInt(CURRENT_PAGE_KEY, currentPage);
        PlayerPrefs.Save();
        Debug.Log($"Ðã hi?n th? trang {currentPage}/{totalPage}, t?o {playerClones.Count} clones");
    }
    public void OnNextPage()
    {
        totalPage = Mathf.Max(1, (userList.Count + pageSize - 1) / pageSize);
        Debug.Log($"Next Page Clicked. currentPage={currentPage}, totalPage={totalPage}, userList.Count={userList.Count}");
        if (currentPage < totalPage)
        {
            currentPage++;
            ShowCurrentPage();
        }
        else
        {
            Debug.LogWarning($"Không th? chuy?n d?n trang ti?p theo: currentPage={currentPage}, totalPage={totalPage}");
            RefreshFromPlayerPrefs();
        }
    }
    public void OnPreviousPage()
    {
        Debug.Log($"Previous Page Clicked. currentPage={currentPage}, totalPage={totalPage}, userList.Count={userList.Count}");
        if (currentPage > 1)
        {
            currentPage--;
            ShowCurrentPage();
        }
        else
        {
            Debug.LogWarning("Ðã ? trang d?u tiên, không th? quay l?i trang tru?c.");
        }
    }
    private void UpdatePlayerInfo(GameObject playerObject, User user)
    {
        if (playerObject == null || user == null)
        {
            Debug.LogError("playerObject ho?c user là null");
            return;
        }
        try
        {
            TMP_Text nameText = playerObject.transform.Find("Name_Player")?.GetComponent<TMP_Text>();
            if (nameText != null)
                nameText.text = user.userName;
            else
                Debug.LogWarning("Không tìm th?y Name_Player trên playerObject");
            TMP_Text levelText = playerObject.transform.Find("Level")?.GetComponent<TMP_Text>();
            if (levelText != null)
                levelText.text = "Level: " + user.level.ToString();
            else
                Debug.LogWarning("Không tìm th?y Level trên playerObject");
            TMP_Text idText = playerObject.transform.Find("Id")?.GetComponent<TMP_Text>();
            if (idText != null)
                idText.text = "ID: " + user.id.ToString();
            else
                Debug.LogWarning("Không tìm th?y Id trên playerObject");
        }
        catch (Exception e)
        {
            Debug.LogError($"L?i khi c?p nh?t thông tin ngu?i choi: {e.Message}");
        }
    }
    private void ClearPlayerClones()
    {
        foreach (GameObject clone in playerClones)
        {
            if (clone != null)
                Destroy(clone);
        }
        playerClones.Clear();
        Debug.Log("Ðã xóa t?t c? player clones");
    }
    public void RefreshFromPlayerPrefs()
    {
        Debug.Log("RefreshFromPlayerPrefs du?c g?i");
        LoadDataFromPlayerPrefs();
        ShowCurrentPage();
    }
    public void DebugCurrentState()
    {
        string rawJson = PlayerPrefs.GetString(USER_LIST_KEY, "[]");
        Debug.Log($"===== DEBUG PAGINATION =====");
        Debug.Log($"Instance ID: {GetInstanceID()}");
        Debug.Log($"userList.Count: {userList?.Count ?? 0}");
        Debug.Log($"currentPage: {currentPage}");
        Debug.Log($"totalPage: {totalPage}");
        Debug.Log($"pageSize: {pageSize}");
        Debug.Log($"Raw JSON length: {rawJson.Length}");
        Debug.Log($"Raw JSON preview: {(rawJson.Length > 100 ? rawJson.Substring(0, 100) + "..." : rawJson)}");
        Debug.Log($"===========================");
    }
}