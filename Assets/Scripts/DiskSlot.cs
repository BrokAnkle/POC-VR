using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SnapZone))]
public class DiskSlot : MonoBehaviour
{
    enum Color
    {
        RED, WHITE, BLUE
    }
    //static DiskScript[] disks = new DiskScript[3];
    static DiskSlot red;
    static DiskSlot white;
    static DiskSlot blue;
    [SerializeField] Color color;
    bool isInPlace;
    SnapZone sz;
    private void Awake()
    {
        sz = GetComponent<SnapZone>();

        switch (color)
        {
            case Color.RED:
                red = this;
                sz.CanRemoveItem = true;
                break;
            case Color.WHITE:
                white = this;
                sz.CanRemoveItem = false;
                break;
            case Color.BLUE:
                blue = this;
                sz.CanRemoveItem = false;
                break;
            default: break;
        }
    }


    //When object whant to get snap into the snapzone
    public void TrySnap()
    {
        switch (color)
        {
            case Color.RED: //to snap the red disk, blue and white disks needs to be snaped
                if (!(white.isInPlace && blue.isInPlace))
                    return;
                white.sz.CanRemoveItem = false;
                break;
            case Color.WHITE:   //to snap the white disk, blue disk needs to be snapped (and red should not)
                if (red.isInPlace || !blue.isInPlace)
                    return;
                blue.sz.CanRemoveItem = false;
                red.gameObject.SetActive(true);
                break;
            case Color.BLUE:    //to snap the blue disk, red and white disks needs to not be snapped
                if (red.isInPlace || white.isInPlace)
                    return;
                white.gameObject.SetActive(true);
                break;
            default: break;
        }
        isInPlace = true;

    }

    //When object is grabbed from the snapzone
    public void TryUnsnap(Grabber grabber)
    {
        switch (color)
        {
            case Color.RED: //to unsnap the red disk, nothing needed, it should be on top
                white.sz.CanRemoveItem = true;  //by unsnapping the red disk, the white is now accessible
                break;
            case Color.WHITE:   //to unsnap the white disk, the red disk should be unsnapped
                if (red.isInPlace)
                    return;
                blue.sz.CanRemoveItem = true;   //by unsnapping the white disk, the blue is now accessible
                red.gameObject.SetActive(false);
                break;
            case Color.BLUE:    //to unsnap the blue disk, the red and white should be unsnapped
                if (white.isInPlace || red.isInPlace)
                    return;
                white.gameObject.SetActive(false);
                break;
            default: break;
        }
        isInPlace = false; ;
        sz.GrabEquipped(grabber);
    }

}
