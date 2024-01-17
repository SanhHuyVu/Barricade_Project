using UnityEngine;

public class Canvas_DestinationPointer : MonoBehaviour
{
    public static Canvas_DestinationPointer Instance { get; private set; }
    [SerializeField] private Camera _camera;
    [SerializeField] private RectTransform pointerRectTransform;
    [SerializeField] private float borderSize = 50f;
    [SerializeField] private Transform extractionPointTF;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }

    private void Update()
    {
        Vector3 toPosition = extractionPointTF.position;
        Vector3 fromPosition = Player.Instance.PlayerTransform.position;

        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = GetAngleFromVector(dir);
        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);

        Vector3 targetPositionScreenPoint = _camera.WorldToScreenPoint(toPosition);
        bool isOffScreen =
                targetPositionScreenPoint.x <= borderSize || // off screen to the left
                targetPositionScreenPoint.x >= Screen.width - borderSize || // off screen to the right
                targetPositionScreenPoint.y <= borderSize || // off screen to below
                targetPositionScreenPoint.y >= Screen.height - borderSize; // off screen to above

        // Debug.Log($"{isOffScreen} {targetPositionScreenPoint}");

        if (isOffScreen)
        {
            if (!pointerRectTransform.gameObject.activeSelf) pointerRectTransform.gameObject.SetActive(true);
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

            Vector3 pointerWorldPosition = cappedTargetScreenPosition;
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0);
        }
        else
        {
            // Vector3 pointerWorldPosition = targetPositionScreenPoint;
            // pointerRectTransform.position = pointerWorldPosition;
            // pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0);
            if (pointerRectTransform.gameObject.activeSelf) pointerRectTransform.gameObject.SetActive(false);
        }
    }

    public void ToggleOnOff(bool on)
    {
        if (gameObject?.activeSelf != on) gameObject.SetActive(on);
    }

    public float GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
