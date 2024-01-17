using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // map
    public float MapAngle;
    public List<Vector3> SBPositions;
    public Vector3 ExtractionPointPos;
    public bool IsExtractionPointAvailable;

    // time
    public SerializableDateTime CurrentTime;
    public int CurrentDay;
    public float SunLightRotation;
    public float MoonLightRotation;
    public bool WaveAlreadySpawned;

    // scores
    public int KillCount;
    public int ItemCrafted;
    public int FoodConsumed;
    public int BeverageConsumed;
    public int MedicineConsumed;
    public int DamageTaken;
    public int DamageDealt;
    public int HPRecoverAmount;
    public int HungerRecoverAmount;
    public int StaminaRecoverAmount;

    // player
    public CharacterEnum SelectedCharacter;
    public Vector3 PlayerPosition;
    public Inventory PlayerInventory = new Inventory();
    public Inventory PlayerHotbar = new Inventory();
    public Inventory PlayerEquipment = new Inventory();
    public int CurrentHP;
    public int SP;
    public int Hunger;

    // interactables
    public SerializableDictionary<string, LootableData> LootableInventories;
    public SerializableDictionary<string, DoorData> DoorDatas;
    public List<GroundItemData> groundItemInfos;
    public List<TrapData> trapInfos;



    public GameData()
    {
        // time
        CurrentTime = new SerializableDateTime();
        CurrentTime.DateTime = DateTime.Now.Date + TimeSpan.FromHours(TimeController.startHour);
        CurrentDay = 1;
        SunLightRotation = 0;
        MoonLightRotation = 0;
        WaveAlreadySpawned = false;

        // map
        MapAngle = 0;
        SBPositions = new List<Vector3>();
        ExtractionPointPos = Vector3.zero;
        IsExtractionPointAvailable = false;

        // scores
        KillCount = 0;
        ItemCrafted = 0;
        FoodConsumed = 0;
        BeverageConsumed = 0;
        MedicineConsumed = 0;
        DamageTaken = 0;
        DamageDealt = 0;
        HPRecoverAmount = 0;
        HungerRecoverAmount = 0;
        StaminaRecoverAmount = 0;

        // player
        SelectedCharacter = CharacterEnum.Male1;
        PlayerPosition = Vector3.zero;
        PlayerInventory.CreateNewSlots(30);
        PlayerHotbar.CreateNewSlots(9);
        PlayerEquipment.CreateNewSlots(5);
        CurrentHP = 0;
        SP = 0;
        Hunger = 0;

        // interactables
        LootableInventories = new SerializableDictionary<string, LootableData>();
        DoorDatas = new SerializableDictionary<string, DoorData>();
        groundItemInfos = new List<GroundItemData>();
        trapInfos = new List<TrapData>();
    }
}
