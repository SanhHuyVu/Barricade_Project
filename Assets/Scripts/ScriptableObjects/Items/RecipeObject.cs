using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/recipe")]

public class RecipeObject : ScriptableObject
{
    [SerializeField] private ItemObject itemObject;
    [SerializeField] ItemObject[] requiredComponents;
    [SerializeField] private int craftAmount = 1;
    [SerializeField] private float hungerCost;
    [SerializeField] private float spCost;

    public ItemObject CraftableItemObject => itemObject;
    public ItemObject[] RequiredComponents => requiredComponents;
    public int CraftAmount => craftAmount;
    public float HungerCost => hungerCost;
    public float SPCost => spCost;

    public bool RequiredThisComponent(ItemObject component)
    {
        for (int i = 0; i < requiredComponents.Length; i++)
        {
            if (component == requiredComponents[i]) return true;
        }
        return false;
    }
}
