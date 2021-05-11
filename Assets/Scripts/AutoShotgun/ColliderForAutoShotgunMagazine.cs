using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ColliderForAutoShotgunMagazine : MonoBehaviour
{
    public GameObject PlaceForMagazine;
    private string newName = "magazine";
    public Transform magazine;
    Vector3 magPos;

    //public AudioSource source; включить потом
    //public AudioClip reloadSound; включить потом

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AutoShotgunMagazine" && AutoShotgunParams.isEmptyMagazine == true)
        {
            if (other.GetComponent<AutoShotgunMagazine>().mode == 2)
            {
                Transform temp = other.transform;
                other.gameObject.transform.rotation = PlaceForMagazine.transform.rotation /** Quaternion.Euler(90, 1, 1)*/;
                other.gameObject.transform.SetParent(transform.parent);
                other.transform.localScale = temp.localScale;
                other.transform.localPosition = magPos;
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                other.transform.name = newName;
                other.GetComponent<AutoShotgunMagazine>().mode = 1;
                AutoShotgunParams.isEmptyMagazine = false;

                //source.PlayOneShot(reloadSound);  включить потом
            }

        }
    }

    void Start()
    {
        magPos = magazine.localPosition;
    }

}

