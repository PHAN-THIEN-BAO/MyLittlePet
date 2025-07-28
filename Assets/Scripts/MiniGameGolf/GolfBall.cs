using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public int shotsTaken = 0;
    private Rigidbody rb;
    private Vector3 spawnPoint;
    private Vector3 lastSafePosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawnPoint = transform.position;
        lastSafePosition = spawnPoint;
    }

    public void SaveSafePosition()
    {
        lastSafePosition = transform.position;
    }

    public void ResetToLastSafe()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        transform.position = lastSafePosition;
    }

    public void ResetToSpawn()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        transform.position = spawnPoint;
    }

    public void HitBall(Vector3 direction, float power)
    {
        // Đảm bảo bóng dừng hoàn toàn trước khi đánh tiếp
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep(); // Đảm bảo Rigidbody reset trạng thái động lực học

        rb.AddForce(direction.normalized * power, ForceMode.Impulse);
        shotsTaken++;
    }

    public bool IsMoving() => rb.linearVelocity.magnitude > 0.05f;
}
