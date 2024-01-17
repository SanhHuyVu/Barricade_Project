using UnityEngine;

// delegate is a reference to a fucntion so we can use it to fire a function
public delegate void SlotUpdated(InventorySlot slot);

public enum IsEquipmentSlot { Null, Helmet, Chest, MainHand, OffHand, Boots }
[System.Serializable]
public class InventorySlot
{
    [SerializeField] private IsEquipmentSlot isEquipmentSlot;
    [SerializeField] private ItemType[] allowedItems = new ItemType[0];

    // using [System.NonSerialized] to prevent saving these field
    [System.NonSerialized] private UserInterface parent; // not saving the userinterface (not saving the InventoryObject inside this class)
    [System.NonSerialized] private GameObject slotDisplay;
    [System.NonSerialized] private SlotUpdated onAfterUpdate;
    [System.NonSerialized] private SlotUpdated onBeforeUpdate;

    [SerializeField] private Item item;
    [SerializeField] private int amount;

    public IsEquipmentSlot IsEquipmentSlot => isEquipmentSlot;
    public int Amount => amount;
    public ItemType[] AllowedItems => allowedItems;
    public Item Item { get { return item; } set { item = value; } }
    public UserInterface Parent { get { return parent; } set { parent = value; } }
    public GameObject SlotDisplay { get { return slotDisplay; } set { slotDisplay = value; } }

    public SlotUpdated OnAfterUpdate { get { return onAfterUpdate; } set { onAfterUpdate = value; } }
    public SlotUpdated OnBeforeUpdate { get { return onBeforeUpdate; } set { onBeforeUpdate = value; } }

    public ItemObject itemObject
    {
        get
        {
            if (item.Id >= 0)
            {
                return parent.Inventory.Database.ItemObjects[item.Id];
            }
            return null;
        }
    }

    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);
    }
    public InventorySlot(Item item, int amount)
    {
        UpdateSlot(item, amount);
    }

    public void UpdateSlot(Item item, int amount)
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);

        this.item = item;
        this.amount = amount;

        if (OnAfterUpdate != null)
            OnAfterUpdate.Invoke(this);
    }
    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }
    public void DropItem()
    {
        if (item.Id < 0) return;
        parent.Inventory.Database.ItemObjects[item.Id].SpawnGroundItem(Player.Instance.transform.position, amount);
        RemoveItem();
    }
    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);
    }

    public bool CanPlaceInSlot(ItemObject itemObject)
    {
        if (allowedItems.Length <= 0 || itemObject == null || itemObject.Data.Id < 0)
            return true;

        for (int i = 0; i < allowedItems.Length; i++)
        {
            if (itemObject.Type == allowedItems[i])
                return true;
        }
        return false;
    }
}

