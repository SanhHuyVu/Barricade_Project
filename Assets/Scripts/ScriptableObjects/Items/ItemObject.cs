using Unity.Mathematics;
using UnityEngine;

public enum ItemType { Food, HeadGear, Spear, Sword, Axe, Dagger, ArmGear, LegGear, ChestGear, Material, medicine, Beverage, Trap }
public enum Attributes
{
    Attack,
    Defense,
    MaxHP,
    FlatDamageReduction,
    HPRecoverAmount,
    SPRecoverAmount,
    HungerRecoverAmount,
    HPCost,
    SPCost,
    HungerCost,
    MoveSpeed,
    RunSpeed
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class ItemObject : ScriptableObject
{
    [SerializeField] private Sprite uiDisplay;
    [SerializeField] private bool stackable;
    [SerializeField] private bool isConsumable;
    [SerializeField] private ItemType type;
    [SerializeField] private GameObject model;
    [SerializeField] private TrapBlueprint trapBlueprint;
    [SerializeField] private Vector3 modelPosition;
    [SerializeField] private Vector3 modelRotation;
    [SerializeField] private GroundItem groundItemPrefab;
    [SerializeField] private float craftingTime;
    [SerializeField] private float useCoolDown;
    [SerializeField] private float useTime;

    [SerializeField][Range(0, 100)] private int dropChance = 50;

    [TextArea(6, 10)]
    [SerializeField] private string description;
    [SerializeField] private Item data = new Item();

    public Sprite UiDisplay => uiDisplay;
    public bool Stackable => stackable;
    public bool IsConsumable => isConsumable;
    public ItemType Type => type;
    public GameObject Model => model;
    public TrapBlueprint TrapBlueprint => trapBlueprint;
    public Vector3 ModelPosition => modelPosition;
    public Vector3 ModelRotation => modelRotation;
    public string Description => description;
    public Item Data => data;
    public float CraftingTime => craftingTime;
    public float UseCoolDown => useCoolDown;
    public float UseTime => useTime;
    public int DropChance => dropChance;
    public Item CreateItem() { return new Item(this); }

    public void SpawnGroundItem(Vector3 position, int amount = 1)
    {
        var groundItem = Instantiate(groundItemPrefab, 
        new Vector3(position.x, position.y + 0.5f, position.z), quaternion.identity);
        
        GroundItemManager.Instance.AddGroundItemTOlist(groundItem);
        groundItem.Item = this;
        groundItem.Amount = amount;
        groundItem.UpdateSprite();
    }
    public bool IsWeapon()
    {
        switch (type)
        {
            case ItemType.Spear:
                return true;
            case ItemType.Sword:
                return true;
            case ItemType.Axe:
                return true;
            case ItemType.Dagger:
                return true;
            default:
                return false;
        }
    }
    public bool IsEquipment()
    {
        switch (type)
        {
            case ItemType.HeadGear:
                return true;
            case ItemType.Spear:
                return true;
            case ItemType.Sword:
                return true;
            case ItemType.Axe:
                return true;
            case ItemType.ArmGear:
                return true;
            case ItemType.LegGear:
                return true;
            case ItemType.ChestGear:
                return true;
            default:
                return false;
        }
    }
}
