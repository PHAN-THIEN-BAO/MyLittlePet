using UnityEngine;

public class CollectableItems : MonoBehaviour
{
    [Header("Loại Vật Phẩm")]
    [SerializeField] private ItemType itemType = ItemType.CarrotSeed;

    [Header("Hiệu Ứng")]
    [SerializeField] private bool playSound = true;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private float destroyDelay = 0.2f;

    // Enum định nghĩa các loại vật phẩm có thể thu thập
    public enum ItemType
    {
        CarrotSeed,
        PotatoSeed,
        TomatoSeed,
        // Thêm các loại vật phẩm khác tại đây
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là người chơi không
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            // Thêm vật phẩm vào túi đồ của người chơi
            CollectItem(player);

            // Phát hiệu ứng nếu được cài đặt
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            // Phát âm thanh nếu được bật
            if (playSound && collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Xóa vật phẩm sau một khoảng thời gian ngắn
            Destroy(gameObject, destroyDelay);
        }
    }

    // Thêm vật phẩm vào người chơi dựa trên loại
    private void CollectItem(Player player)
    {
        switch (itemType)
        {
            case ItemType.CarrotSeed:
                player.numCarrotSeed++;
                Debug.Log("Đã thu thập hạt giống cà rốt. Tổng số: " + player.numCarrotSeed);
                break;

            // Bạn có thể thêm các loại vật phẩm khác ở đây
            // case ItemType.PotatoSeed:
            //     player.numPotatoSeed++;
            //     Debug.Log("Đã thu thập hạt giống khoai tây. Tổng số: " + player.numPotatoSeed);
            //     break;

            default:
                Debug.Log("Thu thập vật phẩm không xác định");
                break;
        }
    }

    // Phương thức này có thể được gọi từ các sự kiện khác (như nhấp chuột)
    public void Collect(Player player)
    {
        if (player != null)
        {
            CollectItem(player);
            Destroy(gameObject);
        }
    }
}