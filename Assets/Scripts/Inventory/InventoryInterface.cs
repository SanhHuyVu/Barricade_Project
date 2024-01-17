using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryInterface : UserInterface
{

    [SerializeField] private GameObject slotPrefab;

    private void Start()
    {
        InitInventory();
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
        }
    }
}
