//using UnityEngine;

//[RequireComponent(typeof(Item))]
//public class CollectableItems : MonoBehaviour
//{
//    [Header("Loại Vật Phẩm")]
//    [SerializeField] private ItemType itemType = ItemType.Gem;

//    [Header("Hiệu Ứng")]
//    [SerializeField] private bool playSound = true;
//    [SerializeField] private AudioClip collectSound;
//    [SerializeField] private GameObject collectEffect;
//    [SerializeField] private float destroyDelay = 0.2f;

//    [Header("Hiệu Ứng Bay")]
//    [SerializeField] private bool shouldFloat = true; // Bật/tắt hiệu ứng bay
//    [SerializeField] private float launchForce = 2f; // Lực bắn ra khi sinh ra
//    [SerializeField] private float launchUpForce = 3f; // Lực bay lên
//    [SerializeField] private float gravityScale = 0.7f; // Độ nặng khi rơi xuống

//    private Item itemComponent;
//    private bool canBeCollected = false; // Ngăn việc thu thập ngay lập tức
//    private float collectionDelay = 0.5f; // Thời gian trước khi có thể thu thập

//    // Enum định nghĩa các loại vật phẩm có thể thu thập
//    public enum ItemType
//    {
//        Gem,
//        CarrotSeed,
//        PotatoSeed,
//        TomatoSeed,
//        // Thêm các loại vật phẩm khác tại đây
//    }

//    private void Awake()
//    {
//        itemComponent = GetComponent<Item>();

//        if (shouldFloat && itemComponent != null)
//        {
//            // Cấu hình Rigidbody2D
//            itemComponent.rb2d.gravityScale = gravityScale;
//            itemComponent.rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

//            // Phóng vật phẩm theo một hướng ngẫu nhiên
//            LaunchItem();

//            // Cho phép thu thập sau một khoảng thời gian
//            Invoke("EnableCollection", collectionDelay);
//        }
//        else
//        {
//            canBeCollected = true;
//        }
//    }

//    // Phóng vật phẩm theo một hướng ngẫu nhiên
//    private void LaunchItem()
//    {
//        // Tạo hướng ngẫu nhiên trên mặt phẳng XY
//        Vector2 randomDirection = Random.insideUnitCircle.normalized;

//        // Áp dụng lực phóng
//        Vector2 launchVector = randomDirection * launchForce + Vector2.up * launchUpForce;
//        itemComponent.rb2d.AddForce(launchVector, ForceMode2D.Impulse);
//    }

//    // Cho phép thu thập vật phẩm sau một thời gian chờ
//    private void EnableCollection()
//    {
//        canBeCollected = true;
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        // Kiểm tra xem đối tượng va chạm có phải là người chơi không
//        Player player = collision.GetComponent<Player>();

//        if (player != null && canBeCollected)
//        {
//            // Thêm vật phẩm vào túi đồ của người chơi thông qua FarmGameManager
//            if (FarmGameManager.instance != null)
//            {
//                FarmGameManager.instance.CollectItem(player, itemType);
//            }

//            // Phát hiệu ứng nếu được cài đặt
//            if (collectEffect != null)
//            {
//                Instantiate(collectEffect, transform.position, Quaternion.identity);
//            }

//            // Phát âm thanh nếu được bật
//            if (playSound && collectSound != null)
//            {
//                // Tạo GameObject tạm thời để phát âm thanh với âm lượng lớn hơn
//                GameObject tempGO = new GameObject("TempAudio");
//                tempGO.transform.position = transform.position;
//                AudioSource audioSource = tempGO.AddComponent<AudioSource>();
//                audioSource.clip = collectSound;
//                audioSource.volume = 1.0f; // Đặt âm lượng tối đa
//                audioSource.spatialBlend = 0f; // 0 = 2D sound (không gian hóa)
//                audioSource.Play();

//                // Xóa GameObject tạm thời sau khi âm thanh phát xong
//                Destroy(tempGO, collectSound.length);
//            }

//            // Xóa vật phẩm sau một khoảng thời gian ngắn
//            Destroy(gameObject, destroyDelay);
//        }
//    }

//    // Phương thức này có thể được gọi từ các sự kiện khác (như nhấp chuột)
//    public void Collect(Player player)
//    {
//        if (player != null && canBeCollected)
//        {
//            if (FarmGameManager.instance != null)
//            {
//                FarmGameManager.instance.CollectItem(player, itemType);
//            }

//            Destroy(gameObject);
//        }
//    }
//}

using UnityEngine;

[RequireComponent(typeof(Item))]
public class CollectableItems : MonoBehaviour
{
    [Header("Loại Vật Phẩm")]
    [SerializeField] private ItemType itemType = ItemType.Gem;

    [Header("Hiệu Ứng")]
    [SerializeField] private bool playSound = true;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private float destroyDelay = 0.2f;

    [Header("Hiệu Ứng Bay")]
    [SerializeField] private bool shouldFloat = true; // Bật/tắt hiệu ứng bay
    [SerializeField] private float minLaunchForce = 3f; // Lực bắn ra tối thiểu
    [SerializeField] private float maxLaunchForce = 5f; // Lực bắn ra tối đa
    [SerializeField] private float minLaunchUpForce = 4f; // Lực bay lên tối thiểu
    [SerializeField] private float maxLaunchUpForce = 6f; // Lực bay lên tối đa
    [SerializeField] private float gravityScale = 1.2f; // Độ nặng khi rơi xuống
    [SerializeField] private float rotationSpeed = 100f; // Tốc độ xoay
    [SerializeField] private float bounceForce = 2f; // Lực nảy khi chạm đất
    [SerializeField] private float dragForce = 0.5f; // Lực cản không khí

