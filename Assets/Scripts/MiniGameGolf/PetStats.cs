using UnityEngine;

public class PetStats : MonoBehaviour
{
    public int health = 10;
    public int mood = 5;
    public int intelligent = 5;

    public void IncreaseHealth()
    {
        health++;
    }

    public void IncreaseMood()
    {
        mood++;
    }

    public void IncreaseIntelligent()
    {
        intelligent++;
    }

    public void IncreaseMoodAndIntelligent()
    {
        mood++;
        intelligent++;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}