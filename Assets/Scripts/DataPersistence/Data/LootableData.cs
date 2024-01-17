using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootableData
{
    [SerializeField] private bool alreadySearched;
    [SerializeField] private Inventory container;

    public bool AlreadySearched => alreadySearched;
    public Inventory Container => container;

    public LootableData(bool alreadySearched, Inventory container)
    {
        this.alreadySearched = alreadySearched;
        this.container = container;
    }
}
