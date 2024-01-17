using System.Collections;
using UnityEngine;

public class PlayerStatus
{
    private float foodUseCD;
    private float currentFoodUseCD;

    private float beverageUseCD;
    private float currentBeverageUseCD;

    private float medicineUseCD;
    private float currentMedicineUseCD;

    public PlayerStatus() { }

    public IEnumerator DoItemUseCoolDown(ItemType itemType, float useCD)
    {
        switch (itemType)
        {
            case ItemType.Food:
                foodUseCD = useCD;
                currentFoodUseCD = useCD;
                UIManager.Instance.FoodUseCD.SetActive(true);
                while (currentFoodUseCD > 0)
                {
                    currentFoodUseCD -= Time.deltaTime;
                    UIManager.Instance.UpdateItemUseCDProgress(itemType, currentFoodUseCD, foodUseCD);
                    yield return null;
                }
                UIManager.Instance.FoodUseCD.SetActive(false);
                break;
            case ItemType.medicine:
                medicineUseCD = useCD;
                currentMedicineUseCD = useCD;
                UIManager.Instance.PillUseCD.SetActive(true);
                while (currentMedicineUseCD > 0)
                {
                    currentMedicineUseCD -= Time.deltaTime;
                    UIManager.Instance.UpdateItemUseCDProgress(itemType, currentMedicineUseCD, medicineUseCD);
                    yield return null;
                }
                UIManager.Instance.PillUseCD.SetActive(false);
                break;
            case ItemType.Beverage:
                beverageUseCD = useCD;
                currentBeverageUseCD = useCD;
                UIManager.Instance.BeverageUseCD.SetActive(true);
                while (currentBeverageUseCD > 0)
                {
                    currentBeverageUseCD -= Time.deltaTime;
                    UIManager.Instance.UpdateItemUseCDProgress(itemType, currentBeverageUseCD, beverageUseCD);
                    yield return null;
                }
                UIManager.Instance.BeverageUseCD.SetActive(false);
                break;
        }
    }

    public bool IsItemUseOnCD(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Food:
                return currentFoodUseCD > 0 ? true : false;
            case ItemType.medicine:
                return currentMedicineUseCD > 0 ? true : false;
            case ItemType.Beverage:
                return currentBeverageUseCD > 0 ? true : false;
            default:
                return false;
        }
    }
}