//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//[System.Serializable]
//public class DialougeCharacter
//{
//    public string name;
//    public Sprite icon;
//}

//[System.Serializable]
//public class DialogueLine
//{
//    public DialougeCharacter character;
//    [TextArea(3, 10)]
//    public string line;
//}

//[System.Serializable]
//public class Dialogue
//{
//    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
//}

//public class DialougeTrigger : MonoBehaviour
//{
//    // Dialogue data
//    public Dialogue dialogue;

//    // UI References
//    public GameObject dialoguePanel;

//    [Header("Chỉ Dẫn")]
//    [Tooltip("Biểu tượng mũi tên chỉ dẫn trên thế giới")]
//    public GameObject indicatorPrefab;
//    [Tooltip("Vị trí cố định của biểu tượng (tương đối với GameObject này)")]
//    public Vector3 indicatorOffset = new Vector3(0, 1.5f, 0);
//    [Tooltip("Tốc độ nhịp nhảy của biểu tượng")]
//    public float bobSpeed = 2f;
//    [Tooltip("Độ cao nhịp nhảy của biểu tượng")]
//    public float bobHeight = 0.2f;

//    [Header("Hiệu ứng xoay")]
//    [Tooltip("Tốc độ quay của biểu tượng (độ/giây)")]
//    public float rotationSpeed = 45f;
//    [Tooltip("Có xoay liên tục không")]
//    public bool continuousRotation = true;
//    [Tooltip("Trục xoay (X, Y hoặc Z)")]
//    public RotationAxis rotationAxis = RotationAxis.Z;
//    [Tooltip("Góc xoay tối đa (nếu không xoay liên tục)")]
//    public float maxRotationAngle = 45f;

//    [Header("Cài đặt khác")]
//    [Tooltip("Hiển thị biểu tượng cùng lúc với dialogue")]
//    public bool showIndicatorWithDialogue = true;

//    // Trigger settings
//    [Tooltip("Nếu true, dialogue sẽ tự động trigger khi va chạm với Player")]
//    public bool triggerOnContact = true;

//    [Tooltip("Nếu true, dialogue chỉ trigger một lần")]
//    public bool triggerOnce = false;

//    [Tooltip("Nếu true, dialogue chỉ trigger một lần và trạng thái sẽ được lưu giữa các scene")]
//    public bool persistAcrossScenes = false;

//    [Tooltip("ID duy nhất cho dialogue này, cần thiết khi persistAcrossScenes = true")]
//    public string dialogueId;

//    // Events
//    [Tooltip("Event gọi khi dialogue bắt đầu")]
//    public UnityEvent onDialogueStart;

//    [Tooltip("Event gọi khi dialogue kết thúc")]
//    public UnityEvent onDialogueEnd;

//    // Enum cho trục xoay
//    public enum RotationAxis { X, Y, Z }

//    private bool hasTriggered = false;
//    private string prefsKey;
//    private GameObject indicatorInstance;
//    private Vector3 initialIndicatorPosition;
//    private float bobTimer = 0f;
//    private float rotationTimer = 0f;
//    private bool isPlayerInTrigger = false;
//    private Quaternion initialRotation;

//    private void Awake()
//    {
//        // Tạo prefsKey khi component được khởi tạo
//        if (persistAcrossScenes)
//        {
//            // Nếu dialogueId không được cung cấp, tự động tạo ID từ tên GameObject và scene
//            if (string.IsNullOrEmpty(dialogueId))
//            {
//                dialogueId = $"{gameObject.scene.name}_{gameObject.name}_{transform.position}";
//                Debug.LogWarning($"DialogueId không được cung cấp cho {gameObject.name}. Tự động tạo ID: {dialogueId}");
//            }

//            prefsKey = $"DialogueTrigger_{dialogueId}";

//            // Kiểm tra xem dialogue này đã từng được trigger chưa
//            hasTriggered = PlayerPrefs.GetInt(prefsKey, 0) == 1;
//        }

//        // Chỉ hiển thị biểu tượng chỉ dẫn từ đầu nếu không cùng hiển thị với dialogue
//        if (!showIndicatorWithDialogue && indicatorPrefab != null && !hasTriggered)
//        {
//            ShowWorldIndicator();
//        }
//    }

//    private void OnEnable()
//    {
//        // Đăng ký lắng nghe sự kiện khi dialogue kết thúc
//        DialogueManager.OnDialogueEnd += OnDialogueEnded;

