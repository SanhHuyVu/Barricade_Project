using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform playerTF;

    private void Start() {
        playerTF = Player.Instance.PlayerTransform;
    }
    private void LateUpdate()
    {
        Vector3 newPosition = playerTF.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
