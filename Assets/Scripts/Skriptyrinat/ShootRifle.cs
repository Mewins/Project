using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ShootRifle : MonoBehaviour
{
    public GameObject hand;
    Hand scriptHand;
    public GameObject casingPrefab;
    public GameObject ReloadCollider;
    ColliderForRifleMagazine colliderForRifle;

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
    int currentAmmo = 50;
    // Start is called before the first frame update
    void Start()
    {
        scriptHand = hand.GetComponent<Hand>();

        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null) //тоже анимация
            gunAnimator = GetComponentInChildren<Animator>();

        colliderForRifle = ReloadCollider.GetComponent<ColliderForRifleMagazine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptHand.currentAttachedObject != null)
        {
            if (scriptHand.currentAttachedObject.GetComponent<ShootRifle>())
            {
                if (!hasSlide)
                {
                    gunAnimator.enabled = false;
                }
                else if (currentAmmo <= 0 || !hasSlide)
                {
                    gunAnimator.enabled = false;
                }
                else gunAnimator.enabled = true; //тут может быть трабл

                if (colliderForRifle.hasSlide != hasSlide) hasSlide = colliderForRifle.hasSlide;

                if (Input.GetKey("space")) //изменить кнопку на кнопку на контроллере
                {
                    gunAnimator.SetTrigger("Fire");// и это анимация
                }
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Shoot");
        if (currentAmmo > 0 && RifleParams.isEmptyMagazine == false && hasSlide)
        {
            //source.PlayOneShot(fireSound); включить потом
            if (muzzleFlashPrefab)
            {
                //создание и удаление мазл флеша
                GameObject tempFlash;
                tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
                Destroy(tempFlash, destroyTimer);
            }
            GameObject tempBullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation*Quaternion.Euler(1,90,1));
            tempBullet.GetComponent<Rigidbody>().AddForce(0, 0, 20 * shotPower, ForceMode.Impulse);
            tempBullet.GetComponent<BulletDestroy>().mode = 3; //

            gameObject.GetComponent<Rigidbody>().AddForce(barrelLocation.up * recoilForce); //вроде работает
            currentAmmo--;
            hasSlide = false;
        }
        //else source.PlayOneShot(noAmmoSound); включить потом
    }

    public void Slide()
    {
        hasSlide = true;
        //audio kakoe nibud
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