//        // Chỉ hiển thị biểu tượng chỉ dẫn từ đầu nếu không cùng hiển thị với dialogue
//        if (!showIndicatorWithDialogue && indicatorPrefab != null && !hasTriggered)
//        {
//            ShowWorldIndicator();
//        }
//    }

//    private void OnDisable()
//    {
//        // Hủy đăng ký để tránh memory leak
//        DialogueManager.OnDialogueEnd -= OnDialogueEnded;

//        // Ẩn biểu tượng chỉ dẫn nếu còn tồn tại
//        DestroyIndicator();
//    }

//    private void Update()
//    {
//        // Cập nhật hiệu ứng của biểu tượng chỉ dẫn
//        AnimateWorldIndicator();
//    }

//    // Phương thức được gọi khi dialogue kết thúc
//    void OnDialogueEnded()
//    {
//        // Ẩn panel nếu nó tồn tại
//        if (dialoguePanel != null)
//            dialoguePanel.SetActive(false);

//        // Gọi event khi dialogue kết thúc
//        onDialogueEnd?.Invoke();

//        // Ẩn biểu tượng khi dialogue kết thúc nếu hiển thị cùng dialogue
//        if (showIndicatorWithDialogue)
//        {
//            DestroyIndicator();
//        }
//        else if (!hasTriggered)
//        {
//            // Hiển thị lại biểu tượng sau khi dialogue kết thúc nếu không trigger một lần
//            ShowWorldIndicator();
//        }

//        // Kiểm tra xem người chơi còn trong vùng trigger không để hiển thị lại nếu cần
//        if (isPlayerInTrigger && !hasTriggered)
//        {
//            // Chờ một chút rồi hiển thị lại nếu người chơi vẫn ở trong vùng trigger
//            Invoke("ShowIndicatorIfPlayerInRange", 0.5f);
//        }
//    }

//    // Kiểm tra và hiển thị lại indicator nếu cần
//    private void ShowIndicatorIfPlayerInRange()
//    {
//        if (isPlayerInTrigger && !hasTriggered)
//        {
//            if (showIndicatorWithDialogue)
//            {
//                ShowWorldIndicator();
//            }
//        }
//    }

//    // Phương thức công khai để kích hoạt dialogue
//    public void TriggerDialogue()
//    {
//        // Kiểm tra nếu chỉ trigger một lần
//        if ((triggerOnce || persistAcrossScenes) && hasTriggered)
//            return;

//        hasTriggered = true;

//        // Lưu trạng thái nếu persistAcrossScenes được bật
//        if (persistAcrossScenes)
//        {
//            PlayerPrefs.SetInt(prefsKey, 1);
//            PlayerPrefs.Save();
//        }

//        // Hiển thị panel nếu nó tồn tại
//        if (dialoguePanel != null)
//            dialoguePanel.SetActive(true);

//        // Kiểm tra DialogueManager tồn tại
//        if (DialogueManager.Instance != null)
//        {
//            // Hiển thị biểu tượng chỉ dẫn cùng với dialogue nếu cấu hình để hiển thị cùng nhau
//            if (showIndicatorWithDialogue && indicatorPrefab != null)
//            {
//                ShowWorldIndicator();
//            }
//            else if (!showIndicatorWithDialogue)
//            {
//                // Nếu không hiển thị cùng, thì ẩn biểu tượng đi
//                DestroyIndicator();
//            }

//            DialogueManager.Instance.StartDialog(dialogue);

//            // Gọi event khi dialogue bắt đầu
//            onDialogueStart?.Invoke();
//        }
//        else
//        {
//            Debug.LogError("DialogueManager.Instance không tồn tại! Đảm bảo có một GameObject với DialogueManager trong scene.");

//            // Ẩn lại panel nếu không có DialogueManager
//            if (dialoguePanel != null)
//                dialoguePanel.SetActive(false);

//            // Nếu không có DialogueManager, vẫn hiển thị biểu tượng nếu được cấu hình
//            if (showIndicatorWithDialogue && indicatorPrefab != null)
//            {
//                ShowWorldIndicator();
//            }
//        }
//    }

//    // Hiển thị biểu tượng chỉ dẫn trong không gian thế giới
//    private void ShowWorldIndicator()
//    {
//        if (indicatorPrefab == null)
//            return;

//        // Nếu biểu tượng đã tồn tại, hủy đi để tạo mới
//        DestroyIndicator();

