using BNG;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SnapableObject : MonoBehaviour
{
    [Tooltip("Enable to keep the current rotation offset when inside the snapzone")]
    [SerializeField] bool storeCurrentOffset = false;

    [Tooltip("Does the object collide when released (not in a snap zone) after being grabbed ? Used to avoid multiple inner collisions and wild behavior")]
    [SerializeField] bool collideWhenReleased = true;

    [SerializeField] bool gravityWhenReleased = true;

    [Tooltip("If the object have one unique slot it can be snapped to; else, keep this field blank")]
    [SerializeField] SnapZone snapSlot;

    Rigidbody rb;
    Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (storeCurrentOffset)
        {
            SnapZoneOffset snapZoneOffset = GetComponent<SnapZoneOffset>();
            if (snapZoneOffset != null)
                snapZoneOffset.LocalRotationOffset = transform.rotation.eulerAngles;    //Keep the editor rotation to the play mode
        }
    }

    /// <summary>
    /// Assigned in the <see cref="GrabbableUnityEvents.onGrab"/>.
    /// Deactivate physics and show helper if condition met, when the object is grabbed
    /// </summary>
    public void OnGrabbed()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        col.isTrigger = true;

        //snapPlace only used for "Assembly" objects (currently)
        if (snapSlot != null)
            snapSlot.GetComponent<AssemblySlotScript>()?.ringHelper.SetActive(true);
        else
            foreach (var slot in BoltSlotScript.slots)  //iterate through every BoltSlots, activate the helper if the slot does not hold a bolt, telling the user he can put the holding bolt in
                if (slot.GetComponent<SnapZone>().HeldItem != null)
                    slot.ringHelper.gameObject.SetActive(false);
                else
                    slot.ringHelper.gameObject.SetActive(true);
    }

    /// <summary>
    /// Assigned in the <see cref="GrabbableUnityEvents.onRelease"/>.
    /// Make the object sensitive to gravity and activate its helper if condition met, when the object is released <b>in the world</b>.
    /// </summary>
    public void OnUngrabbed()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = gravityWhenReleased;
        }
        col.isTrigger = !collideWhenReleased;   //if the object is supposed to collide when released, trigger = false

        if (snapSlot != null)
            snapSlot.GetComponent<AssemblySlotScript>()?.ringHelper.SetActive(false);
        else
            foreach (var slot in BoltSlotScript.slots)
                if (slot.GetComponent<SnapZone>().HeldItem != null) //iterate through every BoltSlots, activate the helper if the slot hold a bolt, telling the user he can grab it
                    slot.ringHelper.gameObject.SetActive(true);
                else
                    slot.ringHelper.gameObject.SetActive(false);

        transform.parent = null;
    }

    /// <summary>
    /// Used only for the Vanne object in <see cref="GrabbableUnityEvents.onRelease"/>.
    /// When it's released, make all it's children's collider to <i>trigger</i> to avoid odd behavior (such as inner collisions)
    /// </summary>
    public void OnVanneReleased()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)  //iterate through all the children's mesh
        {
            MeshCollider collider = transform.GetChild(0).GetChild(i).GetComponent<MeshCollider>();
            if (collider != null)
            {
                collider.convex = true; //only convex mesh collider can be trigger
                collider.isTrigger = true;
            }
        }
    }
}
