using UnityEngine;
using System.Collections.Generic;

public class PetAIController : MonoBehaviour
{
    public GolfBall petBall;
    public Transform hole;
    public float shotPower = 10f;
    public LayerMask obstacleLayer;

    private int outOfBoundsCount = 0;
    private Vector3 lastMoveDirection = Vector3.zero;

    void Start() { }

    void Update() { }

    public void NotifyOutOfBounds()
    {
        outOfBoundsCount++;
    }

    public void MakeMove()
    {
        petBall.SaveSafePosition();

        Vector3 dirToHole = (hole.position - petBall.transform.position).normalized;
        float distance = Vector3.Distance(petBall.transform.position, hole.position);

        float ballRadius = 0.2f;
        List<int> bestIndices = new List<int>();
        float maxHitDistance = 0f;

        Vector3[] directions = new Vector3[73];
        float[] powers = new float[73];

        for (int i = 0; i < 73; i++)
        {
            int angle = -180 + i * 5;
            directions[i] = Quaternion.Euler(0, angle, 0) * dirToHole;
            powers[i] = shotPower * (Mathf.Abs(angle) < 1e-2 ? 1f : 0.9f);
        }

        for (int i = 0; i < directions.Length; i++)
        {
            float hitDistance = distance;
            bool canContinue = true;
            float powerMultiplier = 1f;

            if (Physics.SphereCast(petBall.transform.position, ballRadius, directions[i], out RaycastHit hit, distance, obstacleLayer))
            {
                hitDistance = hit.distance;

                float normalDot = Vector3.Dot(hit.normal, Vector3.up);
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                if (normalDot > 0.7f)
                {
                    canContinue = true;
                    if (slopeAngle > 20f)
                    {
                        float heightDiff = hit.point.y - petBall.transform.position.y;
                        if (heightDiff > 0.05f)
                        {
                            powerMultiplier = 1.0f + Mathf.Clamp(heightDiff * 2f, 0.1f, 1.0f);
                        }
                        else if (heightDiff < -0.05f)
                        {
                            powerMultiplier = 1.0f;
                        }
                    }
                }
                else
                {
                    canContinue = false;
                }
            }

            float thisPower = powers[i] * powerMultiplier;

            if (canContinue && hitDistance > maxHitDistance)
            {
                maxHitDistance = hitDistance;
                bestIndices.Clear();
                bestIndices.Add(i);
            }
            else if (canContinue && Mathf.Approximately(hitDistance, maxHitDistance))
            {
                bestIndices.Add(i);
            }

            Debug.DrawRay(petBall.transform.position, directions[i] * hitDistance, canContinue ? Color.green : Color.red, 2f);
        }

        if (bestIndices.Count > 0)
        {
            int chosen = bestIndices[Random.Range(0, bestIndices.Count)];
            int chosenAngle = -180 + chosen * 5;
            Debug.Log("PetAI: Chọn hướng đi được xa nhất trước khi chạm vật cản! Góc: " + chosenAngle);

            float powerMultiplier = 1f;
            if (Physics.SphereCast(petBall.transform.position, ballRadius, directions[chosen], out RaycastHit hit, distance, obstacleLayer))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                float heightDiff = hit.point.y - petBall.transform.position.y;
                if (slopeAngle > 20f && heightDiff > 0.05f)
                    powerMultiplier = 1.0f + Mathf.Clamp(heightDiff * 2f, 0.1f, 1.0f);
            }

            petBall.HitBall(directions[chosen], powers[chosen] * powerMultiplier);
            return;
        }

        Debug.Log("PetAI: Bị cản hoàn toàn, đánh nhẹ về phía lỗ!");
        petBall.HitBall(dirToHole, shotPower * 0.3f);
    }
}