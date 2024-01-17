using UnityEngine;

[System.Serializable]
public abstract class StatsManager : MonoBehaviour
{
    [SerializeField] protected Attribute[] attributes;

    [Header("These test stats only display value and dont affect actual stats")]
    [SerializeField] protected int testAttack;
    [SerializeField] protected int testDefense;
    [SerializeField] protected int testMaxHP;

    public int Attack { get; protected set; }
    public int Defense { get; protected set; }
    public int MaxHP { get; protected set; }
    public int CurrentHP { get; private set; }
    public int FlatDamageReduction { get; private set; }

    public int MoveSpeed { get; private set; }
    public int RunSpeed { get; private set; }

    // player only
    public int HungerCost { get; private set; }
    public int SPCost { get; private set; }
    public int Hunger { get; private set; } = 100;
    public int SP { get; private set; } = 100;

    public void SetUpStats()
    {
        UpdateStats(true);
    }

    public virtual void UpdateStats(bool init = false)
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            ModifiableInt value = attributes[i].value;
            switch (attributes[i].type)
            {
                case Attributes.Attack:
                    Attack = value.ModifiedValue;
                    if (attributes[i].value.parent is Player)
                        UIManager.Instance.UpdateAttack(Attack);
                    break;
                case Attributes.Defense:
                    Defense = value.ModifiedValue;
                    if (attributes[i].value.parent is Player)
                        UIManager.Instance.UpdateDefensse(Defense);
                    break;
                case Attributes.MaxHP:
                    if (init)
                    {
                        CurrentHP = value.ModifiedValue;
                        MaxHP = value.ModifiedValue;
                    }
                    else
                    {
                        float hpRatio = (float)CurrentHP / MaxHP;
                        MaxHP = value.ModifiedValue;
                        var newHP = hpRatio * MaxHP;
                        CurrentHP = Mathf.RoundToInt(newHP);
                    }
                    if (attributes[i].value.parent is Player)
                        UIManager.Instance.UpdateVisualHP(CurrentHP, MaxHP);
                    break;
                case Attributes.FlatDamageReduction:
                    FlatDamageReduction = Mathf.Clamp(value.ModifiedValue, 0, int.MaxValue);
                    break;
                case Attributes.MoveSpeed:
                    MoveSpeed = value.ModifiedValue;
                    break;
                case Attributes.RunSpeed:
                    RunSpeed = value.ModifiedValue;
                    break;

                case Attributes.SPCost:
                    SPCost = value.ModifiedValue;
                    break;
                case Attributes.HungerCost:
                    HungerCost = value.ModifiedValue;
                    break;
            }
        }
    }
    public int UpdateHunger(int hunger, bool recover = false)
    {
        int hungerDMG = 0;

        if (recover) Hunger += hunger;
        else Hunger -= hunger;

        if (Hunger < 0) hungerDMG = Hunger * -1;

        Hunger = Mathf.Clamp(Hunger, 0, 100);
        return hungerDMG;
    }
    public int UpdateSP(int sp, bool recover = false)
    {
        int spDmg = 0;

        if (recover) SP += sp;
        else SP -= sp;

        if (SP < 0) spDmg = SP * -1;

        SP = Mathf.Clamp(SP, 0, 100);
        return spDmg;
    }
    public virtual bool TakeDamage(int damage, bool hpLost = false)
    {
        int dmg = 0;
        if (hpLost) dmg = damage;
        else dmg = (damage * 100 / (Defense + 100)) - FlatDamageReduction;
        dmg = Mathf.Clamp(dmg, 1, int.MaxValue);

        if (this is Player) Player.Instance.ScoreTracker.DamageTaken += dmg;
        if (this is Enemy) Player.Instance.ScoreTracker.DamageDealt += dmg;

        CurrentHP -= dmg;
        testMaxHP = CurrentHP; // test
        CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);
        if (CurrentHP <= 0)
            return true;

        return false;
    }
    public void Heal(int HPAmount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + HPAmount, 0, MaxHP);
        UIManager.Instance.UpdateVisualHP(CurrentHP, MaxHP);
    }
    public void SetHP(int newCurrentHP)
    {
        CurrentHP = newCurrentHP;
    }
    public void SetSP(int newSP)
    {
        SP = newSP;
    }
    public void SetHunger(int newHunger)
    {
        Hunger = newHunger;
    }
}
