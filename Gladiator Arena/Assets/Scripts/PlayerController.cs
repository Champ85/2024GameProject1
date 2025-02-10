using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]

public class PlayerController : MonoBehaviour
{
    #region actions
        private PlayerActions playerActions;
        private InputAction moveAction;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction attackAction;
    #endregion

    #region parameters
        [SerializeField] private float speed = 3.5f;
        [SerializeField] private float rotateSpeed = 5.0f;
        [SerializeField] private float turningRate = 2.0f;
        [SerializeField] private float maxFallSpeed = -25.0f;
        [SerializeField] private float jumpHeight = 2.0f;
        [SerializeField] private float bufferTime = 0.1f;
        [SerializeField] private float coyoteTime = 0.1f;
        public bool attacking = false;
    #endregion

    #region private variables
        private CharacterController controller;
        private Health health;
        private Animator animator;
        private bool jump = false;
        private float bufferTimer = 0.0f;
        private float coyoteTimer = 0.0f;
        private float yRotation;
    #endregion

    #region consts
        private const float gravity = -9.8f;
    #endregion

    #region Init
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            health = GetComponent<Health>();

            //set actions
            playerActions = new PlayerActions();
            moveAction = playerActions.Game.Movement;
            turnAction = playerActions.Game.Turning;
            jumpAction = playerActions.Game.Jump;
            jumpAction.performed += JumpPressed;
            attackAction = playerActions.Game.Attack;
            attackAction.performed += AttackPressed;

            //set events
            health.HitCallback += OnHit;
            health.DeathCallback += OnDie;

            yRotation = transform.rotation.y;
        }

        private void OnEnable()
        {
            moveAction.Enable();
            turnAction.Enable();
            jumpAction.Enable();
            attackAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            turnAction.Disable();
            jumpAction.Disable();
            attackAction.Disable();
        }
    #endregion

    #region Update
        private void FixedUpdate()
        {
            //turning
            yRotation += turnAction.ReadValue<Vector2>().x * turningRate;
            Quaternion target = Quaternion.Euler(0, yRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.fixedDeltaTime * rotateSpeed);

            //horizontalmovemn
            Vector3 movement = new Vector3(moveAction.ReadValue<Vector2>().x * speed, controller.velocity.y, moveAction.ReadValue<Vector2>().y * speed);
            movement = transform.TransformDirection(movement);

            //vertical movement
            if(jump)
            {
                if(controller.isGrounded || coyoteTimer <= coyoteTime)
                {
                    animator?.SetTrigger("Jump");
                    movement.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
                    jump = false;
                } 
                else //keep jump at true until the bufferTimer reaches bufferTime
                {
                    bufferTimer -= Time.fixedDeltaTime;
                    if(bufferTimer <= 0)
                    {
                        jump = false;
                    }
                }
            }
            if(!controller.isGrounded)
            {
                coyoteTimer += Time.fixedDeltaTime;
            }

            movement.y += gravity * Time.fixedDeltaTime;//apply gravity
            movement.y = Mathf.Max(movement.y, maxFallSpeed);//do not let falling speed exceed maxFallSpeed

            if(!attacking)
            {
                controller.Move(movement * Time.fixedDeltaTime);
            }

            //Animation
            animator?.SetInteger("MoveX", Mathf.RoundToInt(moveAction.ReadValue<Vector2>().x));
            animator?.SetInteger("MoveY", Mathf.RoundToInt(moveAction.ReadValue<Vector2>().y));
            animator?.SetBool("onGround", controller.isGrounded);
        }
    #endregion

    #region events
        private void JumpPressed(InputAction.CallbackContext context)
        {
            jump = true;
            bufferTimer = bufferTime;
        }

        private void OnHit()
        {
            animator?.SetTrigger("Hit");
        }

        private void OnDie()
        {
            animator?.SetTrigger("Death");
        }

        private void AttackPressed(InputAction.CallbackContext context)
        {
            PlayerAttack pAttack = GetComponent<PlayerAttack>();
            if(pAttack && !attacking && controller.isGrounded)
            {
                pAttack.StartAttack();
            }
        }
    #endregion
}