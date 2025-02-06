using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]

public class PlayerMove : MonoBehaviour
{
    #region actions
        private PlayerActions playerActions;
        private InputAction moveAction;
        private InputAction turnAction;
        private InputAction jumpAction;
    #endregion

    #region parameters
        [SerializeField] private float speed;
        [SerializeField] private float rotateSpeed = 5.0f;
        [SerializeField] private float turningRate = 2.0f;
        [SerializeField] private float maxFallSpeed = 25.0f;
        [SerializeField] private float jumpForce = 25.0f;
        [SerializeField] private float bufferTime = 0.1f;
        [SerializeField] private float coyoteTime = 0.1f;
    #endregion

    #region variables
        private Rigidbody body;
        private Health health;
        private Animator animator;
        private bool onGround = true;
        private bool jump = false;
        private float bufferTimer = 0.0f;
        private float coyoteTimer = 0.0f;
        private int groundLayer;
        private ContactPoint[] contacts = new ContactPoint[50]; // Array of contact points to store the contacts
        private int nContacts = 0; // Number of contact points
        private Vector3 jumpPoint;//point player was at when they jumped
        private Vector3[] trail = new Vector3[100]; //the points in the debug trail
        private float yRotation;
    #endregion

    #region consts
        private const string groundLayerName = "Ground";
    #endregion
    
    #region Init
        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            health = GetComponent<Health>();
            groundLayer = LayerMask.NameToLayer(groundLayerName);

            //set actions
            playerActions = new PlayerActions();
            moveAction = playerActions.Game.Movement;
            turnAction = playerActions.Game.Turning;
            jumpAction = playerActions.Game.Jump;
            jumpAction.performed += JumpPressed;
            health.HitCallback += OnHit;
            health.DeathCallback += OnDie;

            //set the inital values in the trail
            for(int i = 0; i < trail.Length; i++)
            {
                trail[i] = transform.position;
            }
            //set inital yRotation
            yRotation = body.rotation.y;
        }

        private void OnEnable()
        {
            moveAction.Enable();
            turnAction.Enable();
            jumpAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            turnAction.Disable();
            jumpAction.Disable();
        }

    #endregion

    #region Update
        private void FixedUpdate()
        {
            //turning
            yRotation += turnAction.ReadValue<Vector2>().x * turningRate;
            Quaternion target = Quaternion.Euler(0, yRotation, 0);
            body.rotation = Quaternion.Slerp(body.rotation, target, Time.fixedDeltaTime * rotateSpeed);

            //horizontal movement
           
            Vector3 inputDirection = new Vector3(moveAction.ReadValue<Vector2>().x * speed, body.velocity.y, moveAction.ReadValue<Vector2>().y * speed);
            body.velocity = transform.TransformDirection(inputDirection);

            //vertical movement
            if(jump)
            {
                if(onGround || coyoteTimer <= coyoteTime)
                {
                    animator?.SetTrigger("Jump");
                    body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    jump = false;
                } 
                else //keep jump at true until the bufferTimer reaches bufferTime
                {
                    bufferTimer += Time.fixedDeltaTime;
                    if(bufferTimer >= bufferTime)
                    {
                        jump = false;
                    }
                }
            } 
            if(!onGround)
            {
                coyoteTimer += Time.fixedDeltaTime;
                if(body.velocity.y <= -maxFallSpeed)//maximum fall speed has been reached
                {
                    body.useGravity = false;
                    body.velocity = new Vector3(body.velocity.x, -maxFallSpeed, body.velocity.z);
                } 
                else 
                {
                    body.useGravity = true;
                }
            } 
            
            //trail
            MoveTrail();

            //Animation
            animator?.SetInteger("MoveX", Mathf.RoundToInt(moveAction.ReadValue<Vector2>().x));
            animator?.SetInteger("MoveY", Mathf.RoundToInt(moveAction.ReadValue<Vector2>().y));
            animator?.SetBool("onGround", onGround);
        }

    #endregion

    #region collisions
        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == groundLayer && collision.impulse.normalized == Vector3.up)
            {
                coyoteTimer = 0.0f;
                onGround = true;
            }
        }

        void OnCollisionStay(Collision collision)
        {
            nContacts = collision.GetContacts(contacts);
            if(collision.gameObject.layer == groundLayer && collision.impulse.normalized == Vector3.up)
            {
                onGround = true;
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if(collision.gameObject.layer == groundLayer && collision.impulse.normalized.y == 0 && onGround)
            {
                onGround = false;
            }
            nContacts = collision.GetContacts(contacts);
        }
    #endregion

    #region Gizmos
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < nContacts; i++)//draw contact points
            {
                Vector3 p = contacts[i].point;
                Gizmos.DrawWireSphere(p, 0.1f);
                Vector3 n = contacts[i].normal;
                Gizmos.DrawLine(p, p + n); 
            }
            for(int i = 1; i < trail.Length; i++)//draw a trail
            {
                Gizmos.DrawLine(trail[i - 1], trail[i]);
            }
        }
    #endregion

    #region events
        private void JumpPressed(InputAction.CallbackContext context)
        {
            jump = true;
            bufferTimer = 0.0f;
        }

        private void OnHit()
        {
            animator?.SetTrigger("Hit");
        }

        private void OnDie()
        {
            animator?.SetTrigger("Death");
        }
    #endregion

    #region helpers
        private void MoveTrail()
        {
            for (int i = trail.Length - 1; i > 0; i--)
            {
                trail[i] = trail[i - 1];
            }
            trail[0] = transform.position;
        }

    #endregion
}