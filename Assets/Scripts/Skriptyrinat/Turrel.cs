using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turrel : MonoBehaviour
{
    public GameObject bulletPref, player, shootPlace;
    public float attackSpeed = 5f, health = 100f, attackRange, damage = 20f;
    private bool alreadyAttacked;

    void Shoot()
    {
        if (!alreadyAttacked)
        {

            Rigidbody rb = Instantiate(bulletPref, shootPlace.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            alreadyAttacked = true;
            rb.AddForce(transform.forward * 40f, ForceMode.Impulse);
            Invoke(nameof(ResetAttack), attackSpeed);
            Destroy(rb.gameObject, 1);
        }
    }
    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int value)
    {
        health -= value;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.LookAt(player.transform);
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) <= attackRange)
        {
            Shoot();
        }
    }
}
