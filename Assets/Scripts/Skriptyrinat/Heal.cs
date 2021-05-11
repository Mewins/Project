using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
   public PlayerStats Player;
  void TakeHeal(int value)
  {
        Player.Health += value;
        Destroy(gameObject);
  }
}
