using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BaseRigidBody_Controller : MonoBehaviour
{
    protected Rigidbody rb;
    protected AudioSource audioSource;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource)
        {
            audioSource.playOnAwake = false;
        }
    }

    void FixedUpdate()
    {
        if (rb)
        {
            HandlePhysics();
        }
    }

    protected virtual void HandlePhysics() { }
}