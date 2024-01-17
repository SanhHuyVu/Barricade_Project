using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private InventorySlot[] slots = new InventorySlot[20];

    public InventorySlot[] Slots => slots;

    public void CreateNewSlots(int numOfSlot)
    {
        slots = new InventorySlot[numOfSlot];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    public void CreateNewSlots(int numOfSlot, List<ItemObject> craftableItems)
    {
        slots = new InventorySlot[numOfSlot];
        for (int i = 0; i < craftableItems.Count; i++)
        {
            slots[i] = new InventorySlot();
            slots[i].UpdateSlot(craftableItems[i].Data, 1);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
        }
    }
}

