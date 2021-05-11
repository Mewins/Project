using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//Допилить говно, hasSlide убрать к херам, сделать как-то детект урона и все такое, ну ты понял. Еще анимацию сделай, а то че как лох

public class AutoShotgun : MonoBehaviour
{
    [Header("Location Refrences")]
    public Transform BarrelLocation; // место дула
    public LayerMask layerMask; // по каким слоям можем стрелять
    //public Transform firePoint;
    public Transform casingExitLocation; //место для гильзы
    private Transform firePointStartTR;
    public GameObject PlaceForMagazine;
    public GameObject magazine; //магазин

    [Header("Prefab Refrences")]
    public GameObject bulletPrefab; //префаб патрона
    public GameObject muzzleFlashPrefab; //префаб дульной вспышки
    public GameObject hand; //рука
    public GameObject casingPrefab; //префаб гильзы

    [Header("Visual and Auditory")]
    //private AudioSource shotgunFireSound;
    public GameObject hitParticles; // партиклы попадания
    //public ParticleSystem shotgunFireEffect; //эффект выстрела

    [Header("Settings")]
    public float fireDist = 50; //дистанция
    private int shotgunFireParticleNumber = 2; // кол-во частиц при эффекте выстрела
    public int minFirePointRandomRot = -6; //минимальное отклонения дула для разброса (в градусах) 
    public int maxFirePointRandomRot = 6;//максимальное отклонения дула для разброса (в градусах) 
    public int forceForRigidbodyObjects = 155; //сила, с которой толкаем ригидбади объекты
    [Tooltip("Specify time to destory the casing object")] [SerializeField] public float destroyTimer = 2f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    private float totalDamage; //итоговый урон одной пульки
    //public int currentAmmo = 8;
    [SerializeField] public float bulletSpeed = 600f;
    private float recoilForce = 150f;

    [SerializeField] private Animator gunAnimator;// анимация

    private Interactable interactable;
    public SteamVR_Behaviour_Pose Pos = null; // Хранит правый контроллер - поле назначается из редактора Unity
    private SteamVR_Action_Boolean buttonGrabPinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
    private SteamVR_Action_Boolean buttonGrabGrip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

    private bool OnPress;

    private RaycastHit hit;
    //public bool hasSlide = true;
    private Valve.VR.InteractionSystem.Hand scriptHand;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        OnPress = false;

        //shotgunFireSound = GetComponent<AudioSource>();
        firePointStartTR = BarrelLocation;
        scriptHand = hand.GetComponent<Valve.VR.InteractionSystem.Hand>();

        if (BarrelLocation == null)
            BarrelLocation = transform;

        if (gunAnimator == null) //тоже анимация
            gunAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptHand.currentAttachedObject != null)
        {
            if (scriptHand.currentAttachedObject.GetComponent<AutoShotgun>())
            {
                if (magazine == null)
                {
                    if (scriptHand.currentAttachedObject.transform.Find("magazine"))
                    {
                        magazine = scriptHand.currentAttachedObject.transform.Find("magazine").gameObject;
                    }
                }

                if (magazine == null)
                {
                    gunAnimator.enabled = false;
                }
                else if (magazine.GetComponent<AutoShotgunMagazine>().ammo <= 0)
                {
                    gunAnimator.enabled = false;
                }
                //else gunAnimator.enabled = true; !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                if (Input.GetKeyDown("space")/*buttonGrabPinch.GetStateDown(Pos.inputSource) && OnPress == false*/) //изменить кнопку на кнопку на контроллере (включить)
                {
                    if(AutoShotgunParams.isEmptyMagazine == false)
                    {
                        if (magazine.GetComponent<AutoShotgunMagazine>().ammo > 0)
                        {
                            //Shoot();
                            gunAnimator.SetTrigger("Fire");// и это анимация
                        }
                        //else source.PlayOneShot(noAmmoSound); включить потом
                    }
                }

                Reload();
                if (magazine != null) ToggleMagMode();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                Debug.Log("AssaultRifleMagazine.ammo = " + magazine.GetComponent<AutoShotgunMagazine>().ammo);
                Debug.Log("AssaultRifleParams.isEmptyMagazine = " + AutoShotgunParams.isEmptyMagazine);
            }
        }
        //if (buttonGrabPinch.GetStateUp(Pos.inputSource)) { OnPress = false; } Включить
    }

    public void Shoot()
    {
        //shotgunFireSound.Play();
        //shotgunFireEffect.Emit(shotgunFireParticleNumber);   
        if (muzzleFlashPrefab)
        {
            //создание и удаление мазл флеша
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, BarrelLocation.position, BarrelLocation.rotation);
            Destroy(tempFlash, destroyTimer);
        }

        float mob_dist;
        for (int bulletCounter = 10; bulletCounter > 0; bulletCounter--)
        {
            BarrelLocation.localRotation = Quaternion.identity;
            BarrelLocation.localRotation = Quaternion.Euler(firePointStartTR.localRotation.x + Random.Range(minFirePointRandomRot, maxFirePointRandomRot),
                                                            firePointStartTR.localRotation.y + Random.Range(minFirePointRandomRot, maxFirePointRandomRot),
                                                            firePointStartTR.localRotation.z + Random.Range(minFirePointRandomRot, maxFirePointRandomRot));
            Vector3 fwd = BarrelLocation.TransformDirection(-Vector3.right/*forward*/);
            GameObject tempBullet = Instantiate(bulletPrefab, BarrelLocation.position, BarrelLocation.rotation * Quaternion.Euler(1, -90, 1));
            tempBullet.GetComponent<Rigidbody>().AddForce(-BarrelLocation.right * bulletSpeed);
            //tempBullet.GetComponent<Rigidbody>().isKinematic = true;
            tempBullet.GetComponent<BulletDestroy>().mode = 4;
            Debug.DrawRay(BarrelLocation.position, fwd * 10, Color.red, 15f, false);
            //Instantiate(bulletPrefab, BarrelLocation.position, BarrelLocation.localRotation).GetComponent<Rigidbody>().AddForce(BarrelLocation.forward * shotPower);
            if (Physics.Raycast(BarrelLocation.position, fwd, out hit, AutoShotgunParams.range,/* fireDist, */layerMask))
            {
                if (hit.collider.tag == "Enemy")
                {
                    mob_dist = Vector3.Distance(transform.position, hit.collider.transform.position);
                    totalDamage = AutoShotgunParams.damage / mob_dist / 10;
                    tempBullet.GetComponent<BulletDestroy>().totalDamage = (int)totalDamage;
                    Debug.Log("Текущий урон = " + (int)totalDamage);
                }
            }
        }
        gameObject.GetComponent<Rigidbody>().AddForce(BarrelLocation.up * recoilForce); //вроде работает
        magazine.GetComponent<AutoShotgunMagazine>().ammo--;
        //else source.PlayOneShot(noAmmoSound); включить потом
    }

    public void CasingRelease()
    {
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);
        Destroy(tempCasing, destroyTimer);
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (AutoShotgunParams.isEmptyMagazine == false)
            {
                //source.PlayOneShot(detachMagazineSound); включить потом
                Detach();
            }
        }
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
            if (magazine.GetComponent<AutoShotgunMagazine>().mode == 1)
            {
                if (DistanceFromMagToPlace(magazine, PlaceForMagazine) >= 0.8f)
                {
                    magazine.GetComponent<AutoShotgunMagazine>().mode = 2;
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
            AutoShotgunParams.isEmptyMagazine = true;
        }
    }
}
