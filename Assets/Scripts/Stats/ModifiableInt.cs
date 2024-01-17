using System.Collections.Generic;
using UnityEngine;

public delegate void ModifiedEvent();
[System.Serializable]
public class ModifiableInt
{
    public StatsManager parent { get; private set; }

    [SerializeField] private int baseValue;
    public int BaseValue { get { return baseValue; } private set { baseValue = value; /*UpdateModefiedValue1*/} }

    [SerializeField] private int modifiedValue;
    public int ModifiedValue { get { return modifiedValue; } private set { modifiedValue = value; } }

    public List<IModifier> modifiers = new List<IModifier>();

    public event ModifiedEvent ValueModified;
    public ModifiableInt(ModifiedEvent method = null)
    {
        modifiedValue = baseValue;
        if (method != null)
            ValueModified += method;
    }

    public void Init(StatsManager parent, ModifiedEvent method = null)
    {
        this.parent = parent;

        modifiers = new List<IModifier>();
        modifiedValue = BaseValue;
        if (method != null)
            ValueModified += method;
    }

    public void RegsiterModEvent(ModifiedEvent method)
    {
        ValueModified += method;
    }

    public void UnregsiterModEvent(ModifiedEvent method)
    {
        ValueModified -= method;
    }

    public void UpdateModifiedValue()
    {
        var valueToAdd = 0;
        for (int i = 0; i < modifiers.Count; i++)
        {
            modifiers[i].AddValue(ref valueToAdd);
        }
        modifiedValue = baseValue + valueToAdd;
        if (ValueModified != null)
            ValueModified.Invoke();
    }

    public void AddModifier(IModifier _modifier)
    {
        modifiers.Add(_modifier);
        UpdateModifiedValue();
    }

    public void RemoveModifier(IModifier _modifier)
    {
        modifiers.Remove(_modifier);
        UpdateModifiedValue();
    }
}
