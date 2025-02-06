using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]

public class EnemyMove : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Health health;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        health.HitCallback += OnHit;
        health.DeathCallback += OnDie;

    }

    void Update()
    {
        if(player != null && Vector3.Distance(player.position, transform.position) <= agent.stoppingDistance)
        {
            agent.SetDestination(player.position);
            animator.SetInteger("MoveY", 1);
        }
        else
            animator.SetInteger("MoveY", 0);
    }

    private void OnHit()
    {
        animator.SetTrigger("Hit");
    }

    private void OnDie()
    {
        animator.SetTrigger("Death");
    }
}
