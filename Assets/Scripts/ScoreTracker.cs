// [System.Serializable]
public class ScoreTracker : IDataPersistence
{
    // scores
    public int KillCount = 0;
    public int ItemCrafted = 0;
    public int FoodConsumed = 0;
    public int BeverageConsumed = 0;
    public int MedicineConsumed = 0;
    public int DamageTaken = 0;
    public int DamageDealt = 0;
    public int HPRecoverAmount = 0;
    public int HungerRecoverAmount = 0;
    public int StaminaRecoverAmount = 0;

    public int GetScore()
    {
        return (KillCount * 5) + (ItemCrafted * 3) + FoodConsumed +
        BeverageConsumed + MedicineConsumed + (DamageDealt * 2) + DamageTaken +
        (HPRecoverAmount + StaminaRecoverAmount + HungerRecoverAmount) * 2;
    }

    public ScoreTracker() { }

    public void LoadData(GameData data)
    {
        KillCount = data.KillCount;
        ItemCrafted = data.ItemCrafted;
        FoodConsumed = data.FoodConsumed;
        BeverageConsumed = data.BeverageConsumed;
        MedicineConsumed = data.MedicineConsumed;
        DamageTaken = data.DamageTaken;
        DamageDealt = data.DamageDealt;
        HPRecoverAmount = data.HPRecoverAmount;
        HungerRecoverAmount = data.HungerRecoverAmount;
        StaminaRecoverAmount = data.StaminaRecoverAmount;
    }

    public void SaveData(ref GameData data)
    {
        data.KillCount = KillCount;
        data.ItemCrafted = ItemCrafted;
        data.FoodConsumed = FoodConsumed;
        data.BeverageConsumed = BeverageConsumed;
        data.MedicineConsumed = MedicineConsumed;
        data.DamageTaken = DamageTaken;
        data.DamageDealt = DamageDealt;
        data.HPRecoverAmount = HPRecoverAmount;
        data.HungerRecoverAmount = HungerRecoverAmount;
        data.StaminaRecoverAmount = StaminaRecoverAmount;
    }

}
