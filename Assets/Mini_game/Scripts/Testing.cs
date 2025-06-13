using System;
using UnityEngine;

public class Testing : MonoBehaviour
{
    void Awake()
    {
        
    }

    void OnEnable()
    {
        
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Monster monster = new Monster();
        Character character = new Character();
        Character npc = new Character("Minh", "1000");
        npc.PrintInfo();
        character.PrintInfo();
        monster.PrintInfo();
        
    }
    

    // Update is called once per frame
    void Update()
    {
    }
}
