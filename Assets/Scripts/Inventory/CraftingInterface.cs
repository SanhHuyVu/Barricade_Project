using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingInterface : UserInterface
{
    public static CraftingInterface Instance { get; private set; }

    [Header("Item Info")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI craftAmountText;
    [SerializeField] private TextMeshProUGUI craftThisItemText;

    [Header("Slot prefab")]
    [SerializeField] private GameObject slotPrefab;

    [Header("Item Stats")]
    [SerializeField] private TextMeshProUGUI itemStatPrefab;
    [SerializeField] private GameObject itemStatContent;

    [Header("Craftable items")]
    [SerializeField] private GameObject craftableItemsContent;

    [Header("Required items")]
    [SerializeField] private GameObject displayRequiredComponents;

    [Header("Craft Btn")]
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI craftBtnText;

    [Header("Progress bar")]
    [SerializeField] private GameObject progressBar;
    [SerializeField] private Image progress;

    [Header("Stat colors")]
    [SerializeField] private Color debuffColor;
    [SerializeField] private Color buffColor;

    private ItemObject selectedItemObject;
    private RecipeObject recipeOfSelectedItemObject;
    private int numOfMaterial = 0;
    private bool crafting = false;
    private float currentCraftIngProgress;

    private void Awake()
    {
        Instance = this;
        nameText.text = "";
        descriptionText.text = "";
        craftThisItemText.text = "";
        if (selectedItemObject == null)
        {
            gameObject.SetActive(false);
            itemStatContent.SetActive(false);
        }

        AddEvent(craftButton.gameObject, EventTriggerType.PointerDown, delegate { CraftButtonOnClicked(); });
        AddEventToInteface();
        progressBar.SetActive(false);
        craftBtnText.text = "Craft";
        craftBtnText.color = Color.white;
    }
    private void Start()
    {
        Player.Instance.OnTakeDamage += CancelCrafting;
    }

    public override void CreateSlots() { }

    public void UpdateSelectedItemObject(ItemObject itemObject)
    {
        if (selectedItemObject == null)
            gameObject.SetActive(true);

        selectedItemObject = itemObject;
        recipeOfSelectedItemObject = inventory.Database.GetRecipeObject(selectedItemObject);
        crafting = false;

        DisplayItemStatInfos();
        DisplayPosibleItemRecipe();
        ReverseOrderOfChildren(craftableItemsContent.transform);

        icon.sprite = selectedItemObject.UiDisplay;
        if (recipeOfSelectedItemObject != null)
        {
            int craftAmount = recipeOfSelectedItemObject.CraftAmount;
            if (craftAmount > 1) craftAmountText.text = craftAmount.ToString();
            else craftAmountText.text = "";
        }
        else craftAmountText.text = "";
        nameText.text = selectedItemObject.Data.Name;
        descriptionText.text = selectedItemObject.Description;
    }

    private void DisplayPosibleItemRecipe()
    {
        foreach (Transform child in craftableItemsContent.transform)
            Destroy(child.gameObject);
        foreach (Transform child in displayRequiredComponents.transform)
            Destroy(child.gameObject);

        // RecipeObject recipeOfSelectedItemObject = inventory.Database.Recipes.FirstOrDefault(obj => obj.CraftableItemObject == selectedItemObject);

        numOfMaterial = recipeOfSelectedItemObject != null ? recipeOfSelectedItemObject.RequiredComponents.Length : 0;

        int numOfSlotPrefab = numOfMaterial;
        List<ItemObject> listOfObjToDisplay = recipeOfSelectedItemObject != null ? recipeOfSelectedItemObject.RequiredComponents.ToList<ItemObject>() : new List<ItemObject>();

        slotOnInterface = new Dictionary<GameObject, InventorySlot>();

        // check if the selecteditemObject can be crafted into anything
        foreach (RecipeObject recipe in inventory.Database.Recipes)
        {
            if (recipe.RequiredThisComponent(selectedItemObject))
            {
                numOfSlotPrefab++;
                listOfObjToDisplay.Add(recipe.CraftableItemObject);
            }
        }
        if (numOfSlotPrefab - numOfMaterial > 0)
            craftThisItemText.text = $"Can be crafted into";
        else
            craftThisItemText.text = "";


        // re-create the craftingMenu's inventory
        inventory.Container.CreateNewSlots(numOfSlotPrefab, listOfObjToDisplay);

        // re-init the inventory after re-creating the craftingMenu's inventory
        ReinitInventory();

        // creating slots for items that are components for crafting this selectedItemObject
        CreateSlotForMaterials(recipeOfSelectedItemObject);

        // creating slots for items that can be crafted by this selectedItemObject
        CreateSlotForCraftableItems(listOfObjToDisplay);
    }

    private void CreateSlotForMaterials(RecipeObject recipeObj)
    {
        if (recipeObj != null)
        {
            for (int i = 0; i < numOfMaterial; i++)
            {
                var reObj = Instantiate(slotPrefab, Vector3.zero, quaternion.identity, displayRequiredComponents.transform);
                AddEvent(reObj, EventTriggerType.PointerDown, delegate { OnClicked(reObj); });

                inventory.GetSlots[i].SlotDisplay = reObj;

                slotOnInterface.Add(reObj, inventory.GetSlots[i]);

                UpdateDisplay(inventory.GetSlots[i].SlotDisplay, recipeObj.RequiredComponents[i], HasThisItemObject(inventory.GetSlots[i].itemObject));
            }
        }
    }

    private void CreateSlotForCraftableItems(List<ItemObject> listOfObjToDisplay)
    {
        for (int i = numOfMaterial; i < listOfObjToDisplay.Count; i++)
        {
            var obj = Instantiate(slotPrefab, Vector3.zero, quaternion.identity, craftableItemsContent.transform);
            AddEvent(obj, EventTriggerType.PointerDown, delegate { OnClicked(obj); });

            inventory.GetSlots[i].SlotDisplay = obj;

            slotOnInterface.Add(obj, inventory.GetSlots[i]);

            bool craftable = true;
            ItemObject[] recipe = inventory.Database.GetComponentsFromItemObject(inventory.GetSlots[i].itemObject);
            foreach (ItemObject itemObject in recipe)
            {
                if (!HasThisItemObject(itemObject))
                {
                    craftable = false;
                    inventory.GetSlots[i].SlotDisplay.transform.SetAsFirstSibling();
                    break;
                }
            }

            UpdateDisplay(inventory.GetSlots[i].SlotDisplay, listOfObjToDisplay[i], craftable);
        }
    }

    private void UpdateDisplay(GameObject slotDisplay, ItemObject itemObject, bool hasItem = true)
    {
        slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = itemObject.UiDisplay;
        if (hasItem)
            slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
        else
            slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    private void DisplayItemStatInfos()
    {
        if (selectedItemObject.Data.Buffs.Length > 0)
        {
            itemStatContent.SetActive(true);
            foreach (Transform child in itemStatContent.transform)
                Destroy(child.gameObject);

            foreach (ItemBuff buff in selectedItemObject.Data.Buffs)
            {
                switch (buff.Attribute)
                {
                    case Attributes.Attack:
                        CreateStatInfo(buff.Value, "Att", true);
                        break;
                    case Attributes.Defense:
                        CreateStatInfo(buff.Value, "Def", true);
                        break;
                    case Attributes.MaxHP:
                        CreateStatInfo(buff.Value, "maxHP", true);
                        break;

                    case Attributes.FlatDamageReduction:
                        CreateStatInfo(buff.Value, "dmg Red", true);
                        break;
                    case Attributes.HPRecoverAmount:
                        CreateStatInfo(buff.Value, "+ HP", true);
                        break;
                    case Attributes.SPRecoverAmount:
                        CreateStatInfo(buff.Value, "+ Sp", true);
                        break;
                    case Attributes.HungerRecoverAmount:
                        CreateStatInfo(buff.Value, "+ Hunger", true);
                        break;
                    case Attributes.MoveSpeed:
                        CreateStatInfo(buff.Value, "move Spd", buff.Value > 0);
                        break;
                    case Attributes.RunSpeed:
                        CreateStatInfo(buff.Value, "run Spd", buff.Value > 0);
                        break;
                    case Attributes.HPCost:
                        CreateStatInfo(buff.Value, "HP cost", false);
                        break;
                    case Attributes.SPCost:
                        CreateStatInfo(buff.Value, "SP Cost", false);
                        break;
                    case Attributes.HungerCost:
                        CreateStatInfo(buff.Value, "Hunget Cost", false);
                        break;
                }
            }
        }
        else
        {
            itemStatContent.SetActive(false);
        }
    }

    private void CreateStatInfo(int value, string stat, bool beneficial)
    {
        TextMeshProUGUI tempItemStat = Instantiate(itemStatPrefab, Vector3.zero, quaternion.identity, itemStatContent.transform);
        tempItemStat.text = $"{stat}: {value}";
        tempItemStat.color = beneficial ? buffColor : debuffColor;
    }

    private void CraftButtonOnClicked()
    {
        if (crafting)
        {
            craftBtnText.text = "Craft";
            craftBtnText.color = Color.white;
            crafting = false;
            return;
        }

        if (numOfMaterial <= 0) return;

        int count = 0;
        InventorySlot[] slotToDeduct = new InventorySlot[numOfMaterial];

        InventorySlot[] slotsTocheck = Player.Instance.Hotbar.Container.Slots.Concat(Player.Instance.Inventory.Container.Slots).ToArray();

        ItemObject[] itemsToDeduct = inventory.Database.GetComponentsFromItemObject(selectedItemObject);

        foreach (ItemObject itemObject in itemsToDeduct)
        {
            foreach (InventorySlot slot in slotsTocheck)
            {
                if (slot.itemObject == itemObject)
                {
                    slotToDeduct[count] = slot;
                    count++;
                    break;
                }
            }
        }

        if (count < slotToDeduct.Length) return;

        if (selectedItemObject.CraftingTime <= 0) DoCrafting(slotToDeduct);
        else StartCoroutine(Crafting(selectedItemObject.CraftingTime, slotToDeduct));
    }

    private bool HasThisItemObject(ItemObject itemObject)
    {
        if (Player.Instance.Hotbar.HasThisItemObject(itemObject)) return true;
        if (Player.Instance.Inventory.HasThisItemObject(itemObject)) return true;
        return false;
    }
    private void ReverseOrderOfChildren(Transform transform)
    {
        for (var i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(0).SetSiblingIndex(transform.childCount - 1 - i);
        }
    }

    private IEnumerator Crafting(float craftTime, InventorySlot[] slotToDeduct)
    {
        UIManager.Instance.CancleAction(this, EventArgs.Empty);

        crafting = true;
        currentCraftIngProgress = 0;
        progress.fillAmount = 0;
        progressBar.SetActive(true);
        craftBtnText.text = "Cancel";
        craftBtnText.color = Color.red;
        bool isCrafting = true;

        while (isCrafting)
        {
            if (Player.Instance.IsMoving() || !crafting)
            {
                crafting = false;
                currentCraftIngProgress = 0;
                progressBar.SetActive(false);
                craftBtnText.text = "Craft";
                craftBtnText.color = Color.white;
                yield break;
            }

            currentCraftIngProgress += Time.deltaTime;
            progress.fillAmount = currentCraftIngProgress / craftTime;
            yield return null;

            if (currentCraftIngProgress >= craftTime)
            {
                isCrafting = false;

                DoCrafting(slotToDeduct);

                craftBtnText.text = "Craft";
                craftBtnText.color = Color.white;
                crafting = false;
                currentCraftIngProgress = 0;
                progressBar.SetActive(false);
            }
        }
    }

    private void DoCrafting(InventorySlot[] slotToDeduct)
    {
        foreach (InventorySlot slot in slotToDeduct)
        {
            if (slot.Amount <= 1)
            {
                slot.RemoveItem();
            }
            else
            {
                slot.AddAmount(-1);
            }
        }

        // var recipeOfSelectedItemObject = inventory.Database.GetRecipeObject(selectedItemObject);

        if (Player.Instance.Hotbar.AddItem(selectedItemObject.Data, recipeOfSelectedItemObject.CraftAmount))
        {
        }
        else if (Player.Instance.Inventory.AddItem(selectedItemObject.Data, recipeOfSelectedItemObject.CraftAmount))
        {
        }
        else
        {
            inventory.Database.ItemObjects[selectedItemObject.Data.Id].SpawnGroundItem(Player.Instance.PlayerTransform.position, recipeOfSelectedItemObject.CraftAmount);
        }
        Player.Instance.ActionCost(recipeOfSelectedItemObject.HungerCost, recipeOfSelectedItemObject.SPCost, true);
        Player.Instance.ScoreTracker.ItemCrafted++;
        UpdateSelectedItemObject(selectedItemObject);
    }

    public void CancelCrafting(object sender, EventArgs e)
    {
        if (crafting) crafting = false;
    }

    private void OnDestroy()
    {
        Player.Instance.OnTakeDamage -= CancelCrafting;
    }
}
