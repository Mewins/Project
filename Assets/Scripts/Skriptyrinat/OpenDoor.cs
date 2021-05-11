using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject Spawner=null;
    private GameObject player;
    private Animator animator;
    private bool opened=false;


    private void Awake()
    {
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
    }

    void Open()
    {
        animator.SetTrigger("Open");
        opened = true;
    }

    void Close()
    {
        animator.SetTrigger("Close");
        opened = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(Spawner);
        }

        if (Spawner==null && opened == false && Vector3.Distance(gameObject.transform.position, player.transform.position) <= 8)
        {
            Open();
        }

        else Close();
    }
}
