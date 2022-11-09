using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DiskGrabbable : MonoBehaviour
{
    Rigidbody rb;
    Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnGrabbed()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        col.isTrigger = true;
    }

    public void OnUngrabbed()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;
    }
}
