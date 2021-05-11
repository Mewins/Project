using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public List<Item.ItemType> supportedItemTypes;
    public List<Transform> itemPositions;
    public List<MeshRenderer> placeholderMeshes;
    public bool isOccupied;

    void Start()
    {
        isOccupied = false;
        placeholderMeshes = new List<MeshRenderer>();
        foreach (Transform mesh in itemPositions)
        {
            MeshRenderer placeholderMesh = mesh.GetComponent<MeshRenderer>();
            placeholderMeshes.Add(placeholderMesh);
            placeholderMesh.enabled = false;
        }
    }

    public void TogglePlaceholderMesh(Item.ItemType itemType, bool value)
    {
        if (supportedItemTypes.Contains(itemType))
        {
            int index = supportedItemTypes.IndexOf(itemType);
            placeholderMeshes[index].enabled = value;
        }
    }

}
