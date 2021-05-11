using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*моды
1 - Rifle2
2 - SniperRifle
3 - AssaulRile
4 - AutoShotgun
5 - SMG
6 - Shotgun
*/
public class BulletDestroy : MonoBehaviour
{
    public GameObject hitEffect;
    [SerializeField] public int mode;
    [SerializeField] public int totalDamage;//для шотгана - урон от одной пули
    public GameObject emptyPrefab;
    private GameObject tempGO;
    void Start()
    {
        tempGO = Instantiate(emptyPrefab, gameObject.transform.position, gameObject.transform.rotation);
    }

    void Update()
    {
        //Debug.Log("Distance = " + Vector3.Distance(tempGO.transform.position, gameObject.transform.position));
        if (tempGO)
        {
            switch(mode)
            {
                case 1:
                    if (Vector3.Distance(tempGO.transform.position, gameObject.transform.position) > RifleParams.range)
                    {
                        Destroy(gameObject);
                        Destroy(tempGO);
                    }
                    break;
                case 2:
                    if (Vector3.Distance(tempGO.transform.position, gameObject.transform.position) > SniperRifleParams.range)
                    {
                        Destroy(gameObject);
                        Destroy(tempGO);
                    }
                    break;
                case 3:
                    if (Vector3.Distance(tempGO.transform.position, gameObject.transform.position) > AssaultRifleParams.range)
                    {
                        Destroy(gameObject);
                        Destroy(tempGO);
                    }
                    break;
                case 4: //autoShotgun
                    if (Vector3.Distance(tempGO.transform.position, gameObject.transform.position) > AutoShotgunParams.range)
                    {
                        Destroy(gameObject);
                        Destroy(tempGO);
                    }
                    break;
                case 5:
                    if (Vector3.Distance(tempGO.transform.position, gameObject.transform.position) > SmgParams.range)
                    {
                        Destroy(gameObject);
                        Destroy(tempGO);
                    }
                    break;
            }
            /*
            if(time>maxTime)
            {
               Destroy(tempGO);
               Destroy(gameObject);
            }
            */
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Попали в " + collision.collider.name);
        ContactPoint contactPoint = collision.contacts[0];
        GameObject temp = Instantiate(hitEffect, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
        temp.transform.SetParent(contactPoint.otherCollider.transform); // удочеряем
        gameObject.transform.SetParent(contactPoint.otherCollider.transform);
        if(collision.collider.tag == "Enemy")
        {
            switch(mode)
            {
                case 1: //Rifle2
                    collision.collider.GetComponent<EnemyAiv2>().TakeDamage(RifleParams.damage) ;
                    break;
                case 2: //SniperRifle
                    collision.collider.GetComponent<EnemyAiv2>().TakeDamage(SniperRifleParams.damage);
                    break;
                case 3://AssaultRifle
                    collision.collider.GetComponent<EnemyAiv2>().TakeDamage(AssaultRifleParams.damage);
                    break;
                case 4: //AutoShotgun
                    collision.collider.GetComponent<EnemyAiv2>().TakeDamage(totalDamage);
                    break;
                case 5: //Smg
                    collision.collider.GetComponent<EnemyAiv2>().TakeDamage(SmgParams.damage);
                    break;
            }
        }
        Destroy(tempGO);
        Destroy(gameObject);
    }
}
