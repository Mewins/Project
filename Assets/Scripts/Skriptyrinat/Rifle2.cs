using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Rifle2 : MonoBehaviour
{
    public GameObject hand;
    Valve.VR.InteractionSystem.Hand scriptHand;
    public GameObject magazine;
    public GameObject casingPrefab;
    public GameObject PlaceForMagazine;
    public GameObject ReloadCollider;
    ColliderForRifleMagazine2 colliderForRifle;

    //public AudioSource source;
    //public AudioClip fireSound;
    //public AudioClip noAmmoSound;
    //public AudioClip detachMagazineSound; //58417577

    public bool hasSlide = true;

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

    private bool OnPress;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        OnPress = false;

        scriptHand = hand.GetComponent<Valve.VR.InteractionSystem.Hand>();

        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null) //тоже анимация
            gunAnimator = GetComponentInChildren<Animator>();

        colliderForRifle = ReloadCollider.GetComponent<ColliderForRifleMagazine2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptHand.currentAttachedObject != null)
        {
            if (scriptHand.currentAttachedObject.GetComponent<Rifle2>())
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
                else if (magazine.GetComponent<MagazinRifle>().ammo <= 0 || !hasSlide)
                {
                    gunAnimator.enabled = false;
                }
                //else gunAnimator.enabled = true; !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                if (colliderForRifle.hasSlide != hasSlide) hasSlide = colliderForRifle.hasSlide;

                if (Input.GetKey("space")/*buttonGrabPinch.GetStateDown(Pos.inputSource) && OnPress == false*/) //изменить кнопку на кнопку на контроллере
                {
                    gunAnimator.SetTrigger("Fire");// и это анимация
                    //Shoot();
                }

                //DEBUG
                if(Input.GetKey(KeyCode.U))
                {
                    Debug.Log("has slide = "+hasSlide);
                }
                //DEBUG

                Reload();
                if (magazine != null) ToggleMagMode();
            }
        }
        //if (buttonGrabPinch.GetStateUp(Pos.inputSource)) { OnPress = false; } Включить
    }

    void Shoot()
    {
        Debug.Log("Shoot");
        if (magazine.GetComponent<MagazinRifle>().ammo > 0 && RifleParams.isEmptyMagazine == false && hasSlide)
        {
            //source.PlayOneShot(fireSound); включить потом
            if (muzzleFlashPrefab)
            {
                //создание и удаление мазл флеша
                GameObject tempFlash;
                tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
                Destroy(tempFlash, destroyTimer);
            }

            GameObject tempBullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
            tempBullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
            tempBullet.GetComponent<BulletDestroy>().mode = 1;

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
            magazine.GetComponent<MagazinRifle>().ammo--;
        }
        //else source.PlayOneShot(noAmmoSound); включить потом
    }
    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (RifleParams.isEmptyMagazine == false)
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
        if (magazine.GetComponent<MagazinRifle>())
        {
            if (magazine.GetComponent<MagazinRifle>().mode == 1)
            {
                if (DistanceFromMagToPlace(magazine, PlaceForMagazine) >= 0.2f)
                {
                    magazine.GetComponent<MagazinRifle>().mode = 2;
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
            RifleParams.isEmptyMagazine = true;
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
