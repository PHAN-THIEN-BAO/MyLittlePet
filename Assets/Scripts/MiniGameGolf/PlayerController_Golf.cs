using UnityEngine;
using UnityEngine.UI;

public class PlayerController_Golf : MonoBehaviour
{
    public GolfBall playerBall;
    public float minShotPower = 2f;
    public float maxShotPower = 30f;
    public LayerMask groundLayer;
    public GameObject arrowIndicator;
    public Slider powerSlider;

    [HideInInspector] public bool isMyTurn = false;

    private bool canHit = true;
    private bool isCharging = false;
    private float currentPower;
    private Vector3 cachedDirection = Vector3.forward;

    private float rotationSpeed = 90f; 
    private float rotationSpeedIncrease = 60f; 
    private float currentRotationSpeed = 0f;

    void Update()
    {
        
        if (isMyTurn)
        {
            arrowIndicator.SetActive(true);

            
            arrowIndicator.transform.position = playerBall.transform.position;
        }
        else
        {
            arrowIndicator.SetActive(false);
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

        if (arrowIndicator.activeSelf && Input.GetMouseButtonDown(1))
        {
            isCharging = true;
            currentPower = minShotPower;
        }

        if (isCharging && Input.GetMouseButton(1))
        {
            currentPower += (maxShotPower - minShotPower) * Time.deltaTime;
            if (currentPower > maxShotPower)
                currentPower = maxShotPower;
        }

        if (isCharging && Input.GetMouseButtonUp(1))
        {
            playerBall.SaveSafePosition(); 
            playerBall.HitBall(-arrowIndicator.transform.right, currentPower);
            FindObjectOfType<GameManager_Golf>().AddPlayerStroke();
            canHit = false;


            if (powerSlider != null)
                powerSlider.value = minShotPower;

            isCharging = false;
        }

        if (isMyTurn && !playerBall.IsMoving())
        {
            if (!canHit)
                canHit = true;
        }
    }
}
