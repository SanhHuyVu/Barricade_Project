using UnityEngine;

[System.Serializable]
public class GroundItemData
{
    [SerializeField] private int itemObjectId;
    [SerializeField] private Vector3 position;
    [SerializeField] private int amount;
    public int ItemObjectId => itemObjectId;
    public Vector3 Position => position;
    public int Amount => amount;

    public GroundItemData(int itemObjectId, Vector3 position, int amount)
    {
        this.itemObjectId = itemObjectId;
        this.position = position;
        this.amount = amount;
    }
}
