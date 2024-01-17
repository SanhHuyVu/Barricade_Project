using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private float initialAngle = 0f;
    [SerializeField] private Transform playerSpawnPoint;

    public Transform PlayerSpawnPoint => playerSpawnPoint;

    public void RotateBuilding(float angle)
    {
        Vector3 currentAngle = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(currentAngle.x, initialAngle + angle, currentAngle.z);
    }
}
