using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int amount;

    public InventorySlot(ItemData item, int value)
    {
        itemData = item;
        amount = value;
    }
}