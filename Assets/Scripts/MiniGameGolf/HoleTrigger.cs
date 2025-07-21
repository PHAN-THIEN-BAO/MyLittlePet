using UnityEngine;
public class HoleTrigger : MonoBehaviour
{
    public GameManager_Golf gameManager;
    void OnTriggerEnter(Collider other)
    {
        GolfBall ball = other.GetComponent<GolfBall>();
        if (ball != null)
        {
            ball.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().Sleep();
            gameManager.RegisterFinish(ball);
        }
    }
}