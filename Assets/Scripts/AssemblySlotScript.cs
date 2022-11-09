using BNG;
using UnityEngine;

[RequireComponent(typeof(SnapZone))]
public class AssemblySlotScript : MonoBehaviour
{
    /// <summary> <see cref="SnapZone"/> component reference </summary>
    SnapZone sz;

    /// <summary> Number of bolts slots the assembly has (serialized just for inspector) </summary>
    
    [SerializeField, Min(0)] int nbBoltSlots;

    /// <summary> Number of bolts currently in the slots </summary>
    [Min(0)] public int nbBoltInSlots;

    /// <summary> Object that contains the slots in its children </summary>
    [SerializeField] Transform BoltSlots;

    /// <summary> Helper displayed when the object is removable from its spot </summary>
    public GameObject ringHelper;

    private void Awake()
    {
        sz = GetComponent<SnapZone>();

        nbBoltSlots = BoltSlots.childCount; //get the number of bolt slots by getting the child count of the BoltSlots object

        if (sz.StartingItem != null)
            sz.OnlyAllowNames[0] = sz.StartingItem.name;    //only allow the object that start snapped on this object to get snaped on this one
    }

    /// <summary>
    /// Assigned on each of its <see cref="BoltSlots"/>' <see cref="GrabAction.OnGrabEvent"/>.
    /// Decrement of <paramref name="nbBoltInSlots"/> and Set the assembly not removable when all slots are empty.
    /// </summary>
    public void OnBoltGrabbed()
    {
        nbBoltInSlots--;
        if (nbBoltInSlots == 0)
            SetItemRemovable(true);
    }

    /// <summary>
    /// Assigned on each of its <see cref="BoltSlots"/>' <see cref="SnapZone.OnSnapEvent"/>. Increment <paramref name="nbBoltInSlots"/>.
    /// </summary>
    public void OnBoltSnapped()
    {
        nbBoltInSlots++;
        SetItemRemovable(false);
    }

    /// <summary> Assigned on its <see cref="SnapZone.OnSnapEvent"/>.
    /// When snapped, show the helper if no bolt has been attached to this object</summary>
    public void OnSnapped()
    {
        if (nbBoltInSlots == 0)
            ringHelper.SetActive(true);
    }

    public void OnReleased()
    {
        ringHelper.SetActive(false);
    }

    /// <summary>
    /// Activate or deactive the helper and <paramref name="CanRemoveItem"/> of the SnapZone according to <paramref name="can"/> parameter
    /// </summary>
    /// <param name="can"></param>
    void SetItemRemovable(bool can)
    {
        ringHelper.SetActive(can);
        sz.CanRemoveItem = can;
    }

    /// <summary>
    /// Specific function for Vanne object, set its children's colliders to <b>not trigger</b> when it's snapped
    /// </summary>
    /// <param name="grab"></param>
    public void OnVanneSnapped(Grabbable grab)
    {
        Transform vanne = grab.transform.GetChild(0);
        for (int i = 0; i < vanne.childCount; i++)
        {
            if (vanne.GetChild(i).GetComponent<SnapZone>() != null) continue;
            Collider collider = vanne.GetChild(i).GetComponent<Collider>();
            if (collider != null)
                collider.isTrigger = false;
        }
    }

    /// <summary>
    /// Used in the two removabe pieces of the vanne assembly, by <see cref="SnapZone.OnSnapEvent"/>. It makes the vanne not flying due the inner collisions when released
    /// </summary>
    /// <param name="grab"></param>
    public void DisableCollider(Grabbable grab)
    {
        grab.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// Used in the two removabe pieces of the vanne assembly, by <see cref="SnapZone.OnDetachEvent"/>. Since they're not attached, they can have their collisions enabled
    /// </summary>
    /// <param name="grab"></param>
    public void EnableCollider(Grabbable grab)
    {
        grab.GetComponent<Collider>().enabled = true;
    }
}
