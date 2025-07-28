using System.Collections.Generic;
using UnityEngine;
public class NPCFollow : MonoBehaviour
{
    public Transform followCharacter;
    public float distanceFromCharacter;
    public List<Vector2> followCharacterPositions = new List<Vector2>();
    public float allowableSampleDistance;
    void Start()
    {
    }
    void Update()
    {
    }
}