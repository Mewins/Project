using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ColliderForSniperMagazine : MonoBehaviour
{
    public GameObject PlaceForMagazine;
    string newName = "magazine";
    GameObject tempGO;
    bool swt = false;
    float smt = 2f;
    public bool hasSlide = true;

    //public AudioSource source; включить потом
    //public AudioClip reloadSound; включить потом

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Other tag: " + other.tag);
        //Debug.Log("Rifle Is Empty Mag: " + RifleParams.isEmptyMagazine);
        if (other.tag == "SniperMagazine" && SniperRifleParams.isEmptyMagazine == true)
        {
            if (other.GetComponent<SniperMagazine>().mode == 2)
            {
                Transform temp = other.transform;
                tempGO = other.gameObject;
                hasSlide = false;
                swt = true;
                other.gameObject.transform.rotation = PlaceForMagazine.transform.rotation*Quaternion.Euler(90,1,1);
                other.gameObject.transform.SetParent(transform.parent);
                other.transform.localScale = temp.localScale;

                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                other.transform.name = newName;
                other.GetComponent<SniperMagazine>().mode = 1;
                SniperRifleParams.isEmptyMagazine = false;

                //source.PlayOneShot(reloadSound);  включить потом
                Debug.Log(temp);
            }

        }
    }
    public void Slide()
    {
        hasSlide = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Debug.Log("IsEmptyMag = " + SniperRifleParams.isEmptyMagazine);
        //if (Input.GetKeyDown(KeyCode.U)) Debug.Log("Has Slide = " + hasSlide);
        //Debug.Log("Collider hasSlide = " + hasSlide);
        if (swt == true && tempGO != null)
        {
            Debug.Log(Vector3.Distance(tempGO.transform.position, PlaceForMagazine.transform.position));
            while (Vector3.Distance(tempGO.transform.position, PlaceForMagazine.transform.position) >= 0.001f)
            {
                Debug.Log("IUUUUUUU");
                tempGO.gameObject.transform.position = Vector3.Lerp(tempGO.gameObject.transform.position, PlaceForMagazine.transform.position, smt * Time.deltaTime);
            }

            tempGO = null;
            swt = false;
        }
        // сделать, чтобы не исчезало говно
    }
    // 0.02706696 -6.558353 -0.3046588
}

