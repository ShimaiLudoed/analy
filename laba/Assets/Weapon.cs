using System;

[Serializable]
public class Weapon
{
  public int id;
  public float damage;
  public float cooldown;

  public Weapon(int id, float damage, float cooldown)
  {
    this.id = id;
    this.damage = damage;
    this.cooldown = cooldown;
  }

  public void DebugPrint()
  {
    UnityEngine.Debug.Log($"🔫 Weapon {id}: damage={damage}, cooldown={cooldown}");
  }
}