using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject effect;

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(effect,transform.position,transform.rotation);
        Destroy(gameObject,1);
    }

}
