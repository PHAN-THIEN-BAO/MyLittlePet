using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanelManager : MonoBehaviour
{
    // Singleton pattern đơn giản
    public static DialoguePanelManager Instance;

    // UI Elements - chỉ giữ 3 thành phần cần thiết
    public Image dialogueImage;          // Hình ảnh
    public TMP_Text titleText;           // Dòng text tiêu đề
    public TMP_Text descriptionText;     // Dòng text mô tả
    public GameObject dialoguePanel;     // Panel chính

    // Thêm animator nếu muốn hiệu ứng đơn giản khi hiển thị/ẩn panel
    public Animator animator;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Ẩn panel khi bắt đầu
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Hiển thị dialogue panel với thông tin được cung cấp
    /// </summary>
    /// <param name="image">Hình ảnh muốn hiển thị</param>
    /// <param name="title">Tiêu đề</param>
    /// <param name="description">Nội dung mô tả</param>
    public void ShowDialogue(Sprite image, string title, string description)
    {
        // Cập nhật nội dung
        if (dialogueImage != null)
            dialogueImage.sprite = image;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        // Hiển thị panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Kích hoạt animation nếu có
        if (animator != null)
            animator.SetBool("IsOpen", true);
    }

    /// <summary>
    /// Đóng dialogue panel
    /// </summary>
    public void HideDialogue()
    {
        // Kích hoạt animation đóng nếu có
        if (animator != null)
        {
            animator.SetBool("IsOpen", false);
            // Nếu có animation, có thể thêm sự kiện animation để ẩn panel sau khi animation kết thúc
        }
        else
        {
            // Không có animation thì ẩn ngay lập tức
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }

    /// <summary>
    /// Đóng panel khi người dùng nhấn nút Close
    /// </summary>
    public void OnCloseButtonClick()
    {
        HideDialogue();
    }
}