using UnityEngine;

public class Character
{
   public string name;
   public string health;

   public Character(string name, string health)
   {
      this.name = name;
      this.health = health;
   }

   public Character()
   {
      this.name = "";
      this.health = "9000";
   }

   public virtual void PrintInfo()
   {
      Debug.Log("name: " + name + ",max health: " +health);
   }
   
}