//        // Tạo biểu tượng mũi tên ở vị trí cố định trên thế giới
//        Vector3 indicatorPosition = transform.position + indicatorOffset;
//        indicatorInstance = Instantiate(indicatorPrefab, indicatorPosition, Quaternion.identity);

//        // Lưu vị trí ban đầu để tạo hiệu ứng lên xuống
//        initialIndicatorPosition = indicatorInstance.transform.position;
//        initialRotation = indicatorInstance.transform.rotation;

//        // Đặt biểu tượng là con của đối tượng này để di chuyển theo
//        indicatorInstance.transform.SetParent(this.transform, true);

//        // Reset các timer
//        bobTimer = 0f;
//        rotationTimer = 0f;
//    }

//    // Hiệu ứng nhịp nhảy và quay cho biểu tượng chỉ dẫn
//    private void AnimateWorldIndicator()
//    {
//        if (indicatorInstance == null)
//            return;

//        // Hiệu ứng nhịp nhảy lên xuống
//        bobTimer += Time.deltaTime * bobSpeed;
//        float bobOffset = Mathf.Sin(bobTimer) * bobHeight;
//        indicatorInstance.transform.position = initialIndicatorPosition + new Vector3(0, bobOffset, 0);

//        // Hiệu ứng quay
//        if (rotationSpeed > 0)
//        {
//            if (continuousRotation)
//            {
//                // Quay liên tục theo trục đã chọn
//                Vector3 rotationVector = Vector3.zero;
//                switch (rotationAxis)
//                {
//                    case RotationAxis.X:
//                        rotationVector = new Vector3(rotationSpeed * Time.deltaTime, 0, 0);
//                        break;
//                    case RotationAxis.Y:
//                        rotationVector = new Vector3(0, rotationSpeed * Time.deltaTime, 0);
//                        break;
//                    case RotationAxis.Z:
//                        rotationVector = new Vector3(0, 0, rotationSpeed * Time.deltaTime);
//                        break;
//                }
//                indicatorInstance.transform.Rotate(rotationVector);
//            }
//            else
//            {
//                // Quay qua lại trong khoảng giới hạn
//                rotationTimer += Time.deltaTime * rotationSpeed * 0.1f;
//                float angle = Mathf.Sin(rotationTimer) * maxRotationAngle;

//                // Reset rotation để tránh tích lũy
//                indicatorInstance.transform.rotation = initialRotation;

//                // Áp dụng góc quay theo trục đã chọn
//                switch (rotationAxis)
//                {
//                    case RotationAxis.X:
//                        indicatorInstance.transform.Rotate(angle, 0, 0);
//                        break;
//                    case RotationAxis.Y:
//                        indicatorInstance.transform.Rotate(0, angle, 0);
//                        break;
//                    case RotationAxis.Z:
//                        indicatorInstance.transform.Rotate(0, 0, angle);
//                        break;
//                }
//            }
//        }
//    }

//    // Hủy biểu tượng chỉ dẫn nếu còn tồn tại
//    private void DestroyIndicator()
//    {
//        if (indicatorInstance != null)
//        {
//            Destroy(indicatorInstance);
//            indicatorInstance = null;
//        }
//    }

//    // Được gọi khi một Collider2D khác vào vùng trigger
//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            isPlayerInTrigger = true;

//            if (triggerOnContact)
//            {
//                TriggerDialogue();
//            }
//        }
//    }

//    // Được gọi khi một đối tượng rời khỏi vùng trigger
//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            isPlayerInTrigger = false;

//            // Ẩn indicator khi người chơi rời đi nếu đang hiển thị cùng với dialogue
//            if (showIndicatorWithDialogue && indicatorInstance != null)
//            {
//                DestroyIndicator();
//            }
//        }
//    }

//    // Tùy chọn: Thêm phương thức để tiếp tục dialogue
//    public void ContinueDialogue()
//    {
//        if (DialogueManager.Instance != null)
//        {
//            DialogueManager.Instance.ContinueDialogue();
//        }
//    }

//    // Phương thức để reset trigger
//    public void ResetTrigger()
//    {
//        hasTriggered = false;

//        // Xóa dữ liệu đã lưu nếu persistAcrossScenes được bật
//        if (persistAcrossScenes && !string.IsNullOrEmpty(prefsKey))
//        {
//            PlayerPrefs.DeleteKey(prefsKey);
//            PlayerPrefs.Save();
//        }

