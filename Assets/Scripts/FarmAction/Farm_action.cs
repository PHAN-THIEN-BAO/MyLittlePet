using UnityEngine;

public class Farm_action : MonoBehaviour
{
    [Header("Cài đặt Animation")]
    [Tooltip("Animator điều khiển animation cây trồng")]
    public Animator plantAnimator;

    [Tooltip("Tên của trigger trong Animator")]
    public string growthTriggerName = "plant";

    [Header("Cài đặt Tương tác")]
    [Tooltip("Phím để tương tác với đất")]
    public KeyCode interactKey = KeyCode.E;

    private bool canInteract = false;
    private bool hasGrown = false;

    void Start()
    {
        // Kiểm tra Animator
        if (plantAnimator == null)
        {
            // Tìm Animator trên cùng GameObject hoặc con của nó
            plantAnimator = GetComponent<Animator>();
            if (plantAnimator == null)
            {
                plantAnimator = GetComponentInChildren<Animator>();
            }

            if (plantAnimator == null)
            {
                Debug.LogWarning("Chưa gán Animator cho cây! Hãy gán trong Inspector.");
            }
        }
    }

    void Update()
    {
        // Kiểm tra khi người chơi nhấn phím tương tác
        if (canInteract && Input.GetKeyDown(interactKey) && !hasGrown)
        {
            GrowPlant();
            hasGrown = true;
        }
    }

    // Thêm từ khóa public cho hàm để nó xuất hiện trong On Click
    public void GrowPlant()
    {
        if (plantAnimator != null)
        {
            plantAnimator.SetTrigger(growthTriggerName);
            Debug.Log("Đã kích hoạt animation trồng cây!");
        }
    }

    // Phát hiện khi người chơi đến gần vùng đất
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            Debug.Log("Bạn có thể tương tác với mảnh đất này. Nhấn phím " + interactKey + " để trồng cây.");
        }
    }

    // Phát hiện khi người chơi rời khỏi vùng đất
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    // Hàm này cho phép tương tác trực tiếp qua nhấp chuột
    void OnMouseDown()
    {
        if (!hasGrown)
        {
            GrowPlant();
            hasGrown = true;
        }
    }

    // Thêm hàm public để reset lại trạng thái của cây
    public void ResetPlant()
    {
        hasGrown = false;
        Debug.Log("Cây đã được reset, có thể trồng lại!");
    }
}