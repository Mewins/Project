using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class Smg : MonoBehaviour
{
    public GameObject hand;
    Valve.VR.InteractionSystem.Hand scriptHand;
    float force = 155;
    public GameObject magazine;
    public GameObject casingPrefab;
    public GameObject PlaceForMagazine;
    public GameObject ReloadCollider;
    ColliderForSmgMagazine colliderForRifle;
    float nextShoot;
    public float fireRate = 100;

    //public AudioSource source;
    //public AudioClip fireSound;
    //public AudioClip noAmmoSound;
    //public AudioClip detachMagazineSound; //58417577

    public bool hasSlide = true;// сделать

    [SerializeField] private Animator gunAnimator;// анимация

    [Header("Prefab Refrences")]
    public GameObject bulletPrefab; //префаб патрона
    public GameObject muzzleFlashPrefab; //префаб дульной вспышки

    [Header("Location Refrences")]
    [SerializeField] private Transform barrelLocation; //крайняя точка дула
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] public float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] public float shotPower = 600f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    public float recoilForce = 100f;

    private Interactable interactable;
    public SteamVR_Behaviour_Pose Pos = null; // Хранит правый контроллер - поле назначается из редактора Unity
    private SteamVR_Action_Boolean buttonGrabPinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
    private SteamVR_Action_Boolean buttonGrabGrip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();

        scriptHand = hand.GetComponent<Valve.VR.InteractionSystem.Hand>();

        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null) //тоже анимация
            gunAnimator = GetComponentInChildren<Animator>();

        colliderForRifle = ReloadCollider.GetComponent<ColliderForSmgMagazine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptHand.currentAttachedObject != null)
        {
            if (scriptHand.currentAttachedObject.GetComponent<Smg>())
            {
                //scriptHand.currentAttachedObject.name == "TestPistol"

                if (magazine == null)
                {
                    if (scriptHand.currentAttachedObject.transform.Find("magazine"))
                    {
                        magazine = scriptHand.currentAttachedObject.transform.Find("magazine").gameObject;
                    }
                }

                if (magazine == null || !hasSlide)
                {
                    gunAnimator.enabled = false;
                }
                else if (magazine.GetComponent<SmgMagazine>().ammo <= 0 || !hasSlide)
                {
                    gunAnimator.enabled = false;
                }
                //else gunAnimator.enabled = true; !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                if (colliderForRifle.hasSlide != hasSlide) hasSlide = colliderForRifle.hasSlide;

                if (Input.GetKey("space")/*buttonGrabPinch.GetStateDown(Pos.inputSource)*/) //изменить кнопку на кнопку на контроллере
                {
                    if (Time.time > nextShoot && magazine.GetComponent<SmgMagazine>().ammo > 0 && SmgParams.isEmptyMagazine == false && hasSlide)
                    {
                        gunAnimator.SetTrigger("Fire");// и это анимация
                                                       //Shoot();
                    }
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    Debug.Log("Time.time = " + Time.time);
                    Debug.Log("NextShoot = " + nextShoot);
                    Debug.Log("AssaultRifleMagazine.ammo = " + magazine.GetComponent<SmgMagazine>().ammo);
                    Debug.Log("AssaultRifleParams.isEmptyMagazine = " + SmgParams.isEmptyMagazine);
                    Debug.Log("HasSlide = " + hasSlide);
                }
                Reload();
                if (magazine != null) ToggleMagMode();
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Shoot");
        nextShoot = Time.time + 1f / fireRate;
        //source.PlayOneShot(fireSound); включить потом
        // время для следующего выстрела
        if (muzzleFlashPrefab)
        {
            //создание и удаление мазл флеша
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(tempFlash, destroyTimer);
        }

        GameObject tempBullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation * Quaternion.Euler(180, -90, 1));
        tempBullet.GetComponent<Rigidbody>().AddForce(barrelLocation.right * shotPower);
        tempBullet.GetComponent<BulletDestroy>().mode = 3;
        //tempBullet.GetComponent<Rigidbody>().isKinematic = true;

        //RaycastHit hit;
        //if (Physics.Raycast(barrelLocation.position, barrelLocation.transform.forward, out hit, RifleParams.range)) // пускаем райкаст
        //{
        //    if (hit.rigidbody != null) // если у объекта, в который попали есть ригидбади
        //    {
        //        hit.rigidbody.AddForce(-hit.normal * force); // толкаем его
        //    }
        //}

        gameObject.GetComponent<Rigidbody>().AddForce(barrelLocation.up * recoilForce); //вроде работает

        /*scriptHand.currentAttachedObject.transform.Find("magazine")*/
        magazine.GetComponent<SmgMagazine>().ammo--;

        //else source.PlayOneShot(noAmmoSound); включить потом
    }
    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SmgParams.isEmptyMagazine == false)
            {
                //source.PlayOneShot(detachMagazineSound); включить потом
                Detach();
            }
        }
    }

    public void Slide()
    {
        hasSlide = true;
        //audio kakoe nibud
    }

    float DistanceFromMagToPlace(GameObject magazine, GameObject PlaceForMag)
    {
        float dist = Vector3.Distance(PlaceForMag.transform.position, magazine.transform.position);
        return dist;
    }

    void ToggleMagMode()
    {
        if (magazine/*.GetComponent<AssaultRifleMagazine>()*/)
        {
            if (magazine.GetComponent<SmgMagazine>().mode == 1)
            {
                if (DistanceFromMagToPlace(magazine, PlaceForMagazine) >= 0.8f)
                {
                    magazine.GetComponent<SmgMagazine>().mode = 2;
                    magazine = null;
                }
            }
        }
    }

    void Detach()
    {
        if (magazine)
        {
            magazine.GetComponent<Rigidbody>().isKinematic = false;
            magazine.transform.SetParent(null);
            SmgParams.isEmptyMagazine = true;
            hasSlide = false;
        }
    }

    void CasingRelease()
    {
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);
        Destroy(tempCasing, destroyTimer);
    }

}
