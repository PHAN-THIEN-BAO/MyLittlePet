using UnityEngine;

public class FollowBall : MonoBehaviour
{
    public Transform target; // Gán bóng cần theo dõi (playerBall hoặc petBall)
    public Vector3 offset = new Vector3(-0.286f, 0.273f, -1.178f); // Điều chỉnh cho phù hợp
    public float rotateSpeed = 60f; // Độ/giây

    private float currentAngle = 0f;

    void LateUpdate()
    {
        if (target != null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                currentAngle += rotateSpeed * Time.deltaTime;
            }
            // transform.position = target.position + offset;
            // // Nếu muốn camera luôn nhìn vào bóng:
            // transform.LookAt(target);

            Quaternion rot = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rotatedOffset = rot * offset;

            transform.position = target.position + rotatedOffset;
            transform.LookAt(target);
        }
    }
}