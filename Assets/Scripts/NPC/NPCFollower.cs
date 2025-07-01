using UnityEngine;

public class NPCFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float stopDistance = 1f;
    
    [Header("Movement Settings")]
    [SerializeField] private float smoothTime = 0.3f;
    
    [Header("Auto Find Player")]
    [SerializeField] private bool autoFindPlayer = true;
    [SerializeField] private string playerTag = "Player";
    
    // Private variables
    private Vector3 velocity = Vector3.zero;
    private bool isFollowing = true;
    
    void Start()
    {
        // Tự động tìm player nếu chưa được gán
        if (autoFindPlayer && player == null)
        {
            FindPlayer();
        }
    }
    
    void Update()
    {
        if (player != null && isFollowing)
        {
            FollowPlayer();
        }
    }
    
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log($"NPCFollower: Found player - {playerObject.name}");
        }
        else
        {
            Debug.LogWarning($"NPCFollower: No GameObject with tag '{playerTag}' found!");
        }
    }
    
    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Chỉ di chuyển nếu player đủ xa
        if (distanceToPlayer > followDistance)
        {
            Vector3 targetPosition = player.position;
            
            // Di chuyển mượt mà về phía player
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref velocity, 
                smoothTime, 
                followSpeed
            );
        }
    }
    
    // Public methods để điều khiển từ bên ngoài
    public void StartFollowing()
    {
        isFollowing = true;
    }
    
    public void StopFollowing()
    {
        isFollowing = false;
    }
    
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}