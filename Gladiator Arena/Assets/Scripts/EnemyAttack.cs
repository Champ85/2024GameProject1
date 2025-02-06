using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    private Weapon weapon;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(Vector3.Distance(player.position, transform.position) <= agent.stoppingDistance)//also need to make sure enemy is facing player
        {
            Vector3 direction = player.position - transform.position;
            Quaternion target = Quaternion.FromToRotation(transform.forward, direction);
            if(false)//target is within specified bounds
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.fixedDeltaTime);
            else
                animator.SetTrigger("Attack");
        }
        bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Base.MeleeAttack_OneHanded");
        weapon.ToggleAttacking(attacking);
    }
}
