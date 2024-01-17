using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private string name;
    [SerializeField] private int id = -1;
    [SerializeField] private ItemBuff[] buffs;
    public Item()
    {
        name = "";
        id = -1;
    }
    public Item(ItemObject item)
    {
        name = item.name;
        id = item.Data.Id;
        buffs = new ItemBuff[item.Data.Buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.Data.Buffs[i].Attribute, item.Data.Buffs[i].Value);
        }
    }

    public int Id { get { return id; } set { id = value; } }
    public string Name => name;
    public ItemBuff[] Buffs => buffs;
}
