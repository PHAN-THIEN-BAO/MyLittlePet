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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
