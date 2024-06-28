using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject projectile;
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;

    public float health;

    bool walkPointSet;

    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private Animator animator;

    private void Awake() {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        
        animator = GetComponentInChildren<Animator>();
        animator.enabled = true;
    }
    
    private void Update() {
        playerInSightRange =  Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) {
            Patroling();
            animator.SetBool("walking",true);
            animator.SetBool("shooting", false);
        }
        if (playerInSightRange && !playerInAttackRange) {
            ChasePlayer();
            animator.SetBool("walking", true);
            animator.SetBool("shooting", false);
        }
        if (playerInSightRange && playerInAttackRange) {
            AttackPlayer();
            animator.SetBool("shooting", true);
            animator.SetBool("walking", false);
        }
    }

    private void Patroling() {
        if (!walkPointSet) {
            SearchWalkPoint();
        }

        if (walkPointSet) {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint(){
        float randomX = Random.Range(-walkPointRange,walkPointRange);
        float randomZ = Random.Range(-walkPointRange,walkPointRange);

        walkPoint = new Vector3(transform.position.x +randomX, transform.position.y, transform.position.z +randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)){
            walkPointSet = true;
        }
    }

    private void ChasePlayer() {
        agent.SetDestination(player.position);
    }
    
    private void AttackPlayer() {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked && health > 0) {
            Rigidbody rb = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward *32f, ForceMode.Impulse);
            rb.AddForce(transform.up *3f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack),timeBetweenAttacks);
        }
    }

    private void ResetAttack(){
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage){
        health -= damage;

        if (health < 0){
            animator.SetBool("shooting", false);
            animator.SetBool("walking", false);
            animator.SetTrigger("dying");
            Invoke(nameof(DestroyEnemy),4.3f);
        }
    }

    private void DestroyEnemy(){
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "projectile") {
            TakeDamage(200);

            Destroy(other.gameObject);
        }
    }
}
