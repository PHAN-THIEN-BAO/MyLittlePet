//using UnityEngine;

//public class CollectableItems : MonoBehaviour




















using UnityEngine;

[RequireComponent(typeof(Item))]
public class CollectableItems : MonoBehaviour
{
    [Header("Lo?i V?t Ph?m")]
    [SerializeField] private ItemType itemType = ItemType.Gem;

    [Header("Hi?u ?ng")]
    [SerializeField] private bool playSound = true;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private float destroyDelay = 0.2f;

    [Header("Hi?u ?ng Bay")]
    [SerializeField] private bool shouldFloat = true;
    [SerializeField] private float minLaunchForce = 3f;
    [SerializeField] private float maxLaunchForce = 5f;
    [SerializeField] private float minLaunchUpForce = 4f;
    [SerializeField] private float maxLaunchUpForce = 6f;
    [SerializeField] private float gravityScale = 1.2f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float bounceForce = 2f;
    [SerializeField] private float dragForce = 0.5f;
    [SerializeField] private bool enableRotation = false;

    private Item itemComponent;
    private bool canBeCollected = true;
    private float collectionDelay = 0.5f;
    private bool hasBounced = false;

    public enum ItemType
    {
        Gem,
        CarrotSeed,
        PotatoSeed,
        TomatoSeed,
    }

    private void Awake()
    {
        itemComponent = GetComponent<Item>();

        if (itemComponent != null && itemComponent.rb2d == null)
        {
            itemComponent.rb2d = GetComponent<Rigidbody2D>();
        }

        if (shouldFloat && itemComponent != null && itemComponent.rb2d != null)
        {
            itemComponent.rb2d.gravityScale = gravityScale;
            itemComponent.rb2d.linearDamping = dragForce;
            itemComponent.rb2d.angularDamping = 0.1f;
            itemComponent.rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            LaunchItem();

            Invoke("EnableCollection", collectionDelay);
            canBeCollected = false;
        }
        else
        {
            canBeCollected = true;
            if (itemComponent == null)
            {
                Debug.LogWarning("Thành ph?n Item không t?n t?i trên GameObject này!");
            }
            else if (itemComponent.rb2d == null)
            {
                Debug.LogWarning("Không tìm th?y Rigidbody2D trên GameObject này!");
            }
        }
    }

    private void Update()
    {
        if (enableRotation && shouldFloat && itemComponent != null &&
            itemComponent.rb2d != null &&
            itemComponent.rb2d.bodyType != RigidbodyType2D.Static &&
            itemComponent.rb2d.linearVelocity.magnitude < 0.5f)
        {
            if (Mathf.Abs(itemComponent.rb2d.angularVelocity) < 20f)
            {
                itemComponent.rb2d.angularVelocity = Random.Range(-30f, 30f);
            }
        }
    }

    private void LaunchItem()
    {
        float randomAngle = Random.Range(-60f, 60f);
        Vector2 direction = Quaternion.Euler(0, 0, randomAngle) * Vector2.right;

        float horizontalForce = Random.Range(minLaunchForce, maxLaunchForce);
        float upForce = Random.Range(minLaunchUpForce, maxLaunchUpForce);

        Vector2 launchVector = direction * horizontalForce + Vector2.up * upForce;
        itemComponent.rb2d.AddForce(launchVector, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (shouldFloat && itemComponent != null && !hasBounced)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 bounceDirection = Vector2.Reflect(itemComponent.rb2d.linearVelocity, normal);

            itemComponent.rb2d.linearVelocity = bounceDirection * 0.3f;

            if (normal.y > 0.5f)
            {
                itemComponent.rb2d.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                hasBounced = true;
            }

            itemComponent.rb2d.angularVelocity *= 0.7f;
        }
    }

    private void EnableCollection()
    {
        canBeCollected = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null && canBeCollected)
        {
            if (FarmGameManager.instance != null)
            {
                FarmGameManager.instance.CollectItem(player, itemType);
            }

            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            if (playSound && collectSound != null)
            {
                GameObject tempGO = new GameObject("TempAudio");
                tempGO.transform.position = transform.position;
                AudioSource audioSource = tempGO.AddComponent<AudioSource>();
                audioSource.clip = collectSound;
                audioSource.volume = 1.0f;
                audioSource.spatialBlend = 0f;
                audioSource.Play();

                Destroy(tempGO, collectSound.length);
            }

            Destroy(gameObject, destroyDelay);
        }
    }

    public void Collect(Player player)
    {
        if (player != null && canBeCollected)
        {
            if (FarmGameManager.instance != null)
            {
                FarmGameManager.instance.CollectItem(player, itemType);
            }

            Destroy(gameObject);
        }
    }
}