using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnBase : MonoBehaviour
{
    public Transform BaseTeleport;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Hand")
        {
            other.gameObject.transform.position = BaseTeleport.position;
        }
    }

}
