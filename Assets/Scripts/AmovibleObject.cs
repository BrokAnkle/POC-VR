using BNG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmovibleObject : MonoBehaviour
{
    [SerializeField] bool IsInPlaceOnInitialization = true; //
    [SerializeField] Transform RemovedTransform;    //transform when the object had been removed from its place
    [SerializeField] Transform InPlaceTransform;    //transform when the object is in place
    bool isInPlace; //to know, when we want to move the object, is we should remove it or put it in place

    [SerializeField] float reasonableDistance = 0.001f; //minimal distance when reached, snap the object into its target

    public List<AmovibleObject> LockedBy;   //objects that blocks this one

    [HideInInspector]
    public List<AmovibleObject> Locks; //objects that are blocked by this one

    private void Awake()
    {
        isInPlace = IsInPlaceOnInitialization;
        Transform t;
        if (IsInPlaceOnInitialization)
            t = InPlaceTransform;
        else
            t = RemovedTransform;

        transform.SetPositionAndRotation(t.position, t.rotation);

        GetComponent<GrabbableUnityEvents>().onGrab.AddListener(TryMove);

        if (RemovedTransform == null)
            Debug.LogError("RemovedTransform of " + gameObject.name + " is not defined", this);
        if (InPlaceTransform == null)
            Debug.LogError("InPlaceTransform of " + gameObject.name + " is not defined", this);
    }

    void Start()
    {
        foreach (AmovibleObject o in LockedBy)
            o.Locks.Add(this);
    }

    IEnumerator TransformToTarget(Transform target)
    {
        Debug.Log("Start TransformToTarget", this);
        while (Vector3.Distance(transform.position, target.position) >= reasonableDistance)
        {
            Vector3 nextPosition = Vector3.Lerp(transform.position, target.position, 0.5f * Time.deltaTime);
            Quaternion nextRotation = Quaternion.Lerp(transform.rotation, target.rotation, 0.5f * Time.deltaTime);
            transform.SetPositionAndRotation(nextPosition, nextRotation);
            yield return null;
        }
        transform.SetPositionAndRotation(target.position, target.rotation);
        isInPlace = !isInPlace;
        yield break;
    }

    public void TryMove(Grabber g)
    {
        g.TryRelease();
        Debug.Log("try move", this);
        if (isInPlace)  //if this object is in its place, try to remove it
        {
            Debug.Log("You try to move a placed object", this);
            foreach (AmovibleObject o in LockedBy)
            {
                Debug.Log("Locked by " + o.name, this);
                if (o.isInPlace)
                {
                    Debug.Log("Can't move", this);
                    return;    //if the objects that blocks this objects are in place => can't move
                }
            }
            Debug.Log("Before coroutine", this);
            StartCoroutine(TransformToTarget(RemovedTransform));
            Debug.Log("After coroutine", this);
        }
        else    //if this object was removed from its place, try to put it in
        {
            Debug.Log("You try to move a removed object", this);
            foreach (AmovibleObject o in Locks)
            {
                Debug.Log("Locks " + o.name, this);
                if (!o.isInPlace)
                {
                    Debug.Log("Can't move", this);
                    return;                //if one object that sould be locked by this object when in place is not placed, can't move
                }
            }
            Debug.Log("Before coroutine", this);
            StartCoroutine(TransformToTarget(InPlaceTransform));
            Debug.Log("After coroutine", this);
        }
    }
}
