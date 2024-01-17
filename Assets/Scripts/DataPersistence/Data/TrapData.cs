using UnityEngine;

[System.Serializable]
public class TrapData
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 forwardVector;
    [SerializeField] private int trapID;
    [SerializeField] private bool triggered;

    public Vector3 Position => position;
    public Vector3 ForwardVector => forwardVector;
    public int TrapID => trapID;
    public bool Triggered => triggered;


    public TrapData(Vector3 position, Vector3 forwardVector, int trapID, bool triggered)
    {
        this.position = position;
        this.forwardVector = forwardVector;
        this.trapID = trapID;
        this.triggered = triggered;
    }
}
