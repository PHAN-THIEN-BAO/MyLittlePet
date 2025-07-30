using UnityEngine;
using UnityEngine.UI;

public class PlayerController_Golf : MonoBehaviour
{
    public GolfBall playerBall;
    public float minShotPower = 2f;
    public float maxShotPower = 20f;
    public LayerMask groundLayer;
    public GameObject arrowIndicator;
    public Slider powerSlider;
    public AudioSource audioSource;
    public AudioClip hitSound;

    [HideInInspector] public bool isMyTurn = false;

    private bool canHit = true;
    private bool isCharging = false;
    private float currentPower;
    private Vector3 cachedDirection = Vector3.forward;

    private float rotationSpeed = 90f; 
    private float rotationSpeedIncrease = 60f; 
    private float currentRotationSpeed = 0f;

    public float chargeSpeed = 5f; 

    void Update()
    {
        if (isMyTurn)
        {
            canHit = true;
            arrowIndicator.SetActive(!playerBall.IsMoving());
            arrowIndicator.transform.position = playerBall.transform.position;

            if (arrowIndicator.activeSelf && Input.GetMouseButtonDown(1) && canHit)
            {
                isCharging = true;
                currentPower = minShotPower;
            }

            if (isCharging && Input.GetMouseButton(1))
            {
                currentPower += chargeSpeed * Time.deltaTime;
                if (currentPower > maxShotPower)
                    currentPower = maxShotPower;

                if (powerSlider != null)
                {
                    powerSlider.minValue = minShotPower;
                    powerSlider.maxValue = maxShotPower;
                    powerSlider.value = currentPower;
                }
            }

            if (isCharging && Input.GetMouseButtonUp(1) && canHit)
            {
                playerBall.SaveSafePosition();
                playerBall.HitBall(-arrowIndicator.transform.right, currentPower);
                FindObjectOfType<GameManager_Golf>().AddPlayerStroke();
                canHit = false;
                isMyTurn = false;

                if (audioSource != null && hitSound != null)
                    audioSource.PlayOneShot(hitSound);

                if (powerSlider != null)
                    powerSlider.value = minShotPower;

                isCharging = false;
            }
        }
        else
        {
            arrowIndicator.SetActive(false);
            canHit = false;
        }

        if (powerSlider != null)
            powerSlider.gameObject.SetActive(isCharging);

        if (arrowIndicator.activeSelf && Input.GetMouseButton(0))
        {
            currentRotationSpeed += rotationSpeedIncrease * Time.deltaTime;
            float speed = rotationSpeed + currentRotationSpeed;
            arrowIndicator.transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
        }

        if (arrowIndicator.activeSelf && Input.GetMouseButtonUp(0))
        {
            currentRotationSpeed = 0f;
        }
    }
}
