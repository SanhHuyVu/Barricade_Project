using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode { CameraForward, CameraForwardInverted, LookAt, LookAtInverted }

    [SerializeField] private Mode mode;

    private Transform cameraTF;

    private Transform thisTransform;

    private void Awake()
    {
        cameraTF = Camera.main.transform;
        thisTransform = transform;
    }
    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                thisTransform.LookAt(cameraTF);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = thisTransform.position - cameraTF.position;
                thisTransform.LookAt(thisTransform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                thisTransform.forward = cameraTF.forward;
                break;
            case Mode.CameraForwardInverted:
                thisTransform.forward = -cameraTF.forward;
                break;
        }
    }
}
