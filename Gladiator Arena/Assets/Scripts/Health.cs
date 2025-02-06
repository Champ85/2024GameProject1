using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float initalHealth;
    private float currentHealth;

    public UnityAction HitCallback;
    public UnityAction DeathCallback;
    
    private void Start()
    {
        currentHealth = initalHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0.0f, currentHealth - damage);
        if(currentHealth > 0)
            HitCallback?.Invoke();
        else
            DeathCallback?.Invoke();
    }
}
