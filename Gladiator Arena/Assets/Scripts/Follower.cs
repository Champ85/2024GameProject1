using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform target;

    void FixedUpdate()
    {
        transform.SetPositionAndRotation(target.position, target.rotation);
    }
}
