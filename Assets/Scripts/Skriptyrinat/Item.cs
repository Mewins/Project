using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        WEAPON,
        AMMO,
        HEAL,
    }

    public ItemType itemType;
    public bool isSnapped;

    public InventorySlot hoveredInventorySlot;

    public void Start()
    {
        isSnapped = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isSnapped)
        {
            return;
        }

        InventorySlot inventorySlot = other.transform.GetComponent<InventorySlot>();
        if (inventorySlot)
        {
            hoveredInventorySlot = inventorySlot;
            hoveredInventorySlot.TogglePlaceholderMesh(itemType, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag != "Bullet")
        {
            if (isSnapped)
            {
                return;
            }

            InventorySlot inventorySlot = other.transform.GetComponent<InventorySlot>();
            if (hoveredInventorySlot == inventorySlot)
            {
                hoveredInventorySlot.TogglePlaceholderMesh(itemType, false);
                hoveredInventorySlot = null;
            }
        }
    }

    public void OnPickup()
    {
        if (isSnapped)
        {
            hoveredInventorySlot.TogglePlaceholderMesh(itemType, false);
            DetachFromInventorySlot();
        }
        else
        {
            hoveredInventorySlot = null;
        }
    }

    public void OnDrop()
    {
        if (hoveredInventorySlot != null && !hoveredInventorySlot.isOccupied && hoveredInventorySlot.supportedItemTypes.Contains(itemType))
        {
            hoveredInventorySlot.TogglePlaceholderMesh(itemType, false);
            AttachToInventorySlot();
        }
        else
        {
            hoveredInventorySlot = null;
        }
    }

    public void DetachFromInventorySlot()
    {
        if (hoveredInventorySlot != null)
        {
            hoveredInventorySlot.isOccupied = false;
            hoveredInventorySlot = null;
        }
        isSnapped = false;
        EnablePhysics();
        //transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void AttachToInventorySlot()
    {
        isSnapped = true;
        hoveredInventorySlot.isOccupied = true;
        int indexOfPosition = hoveredInventorySlot.supportedItemTypes.IndexOf(itemType);
        Transform slotTransform = hoveredInventorySlot.itemPositions[indexOfPosition];
        transform.parent = hoveredInventorySlot.transform;
        transform.position = slotTransform.position;
        transform.rotation = slotTransform.rotation;

        //transform.localScale = new Vector3(0.1f,0.1f,0.1f);

        DisablePhysics();
    }

    public void EnablePhysics()
    {
        Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.None;
    }

    public void DisablePhysics()
    {
        Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
