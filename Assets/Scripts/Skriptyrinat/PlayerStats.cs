using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int Health;
    private int MaxHealth = 100;
    public int armor;
    public GameObject StartPos, DeathPos;
    public Slider slider;

    private void Start()
    {
        Health = MaxHealth;
        StartPos = GameObject.Find("StartPos");
        DeathPos = GameObject.Find("DeathPos");
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            DestroyPlayer();
        }
    }
    private void Update()
    {
        slider.value = Health;
    }
    void DestroyPlayer()
    {
        gameObject.transform.position = DeathPos.transform.position;
        Health = MaxHealth;
    }


}
