using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject EnemyPref, spawnPoint; 
    GameObject [] turrels = new GameObject[2];
    public OpenDoor openDoor;
    private int hp = 10;
    public int spawnTime=2;
    bool canBeenDestroed=false;

    void TakeDamage(int damage = 1)
    {
        hp -= damage;
        if(hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject Enemy = Instantiate(EnemyPref,spawnPoint.transform.position,Quaternion.identity);
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    void Update()
    {
        if(turrels==null)
        {
            canBeenDestroed = true;
        }
    }

}
