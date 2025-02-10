using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float strength = 1;
    [SerializeField] private float attackDelay = 0.87f;

    private Animator animator;
    
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        weaponCollider.enabled = false;
    }

    public void StartAttack()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        PlayerController player = GetComponent<PlayerController>();
        player.attacking = true;
        weaponCollider.enabled = true;
        animator?.SetTrigger("Attack");
        float delay = attackDelay;
        while(delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }
        weaponCollider.enabled = false;
        player.attacking = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        Health h = collider.GetComponentInParent<Health>();
        if(h != null)
        {
            h.TakeDamage(strength);
        }
    }
}
