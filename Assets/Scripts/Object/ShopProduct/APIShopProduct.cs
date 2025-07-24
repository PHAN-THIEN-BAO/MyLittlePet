using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class APIShopProduct : MonoBehaviour
{
    public static List<ShopProduct> GetAllShopProducts(string Type)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/ShopProduct/Type/" + Type);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();

        var allProducts = JsonConvert.DeserializeObject<List<ShopProduct>>(jsonResponse);

        allProducts.RemoveAll(p => p.status == 0);
        return allProducts;
    }
    public static ShopProduct GetShopProductById(int shopProductID)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/ShopProduct/" + shopProductID);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        return JsonConvert.DeserializeObject<ShopProduct>(jsonResponse);
    }

    public static ShopProduct GetShopProductByIdPet(int petId)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/ShopProduct/Pet/{petId}");
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log($"[API] Raw JSON for petId {petId}: {jsonResponse}");

                        var productResponse = JsonConvert.DeserializeObject<ShopProductResponse>(jsonResponse);

                        if (productResponse != null &&
                            productResponse.products != null &&
                            productResponse.products.Count > 0)
                        {
                            var product = productResponse.products[0];
                            Debug.Log($"[API] GetShopProductByIdPet({petId}) => imageUrl: {product.imageUrl}");
                            return product;
                        }
                        else
                        {
                            Debug.LogWarning($"No shop products found in response for pet ID {petId}");
                            return null;
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Error fetching shop product for pet ID {petId}: {response.StatusDescription}");
                    return null;
                }
            }
        }
        catch (WebException ex)
        {
            if (ex.Response is HttpWebResponse errorResponse && errorResponse.StatusCode == HttpStatusCode.NotFound)
            {
                Debug.LogWarning($"No shop product found for pet ID {petId}");
            }
            else
            {
                Debug.LogError($"Error in GetShopProductByIdPet: {ex.Message}");
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error in GetShopProductByIdPet: {ex.Message}");
            return null;
        }
    }

    [Serializable]
    public class ShopProductResponse
    {
        public string message;
        public List<ShopProduct> products;
    }






}