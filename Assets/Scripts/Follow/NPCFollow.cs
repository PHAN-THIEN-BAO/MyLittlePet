using System.Collections.Generic;
using UnityEngine;
public class NPCFollow : MonoBehaviour
{
    public Transform followCharacter; // The character to follow
    public float distanceFromCharacter; // Distance to maintain from the character
    public List<Vector2> followCharacterPositions = new List<Vector2>(); // List to store positions of the character
    public float allowableSampleDistance; // Allowable distance to consider the character as "followed"
    void Start()
    {
    }
    void Update()
    {
    }
}
