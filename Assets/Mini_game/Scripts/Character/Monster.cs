using UnityEngine;

public class Monster : Character
{
    public string damgeAttack;

    public Monster()
    {
        name = "";
        health = "15000";
        damgeAttack = "500";
    }

    public override void PrintInfo()
    {
        Debug.Log("name: " + name + ",max health: " +health + ",damge: " + damgeAttack);
    }
}
