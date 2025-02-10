using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private float deathTime = 3.0f;
    [SerializeField] private float attackDelay = 0.87f;
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float strength;
    
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Health health;
    private EnemyState state;

    private float waitTimer = 0;
    private bool attacking = false;

    

    enum EnemyState
    {
       Idle,
       Moving,
       Attacking,
       Dead,
    }
    
    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        health.HitCallback += OnHit;
        health.DeathCallback += OnDie;
        state = EnemyState.Idle;
    }

    void FixedUpdate()
    {
        switch(state)
        {
            case(EnemyState.Idle):
                animator.SetInteger("MoveY", 0);
                //do nothing
                ChangeState();
                break;
            case(EnemyState.Moving):
                //move to player
                agent.SetDestination(player.position);
                agent.isStopped = false;
                animator.SetInteger("MoveY", 1);
                ChangeState();
                break;
            case(EnemyState.Attacking):
                //attack player
                agent.isStopped = true;
                animator.SetInteger("MoveY", 0);
                if(!attacking)
                    StartCoroutine(Attack());
                break;
            case(EnemyState.Dead):
                //wait for death animation to stop then destroy gameobject
                waitTimer -= Time.fixedDeltaTime;
                if(waitTimer <= 0)
                    Destroy(gameObject);
                break;
        }
    }

    private void ChangeState()
    {
        if(player == null)
        {
            state = EnemyState.Idle;
        }
        else if(Vector3.Distance(player.position, transform.position) > agent.stoppingDistance)
        {
            state = EnemyState.Moving;
        }
        else 
            state = EnemyState.Attacking;
    }

    private void OnHit()
    {
        animator.SetTrigger("Hit");
    }

    private void OnDie()
    {
        animator.SetTrigger("Death");
        agent.isStopped = true;
        state = EnemyState.Dead;
        waitTimer = deathTime;
    }

    IEnumerator Attack()
    {
        attacking = true;
        Vector3 relativePosition = transform.InverseTransformPoint(player.position);
        while(relativePosition.x > 10 || relativePosition.x < -10)
        {
            transform.Rotate(relativePosition.x * Vector3.up, Space.Self);
            yield return null;
        }
        weaponCollider.enabled = true;
        animator.SetTrigger("Attack");
        float delay = attackDelay;
        while(delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }
        weaponCollider.enabled = false;
        attacking = false;
        ChangeState();
    }

    void OnTriggerEnter(Collider collider)
    {
        Health h = collider.GetComponentInParent<Health>();
        if(h != null)
        {
            h.TakeDamage(strength);
            attacking = false;
        }
    }
}
