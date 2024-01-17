using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentInterface : UserInterface
{
    public static EquipmentInterface Instance { get; private set; }
    [SerializeField] private GameObject[] Slots;


    private void Awake()
    {
        Instance = this;
        InitInventory();
    }

    public override void CreateSlots()
    {
        slotOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = Slots[i];

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
}