//        // Hiển thị lại biểu tượng chỉ dẫn
//        if ((showIndicatorWithDialogue && isPlayerInTrigger) || !showIndicatorWithDialogue)
//        {
//            if (indicatorPrefab != null)
//            {
//                ShowWorldIndicator();
//            }
//        }
//    }

//    // Phương thức tĩnh để kiểm tra tất cả các triggers đã kích hoạt
//    public static void ResetAllPersistentTriggers()
//    {
//        // Xóa tất cả các keys liên quan đến DialogueTrigger
//        // Lưu ý: Điều này sẽ reset tất cả các dialogue triggers
//        PlayerPrefs.DeleteAll();
//        PlayerPrefs.Save();
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialougeCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialougeCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialougeTrigger : MonoBehaviour
{
    // Dialogue data
    public Dialogue dialogue;

    // UI References
    public GameObject dialoguePanel;

    [Header("Chỉ Dẫn")]
    [Tooltip("Biểu tượng mũi tên chỉ dẫn trên thế giới")]
    public GameObject indicatorPrefab;
    [Tooltip("Vị trí cố định của biểu tượng (tương đối với GameObject này)")]
    public Vector3 indicatorOffset = new Vector3(0, 1.5f, 0);
    [Tooltip("Tốc độ nhịp nhảy của biểu tượng")]
    public float bobSpeed = 2f;
    [Tooltip("Độ cao nhịp nhảy của biểu tượng")]
    public float bobHeight = 0.2f;

    [Header("Hiệu ứng xoay")]
    [Tooltip("Tốc độ quay của biểu tượng (độ/giây)")]
    public float rotationSpeed = 45f;
    [Tooltip("Có xoay liên tục không")]
    public bool continuousRotation = true;
    [Tooltip("Trục xoay (X, Y, Z hoặc Custom)")]
    public RotationAxis rotationAxis = RotationAxis.Z;
    [Tooltip("Góc xoay tối đa (nếu không xoay liên tục)")]
    public float maxRotationAngle = 45f;
    [Tooltip("Vector hướng quay tùy chỉnh (khi chọn Custom)")]
    public Vector3 customRotationAxis = new Vector3(0, 0, 1);
    [Tooltip("Góc xoay ban đầu (Euler)")]
    public Vector3 initialEulerAngles = Vector3.zero;

    [Header("Cài đặt khác")]
    [Tooltip("Hiển thị biểu tượng cùng lúc với dialogue")]
    public bool showIndicatorWithDialogue = true;

    // Trigger settings
    [Tooltip("Nếu true, dialogue sẽ tự động trigger khi va chạm với Player")]
    public bool triggerOnContact = true;

    [Tooltip("Nếu true, dialogue chỉ trigger một lần")]
    public bool triggerOnce = false;

    [Tooltip("Nếu true, dialogue chỉ trigger một lần và trạng thái sẽ được lưu giữa các scene")]
    public bool persistAcrossScenes = false;

    [Tooltip("ID duy nhất cho dialogue này, cần thiết khi persistAcrossScenes = true")]
    public string dialogueId;

    // Events
    [Tooltip("Event gọi khi dialogue bắt đầu")]
    public UnityEvent onDialogueStart;

    [Tooltip("Event gọi khi dialogue kết thúc")]
    public UnityEvent onDialogueEnd;

    // Enum cho trục xoay
    public enum RotationAxis { X, Y, Z, Custom }

    private bool hasTriggered = false;
    private string prefsKey;
    private GameObject indicatorInstance;
    private Vector3 initialIndicatorPosition;
    private float bobTimer = 0f;
    private float rotationTimer = 0f;
    private bool isPlayerInTrigger = false;
    private Quaternion initialRotation;

    private void Awake()
    {
        // Tạo prefsKey khi component được khởi tạo
        if (persistAcrossScenes)
        {
            // Nếu dialogueId không được cung cấp, tự động tạo ID từ tên GameObject và scene
            if (string.IsNullOrEmpty(dialogueId))
            {
                dialogueId = $"{gameObject.scene.name}_{gameObject.name}_{transform.position}";
                Debug.LogWarning($"DialogueId không được cung cấp cho {gameObject.name}. Tự động tạo ID: {dialogueId}");
            }

            prefsKey = $"DialogueTrigger_{dialogueId}";

            // Kiểm tra xem dialogue này đã từng được trigger chưa
            hasTriggered = PlayerPrefs.GetInt(prefsKey, 0) == 1;
        }

        // Chỉ hiển thị biểu tượng chỉ dẫn từ đầu nếu không cùng hiển thị với dialogue
        if (!showIndicatorWithDialogue && indicatorPrefab != null && !hasTriggered)
        {
            ShowWorldIndicator();
        }
    }

    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện khi dialogue kết thúc
        DialogueManager.OnDialogueEnd += OnDialogueEnded;

        // Chỉ hiển thị biểu tượng chỉ dẫn từ đầu nếu không cùng hiển thị với dialogue
        if (!showIndicatorWithDialogue && indicatorPrefab != null && !hasTriggered)
        {
            ShowWorldIndicator();
        }
    }

    private void OnDisable()
    {
        // Hủy đăng ký để tránh memory leak
        DialogueManager.OnDialogueEnd -= OnDialogueEnded;

        // Ẩn biểu tượng chỉ dẫn nếu còn tồn tại
        DestroyIndicator();
    }

    private void Update()
    {
        // Cập nhật hiệu ứng của biểu tượng chỉ dẫn
        AnimateWorldIndicator();
    }

    // Phương thức được gọi khi dialogue kết thúc
    void OnDialogueEnded()
    {
        // Ẩn panel nếu nó tồn tại
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Gọi event khi dialogue kết thúc
        onDialogueEnd?.Invoke();

        // Ẩn biểu tượng khi dialogue kết thúc nếu hiển thị cùng dialogue
        if (showIndicatorWithDialogue)
        {
            DestroyIndicator();
        }
        else if (!hasTriggered)
        {
            // Hiển thị lại biểu tượng sau khi dialogue kết thúc nếu không trigger một lần
            ShowWorldIndicator();
        }

        // Kiểm tra xem người chơi còn trong vùng trigger không để hiển thị lại nếu cần
        if (isPlayerInTrigger && !hasTriggered)
        {
            // Chờ một chút rồi hiển thị lại nếu người chơi vẫn ở trong vùng trigger
            Invoke("ShowIndicatorIfPlayerInRange", 0.5f);
        }
    }

    // Kiểm tra và hiển thị lại indicator nếu cần
    private void ShowIndicatorIfPlayerInRange()
    {
        if (isPlayerInTrigger && !hasTriggered)
        {
            if (showIndicatorWithDialogue)
            {
                ShowWorldIndicator();
            }
        }
    }

    // Phương thức công khai để kích hoạt dialogue
    public void TriggerDialogue()
    {
        if ((triggerOnce || persistAcrossScenes) && hasTriggered)
            return;

        hasTriggered = true;

        if (persistAcrossScenes)
        {
            PlayerPrefs.SetInt(prefsKey, 1);
            PlayerPrefs.Save();
        }

        // BẬT UI PANEL TRƯỚC
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // GỌI DialogueManager
        if (DialogueManager.Instance != null)
        {
            // CHỈ GỌI StartDialog NẾU DialogueManager đang active
            if (DialogueManager.Instance.gameObject.activeInHierarchy)
            {
                DialogueManager.Instance.StartDialog(dialogue);
            }
            else
            {
                Debug.LogError("DialogueManager đã bị tắt trong Hierarchy!");
            }

            if (showIndicatorWithDialogue && indicatorPrefab != null)
            {
                ShowWorldIndicator();
            }
            else if (!showIndicatorWithDialogue)
            {
                DestroyIndicator();
            }

            onDialogueStart?.Invoke();
        }
        else
        {
            Debug.LogError("DialogueManager.Instance không tồn tại!");
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            if (showIndicatorWithDialogue && indicatorPrefab != null)
            {
                ShowWorldIndicator();
            }
        }
    }

    // Hiển thị biểu tượng chỉ dẫn trong không gian thế giới
    private void ShowWorldIndicator()
    {
        if (indicatorPrefab == null)
            return;

        // Nếu biểu tượng đã tồn tại, hủy đi để tạo mới
        DestroyIndicator();

        // Tạo biểu tượng mũi tên ở vị trí cố định trên thế giới
        Vector3 indicatorPosition = transform.position + indicatorOffset;
        indicatorInstance = Instantiate(indicatorPrefab, indicatorPosition, Quaternion.identity);

        // Áp dụng góc xoay ban đầu nếu được cấu hình
        if (initialEulerAngles != Vector3.zero)
        {
            indicatorInstance.transform.eulerAngles = initialEulerAngles;
        }

        // Lưu vị trí ban đầu để tạo hiệu ứng lên xuống
        initialIndicatorPosition = indicatorInstance.transform.position;
        initialRotation = indicatorInstance.transform.rotation;

        // Đặt biểu tượng là con của đối tượng này để di chuyển theo
        indicatorInstance.transform.SetParent(this.transform, true);

        // Reset các timer
        bobTimer = 0f;
        rotationTimer = 0f;
    }

    // Hiệu ứng nhịp nhảy và quay cho biểu tượng chỉ dẫn
    private void AnimateWorldIndicator()
    {
        if (indicatorInstance == null)
            return;

        // Hiệu ứng nhịp nhảy lên xuống
        bobTimer += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(bobTimer) * bobHeight;
        indicatorInstance.transform.position = initialIndicatorPosition + new Vector3(0, bobOffset, 0);

        // Hiệu ứng quay
        if (rotationSpeed > 0)
        {
            if (continuousRotation)
            {
                // Quay liên tục theo trục đã chọn
                Vector3 rotationVector = Vector3.zero;
                switch (rotationAxis)
                {
                    case RotationAxis.X:
                        rotationVector = new Vector3(rotationSpeed * Time.deltaTime, 0, 0);
                        break;
                    case RotationAxis.Y:
                        rotationVector = new Vector3(0, rotationSpeed * Time.deltaTime, 0);
                        break;
                    case RotationAxis.Z:
                        rotationVector = new Vector3(0, 0, rotationSpeed * Time.deltaTime);
                        break;
                    case RotationAxis.Custom:
                        // Sử dụng vector hướng tùy chỉnh được chuẩn hóa
                        rotationVector = customRotationAxis.normalized * rotationSpeed * Time.deltaTime;
                        break;
                }
                indicatorInstance.transform.Rotate(rotationVector);
            }
            else
            {
                // Quay qua lại trong khoảng giới hạn
                rotationTimer += Time.deltaTime * rotationSpeed * 0.1f;
                float angle = Mathf.Sin(rotationTimer) * maxRotationAngle;

                // Reset rotation để tránh tích lũy
                indicatorInstance.transform.rotation = initialRotation;

                // Áp dụng góc quay theo trục đã chọn
                switch (rotationAxis)
                {
                    case RotationAxis.X:
                        indicatorInstance.transform.Rotate(angle, 0, 0);
                        break;
                    case RotationAxis.Y:
                        indicatorInstance.transform.Rotate(0, angle, 0);
                        break;
                    case RotationAxis.Z:
                        indicatorInstance.transform.Rotate(0, 0, angle);
                        break;
                    case RotationAxis.Custom:
                        // Quay theo vector tùy chỉnh
                        indicatorInstance.transform.Rotate(customRotationAxis.normalized * angle);
                        break;
                }
            }
        }
    }

    // Hủy biểu tượng chỉ dẫn nếu còn tồn tại
    private void DestroyIndicator()
    {
        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
            indicatorInstance = null;
        }
    }

    // Được gọi khi một Collider2D khác vào vùng trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            if (triggerOnContact)
            {
                TriggerDialogue();
            }
        }
    }

    // Được gọi khi một đối tượng rời khỏi vùng trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            // Ẩn indicator khi người chơi rời đi nếu đang hiển thị cùng với dialogue
            if (showIndicatorWithDialogue && indicatorInstance != null)
            {
                DestroyIndicator();
            }
        }
    }

    // Tùy chọn: Thêm phương thức để tiếp tục dialogue
    public void ContinueDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ContinueDialogue();
        }
    }

    // Phương thức để reset trigger
    public void ResetTrigger()
    {
        hasTriggered = false;

        // Xóa dữ liệu đã lưu nếu persistAcrossScenes được bật
        if (persistAcrossScenes && !string.IsNullOrEmpty(prefsKey))
        {
            PlayerPrefs.DeleteKey(prefsKey);
            PlayerPrefs.Save();
        }

        // Hiển thị lại biểu tượng chỉ dẫn
        if ((showIndicatorWithDialogue && isPlayerInTrigger) || !showIndicatorWithDialogue)
        {
            if (indicatorPrefab != null)
            {
                ShowWorldIndicator();
            }
        }
    }

    // Phương thức tĩnh để kiểm tra tất cả các triggers đã kích hoạt
    public static void ResetAllPersistentTriggers()
    {
        // Xóa tất cả các keys liên quan đến DialogueTrigger
        // Lưu ý: Điều này sẽ reset tất cả các dialogue triggers
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}