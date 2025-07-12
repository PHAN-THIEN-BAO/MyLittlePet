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

    // Key constants cho PlayerPrefs (phải khớp với SearchPlayer.cs)
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
        // Load dữ liệu từ PlayerPrefs khi khởi động
        LoadDataFromPlayerPrefs();
        // Hiển thị trang hiện tại
        ShowCurrentPage();
    }

    // Đảm bảo refresh dữ liệu mỗi khi script được kích hoạt
    private void OnEnable()
    {
        Debug.Log("PaginationController OnEnable - Refreshing data");
        RefreshFromPlayerPrefs();
    }

    // Load dữ liệu từ PlayerPrefs
    private void LoadDataFromPlayerPrefs()
    {
        string rawJson = PlayerPrefs.GetString(USER_LIST_KEY, "[]");
        Debug.Log($"Raw JSON from PlayerPrefs: {rawJson}");

        userList = DeserializeUserList(rawJson);
        currentPage = PlayerPrefs.GetInt(CURRENT_PAGE_KEY, 1);
        pageSize = PlayerPrefs.GetInt(PAGE_SIZE_KEY, 3);

        // Luôn tính lại totalPage dựa trên số lượng user thực tế
        int calculatedTotalPage = (userList.Count + pageSize - 1) / pageSize;
        totalPage = calculatedTotalPage > 0 ? calculatedTotalPage : 1;

        // Cập nhật lại PlayerPrefs nếu totalPage tính toán khác với giá trị lưu trữ
        if (totalPage != PlayerPrefs.GetInt(TOTAL_PAGE_KEY, 1))
        {
            PlayerPrefs.SetInt(TOTAL_PAGE_KEY, totalPage);
            PlayerPrefs.Save();
        }

        Debug.Log($"Đã load từ PlayerPrefs: {userList.Count} users, currentPage={currentPage}, totalPage={totalPage}");
    }

    // Phương thức cải tiến để chuyển chuỗi JSON thành List<User>
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
            // Cách xử lý JSON đáng tin cậy hơn
            // Tách chuỗi JSON thành từng đối tượng User riêng biệt
            string processedJson = json.Trim();

            // Kiểm tra xem có phải là mảng JSON không
            if (processedJson.StartsWith("[") && processedJson.EndsWith("]"))
            {
                // Bỏ dấu ngoặc vuông mở đầu và kết thúc
                processedJson = processedJson.Substring(1, processedJson.Length - 2);

                // Nếu chuỗi rỗng sau khi xử lý, trả về list trống
                if (string.IsNullOrEmpty(processedJson))
                {
                    return result;
                }

                // Tách thành từng đối tượng JSON riêng lẻ
                // Đây là một cách xử lý đơn giản, cần cải thiện để xử lý JSON phức tạp hơn
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
                            // Tìm thấy một đối tượng JSON hoàn chỉnh
                            string userJson = processedJson.Substring(startIndex, i - startIndex + 1);
                            userJsons.Add(userJson);
                            startIndex = i + 1;

                            // Bỏ qua dấu phẩy ngăn cách
                            while (startIndex < processedJson.Length &&
                                  (processedJson[startIndex] == ',' || char.IsWhiteSpace(processedJson[startIndex])))
                            {
                                startIndex++;
                            }
                            i = startIndex - 1; // Điều chỉnh index cho vòng lặp tiếp theo
                        }
                    }
                }

                // Parse từng đối tượng JSON thành User
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
                        Debug.LogError($"Lỗi parsing user JSON: {e.Message}, JSON: {userJson}");
                    }
                }
            }
            else
            {
                Debug.LogError($"Format JSON không đúng, không phải mảng: {json}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi xử lý JSON: {e.Message}, JSON: {json}");
        }

        Debug.Log($"Đã parse được {result.Count} users từ JSON");
        return result;
    }

    // Hiển thị trang hiện tại
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

        // Kiểm tra và điều chỉnh currentPage nếu cần
        if (currentPage < 1) currentPage = 1;
        if (currentPage > totalPage) currentPage = totalPage;

        int startIdx = (currentPage - 1) * pageSize;
        int endIdx = Mathf.Min(startIdx + pageSize, userList.Count);

        Debug.Log($"ShowCurrentPage: Tạo player clones từ index {startIdx} đến {endIdx - 1}");

        for (int i = startIdx; i < endIdx; i++)
        {
            if (i >= 0 && i < userList.Count) // Kiểm tra index hợp lệ
            {
                GameObject playerClone = Instantiate(playerPrefab, playerContainer);
                playerClone.SetActive(true);
                playerClones.Add(playerClone);
                UpdatePlayerInfo(playerClone, userList[i]);
            }
        }

        if (pageText != null)
            pageText.text = $"Page {currentPage}/{totalPage}";

        // Hiển thị nút Previous chỉ khi có thể quay lại trang trước
        if (previousButton != null)
            previousButton.gameObject.SetActive(currentPage > 1);

        // Hiển thị nút Next chỉ khi có thể đến trang tiếp theo
        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPage < totalPage);

        // Lưu lại currentPage vào PlayerPrefs
        PlayerPrefs.SetInt(CURRENT_PAGE_KEY, currentPage);
        PlayerPrefs.Save();

        Debug.Log($"Đã hiển thị trang {currentPage}/{totalPage}, tạo {playerClones.Count} clones");
    }

    // Nút Next Page
    public void OnNextPage()
    {
        // Tính lại totalPage dựa trên số lượng user hiện tại
        totalPage = Mathf.Max(1, (userList.Count + pageSize - 1) / pageSize);

        Debug.Log($"Next Page Clicked. currentPage={currentPage}, totalPage={totalPage}, userList.Count={userList.Count}");

        if (currentPage < totalPage)
        {
            currentPage++;
            ShowCurrentPage();
        }
        else
        {
            Debug.LogWarning($"Không thể chuyển đến trang tiếp theo: currentPage={currentPage}, totalPage={totalPage}");

            // Refresh dữ liệu từ PlayerPrefs nếu không chuyển trang được
            RefreshFromPlayerPrefs();
        }
    }

    // Nút Previous Page
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
            Debug.LogWarning("Đã ở trang đầu tiên, không thể quay lại trang trước.");
        }
    }

    // Cập nhật thông tin người chơi trên UI
    private void UpdatePlayerInfo(GameObject playerObject, User user)
    {
        if (playerObject == null || user == null)
        {
            Debug.LogError("playerObject hoặc user là null");
            return;
        }

        try
        {
            TMP_Text nameText = playerObject.transform.Find("Name_Player")?.GetComponent<TMP_Text>();
            if (nameText != null)
                nameText.text = user.userName;
            else
                Debug.LogWarning("Không tìm thấy Name_Player trên playerObject");

            TMP_Text levelText = playerObject.transform.Find("Level")?.GetComponent<TMP_Text>();
            if (levelText != null)
                levelText.text = "Level: " + user.level.ToString();
            else
                Debug.LogWarning("Không tìm thấy Level trên playerObject");

            TMP_Text idText = playerObject.transform.Find("Id")?.GetComponent<TMP_Text>();
            if (idText != null)
                idText.text = "ID: " + user.id.ToString();
            else
                Debug.LogWarning("Không tìm thấy Id trên playerObject");
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi khi cập nhật thông tin người chơi: {e.Message}");
        }
    }

    // Xóa bỏ các clone hiện tại
    private void ClearPlayerClones()
    {
        foreach (GameObject clone in playerClones)
        {
            if (clone != null)
                Destroy(clone);
        }
        playerClones.Clear();
        Debug.Log("Đã xóa tất cả player clones");
    }

    // Phương thức refresh dữ liệu từ PlayerPrefs (có thể gọi từ bên ngoài)
    public void RefreshFromPlayerPrefs()
    {
        Debug.Log("RefreshFromPlayerPrefs được gọi");
        LoadDataFromPlayerPrefs();
        ShowCurrentPage();
    }

    // Phương thức debug để kiểm tra trạng thái hiện tại
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
