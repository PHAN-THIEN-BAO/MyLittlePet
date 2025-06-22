using UnityEngine;

public class CurenciesValidation : MonoBehaviour
{
    public static bool ValidateCurrencies(int userCoin, int validateCoin, int quantity)
    {
        if (userCoin < 0 || validateCoin < 0)
        {
            

            return false;
        }
        if (userCoin < (validateCoin * quantity))
        {
            Debug.Log("User does not have enough coins.");
            return false;
        }
        else
        {
            Debug.Log("valid coin values provided.");
            return true;
        }
    }
}
