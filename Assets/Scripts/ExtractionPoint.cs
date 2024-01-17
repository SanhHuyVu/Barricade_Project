using DG.Tweening;
using TMPro;
using UnityEngine;

public class ExtractionPoint : MonoBehaviour
{
    [Header("Helicopter")]
    [SerializeField] private GameObject helicopter;
    [SerializeField] private Transform helicopterStartPoint;
    [SerializeField] private float helicopterHeightInAir = 15;
    [SerializeField] private float helicopterTimeToArrive = 15;

    [Header("Progress")]
    [SerializeField] private float extractionTime = 5;
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private Color textMeshColor1;
    [SerializeField] private Color textMeshColor2;
    [SerializeField] private Transform progress;
    [SerializeField] private GameObject displayProgress;

    private Transform helicopterTF;
    private Vector3 extractionPointXZ;
    private bool isReadyForExtraction = false;
    private bool helicopterMoving = false;
    private float currentProgress;

    private void Awake()
    {

        helicopter.SetActive(false);
        ToggleOnOff(false);
        CancelProgress();
    }

    private void OnEnable()
    {
        RotateRandomAngle();
        ToggleOnOff(true);
    }

    private void OnDisable()
    {
        ToggleOnOff(false);

    }

    private void UpdateProgress()
    {
        if (!displayProgress.activeSelf) displayProgress.SetActive(true);

        currentProgress = Mathf.Clamp(currentProgress += Time.deltaTime, 0, extractionTime);
        progress.localScale = new Vector3(currentProgress / extractionTime, 1, 1);

        if (currentProgress >= extractionTime)
        {
            // TODO: win game
            EndGameWin();
        }
    }

    private void CancelProgress()
    {
        currentProgress = 0;
        displayProgress.SetActive(false);
        progress.localScale = new Vector3(0, 1, 1);
        textMeshPro.gameObject.SetActive(false);
    }

    private void FlickeringText()
    {
        if (!textMeshPro.gameObject.activeSelf) textMeshPro.gameObject.SetActive(true);
        textMeshPro.color = Color.Lerp(textMeshColor1, textMeshColor2, Mathf.PingPong(Time.time, 1));
    }

    private void MoveHelicopter()
    {
        textMeshPro.text = "Waiting For Helicoptor";
        extractionPointXZ = new Vector3(transform.position.x, helicopterHeightInAir, transform.position.z);

        helicopterTF = helicopter.transform;
        helicopterTF.position = helicopterStartPoint.position;
        helicopterTF.LookAt(extractionPointXZ);
        helicopter.SetActive(true);

        helicopterMoving = true;
        helicopterTF.DOMove(extractionPointXZ, helicopterTimeToArrive).OnComplete(() =>
        {
            isReadyForExtraction = true;
            textMeshPro.text = "Evacuating";
        });
    }

    private void RotateRandomAngle()
    {
        float angle = UnityEngine.Random.Range(0, 360);
        Vector3 curretnAngle = transform.eulerAngles;
        transform.eulerAngles = new Vector3(curretnAngle.x, angle, curretnAngle.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(ConstVariables.PLAYER_TAG)) return;

        if (!helicopterMoving && !isReadyForExtraction) MoveHelicopter();

        if (isReadyForExtraction) UpdateProgress();

        FlickeringText();
    }

    private void EndGameWin()
    {
        GameEndUI.Instance.Show(true);
    }

    public void ToggleOnOff(bool on)
    {
        if (gameObject.activeSelf != on) gameObject.SetActive(on);
        Canvas_DestinationPointer.Instance.ToggleOnOff(on);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(ConstVariables.PLAYER_TAG)) return;
        CancelProgress();
    }
}
