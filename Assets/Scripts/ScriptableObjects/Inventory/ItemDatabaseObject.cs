using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    // an array of items exist within the game
    [SerializeField] private ItemObject[] itemObjects;
    [SerializeField] private RecipeObject[] recipes;

    public ItemObject[] ItemObjects => itemObjects;
    public RecipeObject[] Recipes => recipes;

    public ItemObject GetItemObjectWithId(int id)
    {
        for (int i = 0; i < itemObjects.Length; i++)
        {
            if (itemObjects[i].Data.Id == id) return itemObjects[i];
        }
        return null;
    }

    [ContextMenu("Update IDs")]
    public void UpdateID()
    {
        for (int i = 0; i < itemObjects.Length; i++)
        {
            if (itemObjects[i].Data.Id != i)
            {
                itemObjects[i].Data.Id = i;
            }
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateID();
    }

    public void OnBeforeSerialize()
    {
    }

    public ItemObject[] GetComponentsFromItemObject(ItemObject itemObject)
    {
        foreach (RecipeObject recipe in recipes)
        {
            if (recipe.CraftableItemObject == itemObject) return recipe.RequiredComponents;
        }
        return null;
    }

    public RecipeObject GetRecipeObject(ItemObject itemObject)
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i].CraftableItemObject == itemObject) return recipes[i];
        }
        return null;
    }

    public int GetCraftAmount(ItemObject itemObject)
    {
        foreach (RecipeObject recipe in recipes)
        {
            if (recipe.CraftableItemObject == itemObject) return recipe.CraftAmount;
        }
        return 1;
    }
}
