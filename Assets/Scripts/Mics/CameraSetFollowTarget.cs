using Cinemachine;
using UnityEngine;

public class CameraSetFollowTarget : MonoBehaviour
{
    
    [SerializeField] private CinemachineVirtualCamera ccvCamera;

    private CinemachineTransposer body;

    private void Start()
    {
        SnapCamareToPlayer();
    }

    public void SnapCamareToPlayer()
    {
        if (Player.Instance == null)
        {
            Debug.Log("INITIALIZED FAILED: Player Instance null");
            return;
        }

        Transform playerTF = Player.Instance.transform;

        ccvCamera.Follow = null;
        ccvCamera.transform.position = new Vector3(playerTF.position.x, 12, playerTF.position.z);

        ccvCamera.Follow = playerTF;

        body = ccvCamera.AddCinemachineComponent<CinemachineTransposer>();
        body.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
        body.m_FollowOffset = new Vector3(0, 12, -7);
    }
}

