using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarInterface : UserInterface
{
    public static HotbarInterface Instance { get; private set; }

    [SerializeField] private GameObject[] slots;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;

    public int SelectedSlotIndex { get; private set; } = 0;
    private InventorySlot selectedSlot;
    private Image previousSlotBG;
    private Player playerInstance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitInventory();
        UpdateSelectedSlot(0);

        playerInstance = Player.Instance;
        GameInput.Instance.OnUseItemAction += GameInput_OnUseItemAction;
    }

    private void GameInput_OnUseItemAction(object sender, EventArgs e)
    {
        if (selectedSlot == null || selectedSlot.itemObject == null) return;

        if (selectedSlot.itemObject.IsConsumable)
        {
            DoUseItem(selectedSlot.itemObject);
        }
        else if (selectedSlot.itemObject.IsEquipment())
        {
            DoEquipItem();
        }
        else if (selectedSlot.itemObject.Type == ItemType.Trap)
        {
            PlaceTrap();
        }
    }

    private void PlaceTrap()
    {
        if (Player.Instance.TrapBlueprintPoint.PlaceTrap())
        {
            if (selectedSlot.Amount <= 1)
            {
                selectedSlot.RemoveItem();
                Player.Instance.TrapBlueprintPoint.SetTrapBlueprint(null, -1);
            }
            else
            {
                selectedSlot.AddAmount(-1);
                UIManager.Instance.UseItemText.text = "";
                UIManager.Instance.UseItemText.gameObject.SetActive(false);
            }
        }
    }

    private void DoUseItem(ItemObject itemObject)
    {
        if (!playerInstance.PlayerStatus.IsItemUseOnCD(itemObject.Type))
        {
            bool itemUsed = false;
            foreach (ItemBuff buff in itemObject.Data.Buffs)
            {
                switch (buff.Attribute)
                {
                    case Attributes.HPRecoverAmount:
                        playerInstance.Heal(buff.Value);
                        itemUsed = true;
                        Player.Instance.ScoreTracker.HPRecoverAmount += buff.Value;
                        break;
                    case Attributes.SPRecoverAmount:
                        playerInstance.UpdateSP(buff.Value, true);
                        UIManager.Instance.UpdateVisualStamina(playerInstance.SP);
                        itemUsed = true;
                        Player.Instance.ScoreTracker.StaminaRecoverAmount += buff.Value;
                        break;
                    case Attributes.HungerRecoverAmount:
                        playerInstance.UpdateHunger(buff.Value, true);
                        UIManager.Instance.UpdateVisualHunger(playerInstance.Hunger);
                        itemUsed = true;
                        Player.Instance.ScoreTracker.HungerRecoverAmount += buff.Value;
                        break;
                }
            }

            if (itemUsed)
            {
                switch (itemObject.Type)
                {
                    case ItemType.Food:
                        Player.Instance.ScoreTracker.FoodConsumed++;
                        break;
                    case ItemType.medicine:
                        Player.Instance.ScoreTracker.MedicineConsumed++;
                        break;
                    case ItemType.Beverage:
                        Player.Instance.ScoreTracker.BeverageConsumed++;
                        break;
                }

                if (selectedSlot.Amount <= 1) selectedSlot.RemoveItem();
                else
                {
                    selectedSlot.AddAmount(-1);
                    UIManager.Instance.UseItemText.text = "";
                    UIManager.Instance.UseItemText.gameObject.SetActive(false);
                }
                StartCoroutine(playerInstance.PlayerStatus.DoItemUseCoolDown(itemObject.Type, itemObject.UseCoolDown));
            }
        }
    }

    private void DoEquipItem()
    {
        var itemObj = selectedSlot.itemObject;
        var equipmentSlots = EquipmentInterface.Instance.Inventory.GetSlots;
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].CanPlaceInSlot(itemObj))
            {
                inventory.SwapItems(selectedSlot, equipmentSlots[i]);
                return;
            }
        }
    }

    public override void CreateSlots()
    {
        slotOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = slots[i];

            AddEvent(obj, EventTriggerType.PointerDown, delegate { OnClicked(obj); });
            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            inventory.GetSlots[i].SlotDisplay = obj;

            slotOnInterface.Add(obj, inventory.GetSlots[i]);
        }
    }

    public void UpdateSelectedSlot(int slotIndex)
    {
        if (previousSlotBG != null) previousSlotBG.color = defaultColor;

        if (slotIndex > inventory.GetSlots.Length - 1) slotIndex = 0;
        if (slotIndex < 0) slotIndex = inventory.GetSlots.Length - 1;

        SelectedSlotIndex = slotIndex;
        selectedSlot = inventory.GetSlots[SelectedSlotIndex];
        previousSlotBG = slots[SelectedSlotIndex].GetComponent<Image>();
        previousSlotBG.color = selectedColor;

        DoUpdateSelectedSlot();
    }

    public void UpdateSelectedSlot()
    {
        if (selectedSlot != null)
        {
            DoUpdateSelectedSlot();
        }
    }

    private void DoUpdateSelectedSlot()
    {
        if (Player.Instance.TrapBlueprintPoint.trapGO != null) Player.Instance.TrapBlueprintPoint.SetTrapBlueprint(null, -1);

        if (selectedSlot.itemObject == null)
        {
            UIManager.Instance.UseItemText.text = "";
            UIManager.Instance.UseItemText.gameObject.SetActive(false);
        }
        else if (selectedSlot.itemObject.IsConsumable)
        {
            UIManager.Instance.UseItemText.gameObject.SetActive(true);
            UIManager.Instance.UseItemText.text = "<color=blue>'R'</color> Use Item";
        }
        else if (selectedSlot.itemObject.IsEquipment())
        {
            UIManager.Instance.UseItemText.gameObject.SetActive(true);
            UIManager.Instance.UseItemText.text = "<color=blue>'R'</color> Equip Item";
        }
        else if (selectedSlot.itemObject.Type == ItemType.Trap)
        {
            UIManager.Instance.UseItemText.gameObject.SetActive(true);
            UIManager.Instance.UseItemText.text = "<color=blue>'R'</color> Place Trap";
            Player.Instance.TrapBlueprintPoint.SetTrapBlueprint(selectedSlot.itemObject.TrapBlueprint, selectedSlot.itemObject.Data.Id);
        }
    }

    public int GetIndexFromGameObject(GameObject obj)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == obj) return i;
        }
        return -1;
    }

    private void OnDestroy() {
        GameInput.Instance.OnUseItemAction -= GameInput_OnUseItemAction;
    }
}
