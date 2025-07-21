using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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

    private float rotationSpeed = 90f; // Độ/giây, tốc độ xoay ban đầu
    private float rotationSpeedIncrease = 60f; // Tăng tốc độ mỗi giây khi giữ chuột
    private float currentRotationSpeed = 0f;

    void Update()
    {
        bool showArrow = isMyTurn && !playerBall.IsMoving() && canHit;
        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(showArrow);

            // Luôn đặt mũi tên ngay trước banh người chơi
            if (showArrow)
            {
                arrowIndicator.transform.position = playerBall.transform.position;
            }
        }

        // Chỉ hiện thanh lực khi đang nạp lực
        if (powerSlider != null)
            powerSlider.gameObject.SetActive(isCharging);

        // Khi nhấn giữ chuột trái, mũi tên xoay liên tục và tăng tốc độ
        if (showArrow && Input.GetMouseButton(0))
        {
            currentRotationSpeed += rotationSpeedIncrease * Time.deltaTime;
            float speed = rotationSpeed + currentRotationSpeed;
            arrowIndicator.transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
        }
        // Khi thả chuột trái, giữ nguyên hướng và reset tốc độ xoay
        if (showArrow && Input.GetMouseButtonUp(0))
        {
            currentRotationSpeed = 0f;
        }

        // Khi nhấn giữ chuột phải, bắt đầu nạp lực
        if (showArrow && Input.GetMouseButtonDown(1))
        {
            isCharging = true;
            currentPower = minShotPower;
        }

        // Khi đang nạp lực (giữ chuột phải)
        if (isCharging && Input.GetMouseButton(1))
        {
            currentPower += (maxShotPower - minShotPower) * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, minShotPower, maxShotPower);

            if (powerSlider != null)
            {
                powerSlider.minValue = minShotPower;
                powerSlider.maxValue = maxShotPower;
                powerSlider.value = currentPower;
            }
        }

        // Khi thả chuột phải, đánh bóng theo hướng mũi tên với lực đã nạp
        if (isCharging && Input.GetMouseButtonUp(1))
        {
            playerBall.SaveSafePosition(); // Lưu vị trí an toàn trước khi đánh
            playerBall.HitBall(-arrowIndicator.transform.right, currentPower);
            FindObjectOfType<GameManager_Golf>().AddPlayerStroke();
            canHit = false;
            isCharging = false;
            if (powerSlider != null)
                powerSlider.value = minShotPower;
        }

        // Khi bóng dừng thì cho phép đánh lại (nếu vẫn là lượt mình)
        if (isMyTurn && !playerBall.IsMoving())
        {
            if (!canHit)
                canHit = true;
        }
    }
}
