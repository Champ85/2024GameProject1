using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float strength;
    private bool attacking = false;
    
    void OnTriggerEnter(Collider collider)
    {
        Health h = collider.GetComponentInParent<Health>();
        if(h != null && attacking)
        {
            h.TakeDamage(strength);
            attacking = false;
        }
    }

    public void ToggleAttacking(bool b)
    {
        attacking = b;
    }

}