    private Item itemComponent;
    private bool canBeCollected = false; // Ngăn việc thu thập ngay lập tức
    private float collectionDelay = 0.5f; // Thời gian trước khi có thể thu thập
    private bool hasBounced = false; // Theo dõi nếu vật phẩm đã nảy lần đầu

    // Enum định nghĩa các loại vật phẩm có thể thu thập
    public enum ItemType
    {
        Gem,
        CarrotSeed,
        PotatoSeed,
        TomatoSeed,
        // Thêm các loại vật phẩm khác tại đây
    }

    private void Awake()
    {
        itemComponent = GetComponent<Item>();

        if (shouldFloat && itemComponent != null)
        {
            // Đảm bảo rb2d được khởi tạo
            if (itemComponent.rb2d == null)
                itemComponent.rb2d = itemComponent.GetComponent<Rigidbody2D>();

            // Đảm bảo Body Type là Dynamic
            itemComponent.rb2d.bodyType = RigidbodyType2D.Dynamic;

            // Cấu hình Rigidbody2D
            itemComponent.rb2d.gravityScale = gravityScale;
            itemComponent.rb2d.linearDamping = dragForce;
            itemComponent.rb2d.angularDamping = 0.1f;
            itemComponent.rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Thêm một lực xoay ngẫu nhiên
            float randomRotation = Random.Range(-1f, 1f);
            itemComponent.rb2d.angularVelocity = randomRotation * rotationSpeed;

            // Phóng vật phẩm theo một hướng ngẫu nhiên
            LaunchItem();

            // Cho phép thu thập sau một khoảng thời gian
            Invoke("EnableCollection", collectionDelay);
        }
        else
        {
            canBeCollected = true;
        }
    }

    private void Update()
    {
        // Thêm hiệu ứng xoay nhẹ để trông sinh động hơn
        if (shouldFloat && itemComponent != null && itemComponent.rb2d.linearVelocity.magnitude < 0.5f)
        {
            // Nếu vật phẩm gần như đứng yên, cho nó xoay nhẹ
            if (Mathf.Abs(itemComponent.rb2d.angularVelocity) < 20f)
            {
                itemComponent.rb2d.angularVelocity = Random.Range(-30f, 30f);
            }
        }
    }

    // Phóng vật phẩm theo một hướng ngẫu nhiên
    private void LaunchItem()
    {
        // Tạo hướng ngẫu nhiên trên mặt phẳng XY (nghiêng nhiều hơn về phía trên)
        float randomAngle = Random.Range(-60f, 60f); // Góc từ -60 đến 60 độ
        Vector2 direction = Quaternion.Euler(0, 0, randomAngle) * Vector2.right;

        // Tạo lực ngẫu nhiên trong khoảng min-max
        float horizontalForce = Random.Range(minLaunchForce, maxLaunchForce);
        float upForce = Random.Range(minLaunchUpForce, maxLaunchUpForce);

        // Áp dụng lực phóng
        Vector2 launchVector = direction * horizontalForce + Vector2.up * upForce;
        itemComponent.rb2d.AddForce(launchVector, ForceMode2D.Impulse);
    }

    // Xử lý va chạm với mặt đất/tường
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem va chạm có phải với mặt đất/tường không
        if (shouldFloat && itemComponent != null && !hasBounced)
        {
            // Tính toán hướng nảy dựa trên normal của va chạm
            Vector2 normal = collision.contacts[0].normal;
            Vector2 bounceDirection = Vector2.Reflect(itemComponent.rb2d.linearVelocity, normal);

            // Áp dụng lực nảy với giảm độ mạnh
            itemComponent.rb2d.linearVelocity = bounceDirection * 0.3f;

            // Thêm lực nảy lên nếu va chạm từ dưới lên (mặt đất)
            if (normal.y > 0.5f) // Đang va chạm với bề mặt ở dưới
            {
                itemComponent.rb2d.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                hasBounced = true; // Đánh dấu đã nảy lần đầu
            }

            // Giảm lực xoay
            itemComponent.rb2d.angularVelocity *= 0.7f;
        }
    }

    // Cho phép thu thập vật phẩm sau một thời gian chờ
    private void EnableCollection()
    {
        canBeCollected = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là người chơi không
        Player player = collision.GetComponent<Player>();

        if (player != null && canBeCollected)
        {
            // Thêm vật phẩm vào túi đồ của người chơi thông qua FarmGameManager
            if (FarmGameManager.instance != null)
            {
                FarmGameManager.instance.CollectItem(player, itemType);
            }

            // Phát hiệu ứng nếu được cài đặt
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            // Phát âm thanh nếu được bật
            if (playSound && collectSound != null)
            {
                // Tạo GameObject tạm thời để phát âm thanh với âm lượng lớn hơn
                GameObject tempGO = new GameObject("TempAudio");
                tempGO.transform.position = transform.position;
                AudioSource audioSource = tempGO.AddComponent<AudioSource>();
                audioSource.clip = collectSound;
                audioSource.volume = 1.0f; // Đặt âm lượng tối đa
                audioSource.spatialBlend = 0f; // 0 = 2D sound (không gian hóa)
                audioSource.Play();

                // Xóa GameObject tạm thời sau khi âm thanh phát xong
                Destroy(tempGO, collectSound.length);
            }

            // Xóa vật phẩm sau một khoảng thời gian ngắn
            Destroy(gameObject, destroyDelay);
        }
    }

    // Phương thức này có thể được gọi từ các sự kiện khác (như nhấp chuột)
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