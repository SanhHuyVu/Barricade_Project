using UnityEngine;

[System.Serializable]
public class ItemBuff : IModifier
{
    [SerializeField] private Attributes attribute;
    [SerializeField] private int value;

    public ItemBuff(Attributes attribute, int value)
    {
        this.attribute = attribute;
        this.value = value;
    }

    public Attributes Attribute => attribute;
    public int Value => value;

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }
}
