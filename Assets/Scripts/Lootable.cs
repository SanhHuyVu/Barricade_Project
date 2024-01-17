using System.Collections.Generic;
using UnityEngine;

public class Lootable : Interactable, IDataPersistence
{
    [SerializeField] private string id;
    [SerializeField] private bool test = false;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] private InventoryObject lootableInventory;
    [SerializeField] private ItemObject[] lootList;
    [SerializeField] private int minLoots = 0;
    [SerializeField] private int maxLoots = 5;
    [SerializeField] private float searchTime = 3f;

    private bool alreadySearched = false;

    public float SearchTime => searchTime;

    private void Start()
    {
        TestItems();
        if (DataPersistenceManager.Instance.IsNewGame)
        {
            GenerateLoot();
        }
    }

    public override void Interact()
    {
        if (alreadySearched)
            InteractableInventoryInterface.Instance.SetInventory(lootableInventory);
        else if (!UIManager.Instance.DoingAction)
            StartCoroutine(UIManager.Instance.DoAction(this, UIManager.ActionType.Searching));
    }

    public void DoSetInventory()
    {
        // Debug.Log($"Interacted with: {gameObject.name}");
        alreadySearched = true;
        InteractableInventoryInterface.Instance.SetInventory(lootableInventory);
        selectedInteractableVisual?.Show();
    }

    public override void ShowSelectedVisual()
    {
        if (alreadySearched)
        {
            UIManager.Instance.InteractText.text = "<color=green>'E'</color> Open";
            selectedInteractableVisual?.Show();
        }
        else
        {
            UIManager.Instance.InteractText.text = "<color=yellow>'E'</color> Search";
            selectedInteractableVisual?.Show();
        }
    }
    public override void HideSelectedVisual()
    {
        UIManager.Instance.InteractText.text = "";
        selectedInteractableVisual?.Hide();
    }

    private void GenerateLoot()
    {
        lootableInventory.Clear();

        ItemObject[] possibleLoots = GetPossibleLoots();

        if (possibleLoots == null) return;

        int randomNumOfLoots = Random.Range(minLoots, maxLoots);

        // if possible loot number is geater than random loot number then take random loot number and via versa
        bool doRandom = possibleLoots.Length > randomNumOfLoots ? true : false;

        if (doRandom)
        {
            for (int i = 0; i < randomNumOfLoots; i++)
            {
                lootableInventory.AddItem(possibleLoots[Random.Range(0, possibleLoots.Length)].Data, 1);
            }
        }
        else
        {
            for (int i = 0; i < possibleLoots.Length; i++)
            {
                lootableInventory.AddItem(possibleLoots[i].Data, 1);
            }
        }
    }

    private ItemObject[] GetPossibleLoots()
    {
        List<ItemObject> possibleLoots = new List<ItemObject>();

        // try get possible loot the first time
        int randomDropRare = Random.Range(1, 101);
        foreach (ItemObject itemObject in lootList)
        {
            if (randomDropRare <= itemObject.DropChance) possibleLoots.Add(itemObject);
        }

        if (possibleLoots.Count > 0) return possibleLoots.ToArray();
        else
        {
            // try get possible loot twice
            randomDropRare = Random.Range(1, 101);
            foreach (ItemObject itemObject in lootList)
            {
                if (randomDropRare <= itemObject.DropChance) possibleLoots.Add(itemObject);
            }

            if (possibleLoots.Count > 0) return possibleLoots.ToArray();
            else return null;
        }
    }

    private void TestItems()
    {
        if (!test) return;
        lootableInventory.Clear();
        transform.position = Player.Instance.PlayerTransform.position;
        var db = lootableInventory.Database;
        for (int i = 0; i < lootableInventory.GetSlots.Length; i++)
        {
            if (i >= db.ItemObjects.Length) return;
            lootableInventory.GetSlots[i].UpdateSlot(db.ItemObjects[i].Data, 1);
        }
    }

    public void LoadData(GameData data)
    {
        lootableInventory.Clear();

        data.LootableInventories.TryGetValue(id, out LootableData lootableData);
        if (lootableData != null)
        {
            alreadySearched = lootableData.AlreadySearched;
            lootableInventory.LoadContainer(lootableData.Container);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.LootableInventories.ContainsKey(id))
        {
            data.LootableInventories.Remove(id);
        }
        data.LootableInventories.Add(id, new LootableData(alreadySearched, lootableInventory.Container));
    }
}
