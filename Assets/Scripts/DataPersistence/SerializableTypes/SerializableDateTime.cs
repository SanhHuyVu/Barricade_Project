using System;
using UnityEngine;

[Serializable]
public class SerializableDateTime : ISerializationCallbackReceiver
{
    [SerializeField] private string serializedDateTime;

    private DateTime dateTime;

    public DateTime DateTime
    {
        get => dateTime;
        set => dateTime = value;
    }

    public SerializableDateTime()
    {
        serializedDateTime = null;
        dateTime = default;
    }

    public void OnAfterDeserialize()
    {
        dateTime = DateTime.Parse(serializedDateTime);
    }

    public void OnBeforeSerialize()
    {
        serializedDateTime = dateTime.ToString("HH:mm");
    }
}
