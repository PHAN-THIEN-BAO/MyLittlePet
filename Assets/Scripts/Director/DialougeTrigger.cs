//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class DialougeCharacter

//public class DialogueLine

//public class Dialogue

//public class DialougeTrigger : MonoBehaviour

























































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
    public Dialogue dialogue;

    public GameObject dialoguePanel;

    [Header("Ch? D?n")]
    [Tooltip("Bi?u tu?ng mui tên ch? d?n trên th? gi?i")]
    public GameObject indicatorPrefab;
    [Tooltip("V? trí c? d?nh c?a bi?u tu?ng (tuong d?i v?i GameObject này)")]
    public Vector3 indicatorOffset = new Vector3(0, 1.5f, 0);
    [Tooltip("T?c d? nh?p nh?y c?a bi?u tu?ng")]
    public float bobSpeed = 2f;
    [Tooltip("Ð? cao nh?p nh?y c?a bi?u tu?ng")]
    public float bobHeight = 0.2f;

    [Header("Hi?u ?ng xoay")]
    [Tooltip("T?c d? quay c?a bi?u tu?ng (d?/giây)")]
    public float rotationSpeed = 45f;
    [Tooltip("Có xoay liên t?c không")]
    public bool continuousRotation = true;
    [Tooltip("Tr?c xoay (X, Y, Z ho?c Custom)")]
    public RotationAxis rotationAxis = RotationAxis.Z;
    [Tooltip("Góc xoay t?i da (n?u không xoay liên t?c)")]
    public float maxRotationAngle = 45f;
    [Tooltip("Vector hu?ng quay tùy ch?nh (khi ch?n Custom)")]
    public Vector3 customRotationAxis = new Vector3(0, 0, 1);
    [Tooltip("Góc xoay ban d?u (Euler)")]
    public Vector3 initialEulerAngles = Vector3.zero;

    [Header("Cài d?t khác")]
    [Tooltip("Hi?n th? bi?u tu?ng cùng lúc v?i dialogue")]
    public bool showIndicatorWithDialogue = true;

    [Tooltip("N?u true, dialogue s? t? d?ng trigger khi va ch?m v?i Player")]
    public bool triggerOnContact = true;

    [Tooltip("N?u true, dialogue ch? trigger m?t l?n")]
    public bool triggerOnce = false;

    [Tooltip("N?u true, dialogue ch? trigger m?t l?n và tr?ng thái s? du?c luu gi?a các scene")]
    public bool persistAcrossScenes = false;

    [Tooltip("ID duy nh?t cho dialogue này, c?n thi?t khi persistAcrossScenes = true")]
    public string dialogueId;

    [Tooltip("Event g?i khi dialogue b?t d?u")]
    public UnityEvent onDialogueStart;

    [Tooltip("Event g?i khi dialogue k?t thúc")]
    public UnityEvent onDialogueEnd;

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
        if (persistAcrossScenes)
        {
            if (string.IsNullOrEmpty(dialogueId))
            {
                dialogueId = $"{gameObject.scene.name}_{gameObject.name}_{transform.position}";
                Debug.LogWarning($"DialogueId không du?c cung c?p cho {gameObject.name}. T? d?ng t?o ID: {dialogueId}");
            }

            prefsKey = $"DialogueTrigger_{dialogueId}";

            hasTriggered = PlayerPrefs.GetInt(prefsKey, 0) == 1;
        }

        if (!showIndicatorWithDialogue && indicatorPrefab != null && !hasTriggered)
        {
            ShowWorldIndicator();
        }
    }

    private void OnEnable()
    {
        DialogueManager.OnDialogueEnd += OnDialogueEnded;

        if (!showIndicatorWithDialogue && indicatorPrefab != null && !hasTriggered)
        {
            ShowWorldIndicator();
        }
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnd -= OnDialogueEnded;

        DestroyIndicator();
    }

    private void Update()
    {
        AnimateWorldIndicator();
    }

    void OnDialogueEnded()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        onDialogueEnd?.Invoke();

        if (showIndicatorWithDialogue)
        {
            DestroyIndicator();
        }
        else if (!hasTriggered)
        {
            ShowWorldIndicator();
        }

        if (isPlayerInTrigger && !hasTriggered)
        {
            Invoke("ShowIndicatorIfPlayerInRange", 0.5f);
        }
    }

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

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.gameObject.activeInHierarchy)
            {
                DialogueManager.Instance.StartDialog(dialogue);
            }
            else
            {
                Debug.LogError("DialogueManager dã b? t?t trong Hierarchy!");
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
            Debug.LogError("DialogueManager.Instance không t?n t?i!");
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            if (showIndicatorWithDialogue && indicatorPrefab != null)
            {
                ShowWorldIndicator();
            }
        }
    }

    private void ShowWorldIndicator()
    {
        if (indicatorPrefab == null)
            return;

        DestroyIndicator();

        Vector3 indicatorPosition = transform.position + indicatorOffset;
        indicatorInstance = Instantiate(indicatorPrefab, indicatorPosition, Quaternion.identity);

        if (initialEulerAngles != Vector3.zero)
        {
            indicatorInstance.transform.eulerAngles = initialEulerAngles;
        }

        initialIndicatorPosition = indicatorInstance.transform.position;
        initialRotation = indicatorInstance.transform.rotation;

        indicatorInstance.transform.SetParent(this.transform, true);

        bobTimer = 0f;
        rotationTimer = 0f;
    }

    private void AnimateWorldIndicator()
    {
        if (indicatorInstance == null)
            return;

        bobTimer += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(bobTimer) * bobHeight;
        indicatorInstance.transform.position = initialIndicatorPosition + new Vector3(0, bobOffset, 0);

        if (rotationSpeed > 0)
        {
            if (continuousRotation)
            {
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
                        rotationVector = customRotationAxis.normalized * rotationSpeed * Time.deltaTime;
                        break;
                }
                indicatorInstance.transform.Rotate(rotationVector);
            }
            else
            {
                rotationTimer += Time.deltaTime * rotationSpeed * 0.1f;
                float angle = Mathf.Sin(rotationTimer) * maxRotationAngle;

                indicatorInstance.transform.rotation = initialRotation;

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
                        indicatorInstance.transform.Rotate(customRotationAxis.normalized * angle);
                        break;
                }
            }
        }
    }

    private void DestroyIndicator()
    {
        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
            indicatorInstance = null;
        }
    }

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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            if (showIndicatorWithDialogue && indicatorInstance != null)
            {
                DestroyIndicator();
            }
        }
    }

    public void ContinueDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ContinueDialogue();
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;

        if (persistAcrossScenes && !string.IsNullOrEmpty(prefsKey))
        {
            PlayerPrefs.DeleteKey(prefsKey);
            PlayerPrefs.Save();
        }

        if ((showIndicatorWithDialogue && isPlayerInTrigger) || !showIndicatorWithDialogue)
        {
            if (indicatorPrefab != null)
            {
                ShowWorldIndicator();
            }
        }
    }

    public static void ResetAllPersistentTriggers()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}