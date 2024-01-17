using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UserInterface : MonoBehaviour
{
    [SerializeField] protected InventoryObject inventory;
    protected Dictionary<GameObject, InventorySlot> slotOnInterface = new Dictionary<GameObject, InventorySlot>();

    public InventoryObject Inventory => inventory;

    protected void InitInventory()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].Parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }

        CreateSlots();

        AddEventToInteface();
    }

    protected void AddEventToInteface()
    {
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    protected void ReinitInventory()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            // if (inventory.GetSlots[i] == null) continue;
            inventory.GetSlots[i].Parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }
    }

    private void OnSlotUpdate(InventorySlot slot)
    {
        if (slot.Item.Id >= 0)
        {
            // slot has item in it 
            if (slot.SlotDisplay == null) return;
            slot.SlotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = slot.itemObject.UiDisplay;
            slot.SlotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            slot.SlotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = slot.Amount == 1 ? "" : slot.Amount.ToString("n0");
        }
        else
        {
            // slot does not has item in it 
            if (slot.SlotDisplay == null) return;
            slot.SlotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            slot.SlotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            slot.SlotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    protected void OnClicked(GameObject obj)
    {
        if (slotOnInterface[obj].Item.Id >= 0)
        {
            CraftingInterface.Instance.UpdateSelectedItemObject(slotOnInterface[obj].itemObject);
        }
        if (this is HotbarInterface hotbarInterface)
        {
            hotbarInterface.UpdateSelectedSlot(hotbarInterface.GetIndexFromGameObject(obj));
        }
    }

    protected void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
    }
    protected void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }
    protected void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }
    protected void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }
    protected void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }
    private GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotOnInterface[obj].Item.Id >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            float imgSize = 50;
            rt.sizeDelta = new Vector2(imgSize, imgSize);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotOnInterface[obj].itemObject.UiDisplay;
            img.raycastTarget = false;
        }
        return tempItem;
    }
    protected void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        if (MouseData.interfaceMouseIsOver == null)
        {
            // drop item
            slotOnInterface[obj].DropItem();
            HotbarInterface.Instance.UpdateSelectedSlot();
            return;
        }
        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotOnInterface[MouseData.slotHoveredOver];
            if (slotOnInterface[obj].Item.Id >= 0)
            {
                inventory.SwapItems(slotOnInterface[obj], mouseHoverSlotData);
            }
        }
        if (this is HotbarInterface hotbarInterface)
        {
            hotbarInterface.UpdateSelectedSlot(hotbarInterface.GetIndexFromGameObject(MouseData.slotHoveredOver));
        }
        HotbarInterface.Instance.UpdateSelectedSlot();
    }
    protected void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }

}
