using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GolfBall playerBall;
    public GolfBall petBall;
    public Transform hole;
    public PetAIController petAI;
    public PlayerController playerController;

    public GameObject playerCamera;
    public GameObject petCamera;

    private bool isPlayerTurn = true;
    private bool ballIsMoving = false;
    private bool petAIMovedThisTurn = false;
    private float stopTimer = 0f;
    private float waitAfterStop = 1.5f; // Số giây chờ sau khi bóng dừng

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isPlayerTurn)
            Debug.Log("Đến lượt Player");
        else
            Debug.Log("Đến lượt Pet");
        UpdateCamera();
    }

    // Update is called once per frame
    void Update()
    {
        // Cập nhật trạng thái lượt cho PlayerController
        playerController.isMyTurn = isPlayerTurn;

        if (!ballIsMoving)
        {
            if (isPlayerTurn)
            {
                // Chờ PlayerController xử lý input và đánh bóng
                if (playerBall.IsMoving())
                {
                    ballIsMoving = true;
                }
            }
            else
            {
                if (!petAIMovedThisTurn)
                {
                    petAI.MakeMove();
                    ballIsMoving = true;
                    petAIMovedThisTurn = true;
                }
            }
        }
        else
        {
            if (!playerBall.IsMoving() && !petBall.IsMoving())
            {
                stopTimer += Time.deltaTime;
                if (stopTimer >= waitAfterStop)
                {
                    ballIsMoving = false;
                    isPlayerTurn = !isPlayerTurn;
                    petAIMovedThisTurn = false;

                    // Thông báo lượt
                    if (isPlayerTurn)
                        Debug.Log("Đến lượt Player");
                    else
                        Debug.Log("Đến lượt Pet");

                    UpdateCamera();
                    stopTimer = 0f;
                }
            }
            else
            {
                stopTimer = 0f; // Nếu bóng lại di chuyển, reset timer
            }
        }
    }

    private void UpdateCamera()
    {
        playerCamera.SetActive(isPlayerTurn);
        petCamera.SetActive(!isPlayerTurn);

        var playerListener = playerCamera.GetComponent<AudioListener>();
        var petListener = petCamera.GetComponent<AudioListener>();
        if (playerListener != null) playerListener.enabled = isPlayerTurn;
        if (petListener != null) petListener.enabled = !isPlayerTurn;
    }
}
