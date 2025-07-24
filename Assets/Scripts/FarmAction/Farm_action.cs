//using UnityEngine;

//public class Farm_action : MonoBehaviour












using UnityEngine;

public class Farm_action : MonoBehaviour
{
    [Header("Cài d?t Animation")]
    [Tooltip("Animator di?u khi?n animation cây tr?ng")]
    public Animator plantAnimator;

    [Tooltip("Tên c?a trigger trong Animator")]
    public string growthTriggerName = "plant";

    [Header("Cài d?t Tuong tác")]
    [Tooltip("Phím d? tuong tác v?i d?t")]
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
                Debug.LogWarning("Chua gán Animator cho cây! Hãy gán trong Inspector.");
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
            Debug.Log("Ðã kích ho?t animation tr?ng cây!");

            Invoke("FinishAnimation", 2.0f);
        }
    }

    private void FinishAnimation()
    {
        isAnimating = false;
        Debug.Log("Animation dã hoàn thành.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            Debug.Log("B?n có th? tuong tác v?i m?nh d?t này. Nh?n phím " + interactKey + " d? tr?ng cây.");
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

        Debug.Log("Cây dã du?c reset, có th? tr?ng l?i!");
    }
}