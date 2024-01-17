using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorData
{
    [SerializeField] private int hp;
    [SerializeField] private int barricadeHP;
    [SerializeField] private int barricadeLevel;
    [SerializeField] private bool doorOpened;
    [SerializeField] private bool isBarricaded;

    public int HP => hp;
    public int BarricadeHP => barricadeHP;
    public int BarricadeLevel => barricadeLevel;
    public bool DoorOpened => doorOpened;
    public bool IsBarricaded => isBarricaded;

    public DoorData(int hp, int barricadeHP, int barricadeLevel, bool doorOpened, bool isBarricaded)
    {
        this.hp = hp;
        this.barricadeHP = barricadeHP;
        this.barricadeLevel = barricadeLevel;
        this.doorOpened = doorOpened;
        this.isBarricaded = isBarricaded;
    }
}
