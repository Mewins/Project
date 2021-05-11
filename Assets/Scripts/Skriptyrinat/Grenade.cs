using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 900f;
    public float delay = 3.5f;
    public float damage = 100f;
    public GameObject particle;

    public void ThrowGrenade()
    {
        Invoke("Explode", delay);
    }
    void Explode()
    {
        Instantiate(particle, transform.position, transform.rotation, gameObject.transform);
        Collider [] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider nearObjects in colliders)
        {
            Rigidbody rb = nearObjects.GetComponent<Rigidbody>();
            if(rb!=null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            EnemyAiv2 enemies = nearObjects.GetComponent<EnemyAiv2>();
            if(enemies != null)
            {
                enemies.TakeDamage(damage);
            }
        }
        Destroy(gameObject,0.5f);
    }

}
