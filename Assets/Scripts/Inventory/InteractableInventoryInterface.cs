using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableInventoryInterface : UserInterface
{
    public static InteractableInventoryInterface Instance { get; private set; }
    [SerializeField] private GameObject slotPrefab;
    private void Awake()
    {
        Instance = this;
        AddEventToInteface();
        gameObject.SetActive(false);
    }

    public override void CreateSlots()
    {
        slotOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = Instantiate(slotPrefab, Vector3.zero, quaternion.identity, transform);

            AddEvent(obj, EventTriggerType.PointerDown, delegate { OnClicked(obj); });
            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            inventory.GetSlots[i].SlotDisplay = obj;

            slotOnInterface.Add(obj, inventory.GetSlots[i]);

            UpdateDisplay(inventory.GetSlots[i], inventory.GetSlots[i].itemObject);
        }
    }

    private void UpdateDisplay(InventorySlot slot, ItemObject itemObject)
    {
        GameObject slotDisplay = slot.SlotDisplay;
        if (itemObject == null) return;
        slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = itemObject.UiDisplay;
        slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
        slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = slot.Amount > 1 ? slot.Amount.ToString() : "";

    }

    public void SetInventory(InventoryObject inventoryToSet)
    {
        if (inventory != null) return;
        gameObject.SetActive(true);
        inventory = inventoryToSet;
        ReinitInventory();
        CreateSlots();

        if (!UIManager.Instance.IsInventoryMenusOpen)
            UIManager.Instance.ToggleInventoryMenu();
    }
    public void UnSetInventory()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        inventory = null;
        gameObject.SetActive(false);
    }
}
