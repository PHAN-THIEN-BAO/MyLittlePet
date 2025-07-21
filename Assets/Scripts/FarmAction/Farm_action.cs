using UnityEngine;
public class Farm_action : MonoBehaviour
{
    [Header("C�i d?t Animation")]
    [Tooltip("Animator di?u khi?n animation c�y tr?ng")]
    public Animator plantAnimator;
    [Tooltip("T�n c?a trigger trong Animator")]
    public string growthTriggerName = "plant";
    [Header("C�i d?t Tuong t�c")]
    [Tooltip("Ph�m d? tuong t�c v?i d?t")]
    public KeyCode interactKey = KeyCode.E;
    private bool canInteract = false;
    private bool hasGrown = false;
    private bool isAnimating = false;
    void Start()
    {
        if (plantAnimator == null)
        {
            plantAnimator = GetComponent<Animator>();
            if (plantAnimator == null)
            {
                plantAnimator = GetComponentInChildren<Animator>();
            }
            if (plantAnimator == null)
            {
                Debug.LogWarning("Chua g�n Animator cho c�y! H�y g�n trong Inspector.");
            }
        }
        if (plantAnimator != null)
        {
            plantAnimator.Rebind();
            plantAnimator.Update(0f);
        }
    }
    void Update()
    {
        if (canInteract && Input.GetKeyDown(interactKey) && !hasGrown && !isAnimating)
        {
            GrowPlant();
            hasGrown = true;
        }
    }
    public void GrowPlant()
    {
        if (plantAnimator != null && !isAnimating)
        {
            isAnimating = true;
            plantAnimator.ResetTrigger(growthTriggerName);
            plantAnimator.SetTrigger(growthTriggerName);
            Debug.Log("�� k�ch ho?t animation tr?ng c�y!");
            Invoke("FinishAnimation", 2.0f);
        }
    }
    private void FinishAnimation()
    {
        isAnimating = false;
        Debug.Log("Animation d� ho�n th�nh.");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            Debug.Log("B?n c� th? tuong t�c v?i m?nh d?t n�y. Nh?n ph�m " + interactKey + " d? tr?ng c�y.");
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
    void OnMouseDown()
    {
        if (!hasGrown && !isAnimating)
        {
            GrowPlant();
            hasGrown = true;
        }
    }
    public void ResetPlant()
    {
        hasGrown = false;
        isAnimating = false;
        if (plantAnimator != null)
        {
            plantAnimator.Rebind();
            plantAnimator.Update(0f);
        }
        Debug.Log("C�y d� du?c reset, c� th? tr?ng l?i!");
    }
}