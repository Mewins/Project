using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadColliderForShotgun : MonoBehaviour
{
    public GameObject shotgun;
    private Shotgun shotgunScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ShotgunBullet")
        {
            if (shotgunScript.currentAmmo < 8)
            {
                shotgunScript.currentAmmo++;
                shotgunScript.hasSlide = false;
                Destroy(other.gameObject);
            }
        }
    }

    void Start()
    {
        shotgunScript = shotgun.GetComponent<Shotgun>();
    }
}
