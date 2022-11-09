using System.Collections.Generic;
using UnityEngine;


//Seems some code are not used in the project
public class BoltSlotScript : MonoBehaviour
{
    /// <summary> Global list of <see cref="BoltSlotScript"/> used to show or hide the <see cref="BNG.RingHelper"/>. Used in <see cref="SnapableObject"/> </summary>
    public static List<BoltSlotScript> slots = new List<BoltSlotScript>();

    /// <summary> Helper shown when at <b><see cref="BNG.Grabbable.RemoteGrabDistance"/></b>, even through wall </summary>
    [SerializeField] public GameObject ringHelper;

    /// <summary> probably useless </summary>
    public bool isSnapped = true;

    private void Awake()
    {
        slots.Add(this);
        ringHelper.SetActive(false);
    }

    /// <summary>
    /// Assigned in <see cref="BNG.GrabAction.OnGrabEvent"/>
    /// </summary>
    public void OnGrabbed()
    {
        isSnapped = false;
        ringHelper.SetActive(true);
    }

    /// <summary>
    /// Assigned in <see cref="BNG.GrabAction.OnGrabEvent"/>
    /// </summary>
    public void OnSnapped()
    {
        isSnapped = true;
        ringHelper.SetActive(true);
    }
}
