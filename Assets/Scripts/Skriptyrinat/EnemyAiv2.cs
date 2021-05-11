using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiv2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject player, shootPlace, projectile;
    public LayerMask whatIsGround, whatIsPlayer, whatIsWall;
    public Animator animator;

    public float health= 100f,lowHP = 30;
    public float maxhealth = 100f;
    public int damage = 30, regenValue=1;

    public Vector3 walkPoint;
    public Vector3 hidePoint;

    bool walkPointSet;
    bool hidePointSet;

    public float walkPointRange, hidePointRange;

    public float timeBetweenAttack;
    public bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public GameObject Spawner;

    public float razbrosRange;


    public GameObject[] Walls;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponentInChildren<Animator>();
        Walls = GameObject.FindGameObjectsWithTag("Wall");
    }
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if(health>lowHP)
        {
           if (!playerInSightRange && !playerInAttackRange) Patroling();
           //if (playerInSightRange && !playerInAttackRange) ChasePlayer();
           //if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }

        else
        {
            Hide();
        }
    }

    void Hide()
    {
        RaycastHit raycastHit;

        if (!hidePointSet)
        {
            HideFromPlayer();
        }

        if (hidePointSet) agent.SetDestination(hidePoint);

        Vector3 distantToHidePoint = transform.position - hidePoint;
        if (distantToHidePoint.magnitude < 1)
        {
            transform.LookAt(player.transform);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out raycastHit, sightRange))
            {
                if (raycastHit.transform.tag == "Player") hidePointSet = false;
            }
        }
    }
    void HideFromPlayer()
    {
        RaycastHit hit;
        GameObject closeWall = Walls[0];
        float randomZ = Random.Range(-hidePointRange, hidePointRange);
        float randomX = Random.Range(-hidePointRange, hidePointRange);

        for (int i = 0; i < Walls.Length; i++)
        {
            if (Vector3.Distance(closeWall.transform.position, player.transform.position) > Vector3.Distance(Walls[i].transform.position, player.transform.position))
            {
                closeWall = Walls[i];
            }
        }

        hidePoint = new Vector3(closeWall.transform.position.x + randomX, transform.position.y, closeWall.transform.position.z + randomZ);

        if (Physics.Raycast(hidePoint, -transform.up, whatIsGround))
        {
            if (Physics.Raycast(hidePoint, player.transform.position - transform.position, out hit, sightRange))
            {
                if (hit.transform.tag == "Wall")
                {
                    hidePointSet = true;
                }
            }
        }
    }
    IEnumerator Repair()
    {
        int i = 0, repairValue = 5;
        while (i <= repairValue)
        {
            i++;
            yield return new WaitForSeconds(1);
        }

        if (i >= repairValue)
        {
            Spawner.SetActive(true);
        }
    }
    void RepairSpawner()
    {
        agent.SetDestination(Spawner.transform.position);
        StartCoroutine(Repair());
    }
    void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distantToWalkPoint = transform.position - walkPoint;

        if (distantToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, whatIsGround))
        {
            if (!Physics.CheckSphere(walkPoint, 0.3f, whatIsWall))
            {
                walkPointSet = true;
            }
        }
    }
    void ChasePlayer()
    {
        agent.SetDestination(player.transform.position);
    }
    void AttackPlayer()
    {
        bool wallNear = Physics.CheckSphere(transform.position, 1.5f, whatIsWall);
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        agent.SetDestination(transform.position);
        transform.LookAt(player.transform.position);

        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, sightRange))
        {
            if (hit.transform.tag != "Player")
            {
                playerInAttackRange = false;
            }

            if(wallNear)
            {
                playerInAttackRange = false;
            }

            else
            {
                if (!alreadyAttacked)
                {
                    Rigidbody rb = Instantiate(projectile, shootPlace.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                    alreadyAttacked = true;
                    rb.AddForce(fwd * 40f, ForceMode.Impulse);
                    Invoke(nameof(ResetAttack), timeBetweenAttack);
                    Destroy(rb.gameObject, 1);
                }
            }
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void GoToPlayer()
    {
        agent.SetDestination(player.transform.position);
    }
}