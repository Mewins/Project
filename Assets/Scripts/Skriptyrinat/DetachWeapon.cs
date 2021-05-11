using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator returnTag()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            tag = "Weapon";
            yield break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "SetWeapon") // Если оружие касается площадки, где оно должно храниться
        {
            // Временно меняем тег оружия, чтобы сработало событие DetachObject из скрипта Hand (событие изменено)
            tag = "WeaponOK";
            StartCoroutine(returnTag()); // Закускаем корутину для обратной смены тега на "Weapon"
        }
    }
} 

