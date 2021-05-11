using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public GameObject grenadePref;
    public Transform startPos;
    public int magAmmo=1, allAmmo=3;

    void Reload()
    {
        if (allAmmo > 0 && magAmmo ==0)
        {
            magAmmo = 1;
            allAmmo--;
        }
    }

    void Shoot()
    {
        //сделать при нажатии кнопки выстрела
        Vector3 vector = new Vector3(1,0,0);
        Rigidbody rb = Instantiate(grenadePref, startPos.position, Quaternion.Euler(90,0,0)).GetComponent<Rigidbody>();
        rb.AddForce(0, 0, 20, ForceMode.Impulse);
        magAmmo--;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && magAmmo!=0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
}
