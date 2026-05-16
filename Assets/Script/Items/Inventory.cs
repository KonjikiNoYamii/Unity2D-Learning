using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();

    public void AddItem(ItemData item, int amount)
    {
        InventorySlot slot = FindStackableSlot(item);

        if (slot != null)
        {
            slot.amount += amount;

            Debug.Log(item.itemName + " stack menjadi " + slot.amount);

            return;
        }

        InventorySlot newSlot = new InventorySlot(item, amount);

        slots.Add(newSlot);

        Debug.Log("Item baru masuk Inventory: " + item.itemName);
    }

    InventorySlot FindStackableSlot(ItemData item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemData == item && item.stackable)
            {
                return slot;
            }
        }

        return null;
    }
        
}