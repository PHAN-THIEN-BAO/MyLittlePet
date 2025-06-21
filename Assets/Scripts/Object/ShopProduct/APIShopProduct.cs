using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class APIShopProduct : MonoBehaviour
{
    /// <summary>
    /// Fetches all shop products of a specific type from the API.
    /// </summary>
    /// <param name="Type"></param>
    /// <returns></returns>
    public static List<ShopProduct> GetAllShopProducts(string Type)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/ShopProduct/Type/" + Type);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();

        // Parse the JSON response into a list of PlayerPet objects
        return JsonConvert.DeserializeObject<List<ShopProduct>>(jsonResponse);
    }
    /// <summary>
    /// Fetches a specific shop product by its ID from the API.
    /// </summary>
    /// <param name="shopProductID"></param>
    /// <returns></returns>
    public static ShopProduct GetShopProductById(int shopProductID)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/ShopProduct/" + shopProductID);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        // Parse the JSON response into a ShopProduct object
        return JsonConvert.DeserializeObject<ShopProduct>(jsonResponse);
    }


}
