using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
//при попадании рейкастом на объект реагировать на него через время, за которое должна была пуля пролететь до объекта. Расстояние делить на скорость

public class Shotgun : MonoBehaviour
{
    [Header("Location Refrences")]
    public Transform BarrelLocation; // место дула
    public LayerMask layerMask; // по каким слоям можем стрелять
    //public Transform firePoint;
    public Transform casingExitLocation; //место для гильзы
    private Transform firePointStartTR;
    public GameObject magazineCollider; //коллайдер магазина мб пригодится

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
    private int shotgunFireParticleNumber = 2; // кол-во частиц при эффекте выстрела
    public int minFirePointRandomRot = -6; //минимальное отклонения дула для разброса (в градусах) 
    public int maxFirePointRandomRot = 6;//максимальное отклонения дула для разброса (в градусах) 
    [Tooltip("Specify time to destory the casing object")] [SerializeField] public float destroyTimer = 2f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    private float totalDamage; //итоговый урон одной пульки
    public int currentAmmo = 8;
    [SerializeField] public float bulletSpeed = 600f;
    private float recoilForce = 150f;

    [SerializeField] private Animator gunAnimator;// анимация

    private RaycastHit hit;
    public bool hasSlide = true;
    private Hand scriptHand;
    // Start is called before the first frame update
    void Start()
    {
        //shotgunFireSound = GetComponent<AudioSource>();
        firePointStartTR = BarrelLocation;
        scriptHand = hand.GetComponent<Hand>();

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
            if (scriptHand.currentAttachedObject.GetComponent<Shotgun>())
            {
                if (Input.GetKeyDown("space")/*buttonGrabPinch.GetStateDown(Pos.inputSource) && OnPress == false*/) //изменить кнопку на кнопку на контроллере
                {
                    if (currentAmmo > 0 && hasSlide)
                    {
                        //Shoot();
                        gunAnimator.SetTrigger("Fire");// и это анимация
                        hasSlide = false;
                    }
                    //else source.PlayOneShot(noAmmoSound); включить потом
                }
            }
        }
        //if (buttonGrabPinch.GetStateUp(Pos.inputSource)) { OnPress = false; } Включить
    }

    public void Shoot()
    {
        //shotgunFireSound.Play();
        //shotgunFireEffect.Emit(shotgunFireParticleNumber);   
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
            tempBullet.GetComponent<BulletDestroy>().mode = 6;
            //Instantiate(bulletPrefab, BarrelLocation.position, BarrelLocation.localRotation).GetComponent<Rigidbody>().AddForce(BarrelLocation.forward * shotPower);
            if (Physics.Raycast(BarrelLocation.position, fwd, out hit, ShotgunParams.range/*fireDist*/, layerMask))
            {
                Instantiate(hitParticles, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, BarrelLocation.forward)); //партиклы в месте попадания
                Debug.Log("Попали в имя " + hit.collider.name);

                if (hit.collider.tag == "Enemy")
                {
                    mob_dist = Vector3.Distance(transform.position, hit.collider.transform.position);
                    totalDamage = ShotgunParams.damage / mob_dist / 10;
                    Debug.Log("Текущий урон = " + (int)totalDamage);
                    tempBullet.GetComponent<BulletDestroy>().totalDamage = (int)totalDamage;
                }
            }
        }
        gameObject.GetComponent<Rigidbody>().AddForce(BarrelLocation.up * recoilForce); //вроде работает
        currentAmmo--;
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

    public void Slide()
    {
        hasSlide = true;
        //audio
    }
}
