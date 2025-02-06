using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerActions playerActions;
    private InputAction attackAction;
    private Weapon weapon;
    private Animator animator;
    
    void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        animator = GetComponentInChildren<Animator>();

        playerActions = new PlayerActions();
        attackAction = playerActions.Game.Attack;
        attackAction.performed += AttackPressed;
    }

    void OnEnable()
    {
        attackAction.Enable();
    }

    void OnDisable()
    {
        attackAction.Disable();
    }

    void AttackPressed(InputAction.CallbackContext context)
    {
        weapon.ToggleAttacking(true);
        animator.SetTrigger("Attack");
    }
}
