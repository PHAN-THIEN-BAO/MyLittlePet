[System.Serializable]
public class ShopProduct
{
    public int shopProductID;      // ShopProductID
    public int shopID;             // ShopID
    public int adminID;            // AdminID
    public int? petID;             // PetID (nullable)
    public string name;            // Name
    public string type;            // Type
    public string description;     // Description
    public string imageUrl;        // ImageUrl
    public int price;              // Price
    public string currencyType;    // CurrencyType
    public int? quantity;          // Quantity (nullable)
    public int status;             // Status
}
