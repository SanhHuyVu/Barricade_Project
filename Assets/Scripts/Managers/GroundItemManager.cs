using System.Collections.Generic;
using UnityEngine;

public class GroundItemManager : MonoBehaviour, IDataPersistence
{
    public static GroundItemManager Instance { get; private set; }
    [SerializeField] private ItemDatabaseObject database;
    private List<GroundItem> groundItemList;
    private List<Trap> trapList;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        groundItemList = new List<GroundItem>();
        trapList = new List<Trap>();
    }

    public void AddGroundItemTOlist(GroundItem groundItem)
    {
        groundItemList.Add(groundItem);
    }
    public void AddTrapToList(Trap trap)
    {
        trapList.Add(trap);
    }
    public void RemoveGroundItemFromList(GroundItem groundItem)
    {
        for (int i = groundItemList.Count - 1; i >= 0; i--)
        {
            if (groundItemList[i] == groundItem)
            {
                groundItemList.RemoveAt(i);
                return;
            }
        }
    }
    public void RemoveTrapFromList(Trap trap)
    {
        for (int i = trapList.Count - 1; i >= 0; i--)
        {
            if (trapList[i] == trap)
            {
                trapList.RemoveAt(i);
                return;
            }
        }
    }

    public void LoadData(GameData data)
    {
        // loading the groundItems
        GroundItem[] tempGroundItemList = groundItemList.ToArray();
        groundItemList.Clear();
        foreach (GroundItem groundItem in tempGroundItemList)
        {
            Destroy(groundItem.gameObject);
        }
        // foreach (KeyValuePair<int, GroundItemData> groundItemInfo in data.groundItemInfos)
        foreach (GroundItemData groundItemData in data.groundItemInfos)
        {
            ItemObject itemObject = database.GetItemObjectWithId(groundItemData.ItemObjectId);
            itemObject.SpawnGroundItem(groundItemData.Position, groundItemData.Amount);
        }

        // loading the traps
        Trap[] tempTrapList = trapList.ToArray();
        trapList.Clear();
        foreach (Trap trap in tempTrapList)
        {
            Destroy(trap.gameObject);
        }
        foreach (TrapData trapData in data.trapInfos)
        {
            ItemObject itemObject = database.GetItemObjectWithId(trapData.TrapID);
            itemObject.TrapBlueprint.SetUpTrap(trapData.TrapID, trapData.Triggered);
            itemObject.TrapBlueprint.PlaceTrap(trapData.Position, trapData.ForwardVector);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.groundItemInfos.Clear();
        data.trapInfos.Clear();
        foreach (GroundItem groundItem in groundItemList)
        {
            data.groundItemInfos.Add(new GroundItemData(groundItem.Item.Data.Id, groundItem.transform.position, groundItem.Amount));
        }
        foreach (Trap trap in trapList)
        {
            data.trapInfos.Add(new TrapData(trap.TrapTF.position, trap.TrapTF.forward, trap.TrapID, trap.Triggered));
        }
    }
}
