using UnityEngine;

public abstract class Trap : Interactable
{
    [SerializeField] private ItemDatabaseObject database;

    protected bool triggered = false;
    protected int damage;
    protected int trapID;
    protected Transform trapTF;


    public int TrapID => trapID;
    public Transform TrapTF => trapTF;
    public bool Triggered => triggered;

    public void SetUpTrap(int id, bool triggered = false)
    {
        trapID = id;
        this.triggered = triggered;
        SetTrapTrigger(triggered);
    }

    public void SetTrigger()
    {

    }

    public virtual void SetTrapTrigger(bool triggered)
    {

    }

    protected virtual void TriggerTrap(Enemy enemy)
    {

    }

    public void ResetDmg()
    {
        damage = GetDamage();
    }

    public ItemObject GetItemObject()
    {
        for (int i = 0; i < database.ItemObjects.Length; i++)
        {
            if (database.ItemObjects[i].Data.Id == trapID) return database.ItemObjects[i];
        }
        return null;
    }

    protected int GetDamage()
    {
        var buffs = database.GetItemObjectWithId(TrapID).Data.Buffs;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i].Attribute == Attributes.Attack) return buffs[i].Value;
        }
        return -1;
    }
}
