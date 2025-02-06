using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Rigidbody target;

    void FixedUpdate()
    {
        transform.SetPositionAndRotation(target.position, target.rotation);
    }
}
