using UnityEngine;

public enum InterfaceType { Inventory, Equipment, Hotbar, Crafting, Lootable }
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    [SerializeField] private ItemDatabaseObject database;
    [SerializeField] private InterfaceType type;
    [SerializeField] private Inventory container;

    public Inventory Container => container;
    public ItemDatabaseObject Database => database;
    public InterfaceType Type => type;
    public InventorySlot[] GetSlots { get { return container.Slots; } }

    public bool AddItem(Item item, int amount)
    {
        //  Check if a stack already exists.
        InventorySlot slot = FindItemInInventory(item);
        if (database.ItemObjects[item.Id].Stackable && slot != null)
        {
            // add amount to that existing stack
            slot.AddAmount(amount);
            return true;
        }
        if (EmptySlotCount > 0)
        {
            //Place in first empty slot.
            SetEmptySlot(item, amount);
            return true;
        }
        else
        {
            //No way to add item.
            return false;
        }
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].Item.Id <= -1)
                    counter++;
            }
            return counter;
        }
    }

    public InventorySlot FindItemInInventory(Item item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].Item.Id == item.Id)
            {
                return GetSlots[i];
            }
        }
        return null;
    }
    public InventorySlot SetEmptySlot(Item item, int amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].Item.Id <= -1)
            {
                GetSlots[i].UpdateSlot(item, amount);
                return GetSlots[i];
            }
        }

        // set up func for inven is full
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item1.itemObject == item2.itemObject && item1.itemObject.Stackable)
        {
            item2.AddAmount(item1.Amount);
            item1.RemoveItem();
            return;
        }
        if (item2.CanPlaceInSlot(item1.itemObject) && item1.CanPlaceInSlot(item2.itemObject))
        {
            InventorySlot temp = new InventorySlot(item2.Item, item2.Amount);
            item2.UpdateSlot(item1.Item, item1.Amount);
            item1.UpdateSlot(temp.Item, temp.Amount);
        }
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].Item == item)
            {
                GetSlots[i].UpdateSlot(null, 0);
            }
        }
    }

    public bool HasThisItemObject(ItemObject itemObject)
    {
        foreach (InventorySlot slot in Container.Slots)
        {
            if (slot.itemObject != null && slot.itemObject == itemObject)
                return true;
        }
        return false;
    }

    public void LoadContainer(Inventory containerToLoad)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            Container.Slots[i].UpdateSlot(containerToLoad.Slots[i].Item, containerToLoad.Slots[i].Amount);
        }
    }

    // old saving system
    // public void Save()
    // {
    //     //string saveData = JsonUtility.ToJson(this, true);
    //     //BinaryFormatter bf = new BinaryFormatter();
    //     //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
    //     //bf.Serialize(file, saveData);
    //     //file.Close();

    //     IFormatter formatter = new BinaryFormatter();
    //     Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
    //     formatter.Serialize(stream, Container);
    //     stream.Close();
    // }

    // old loading system
    // public void Load()
    // {
    //     if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
    //     {
    //         //BinaryFormatter bf = new BinaryFormatter();
    //         //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
    //         //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
    //         //file.Close();

    //         IFormatter formatter = new BinaryFormatter();
    //         Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
    //         Inventory newContainer = (Inventory)formatter.Deserialize(stream);
    //         for (int i = 0; i < GetSlots.Length; i++)
    //         {
    //             GetSlots[i].UpdateSlot(newContainer.Slots[i].Item, newContainer.Slots[i].Amount);
    //         }
    //         stream.Close();
    //     }
    // }

    [ContextMenu("Clear")]
    public void Clear()
    {
        container.Clear();
    }
}


