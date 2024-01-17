using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    public static GameEndUI Instance { get; private set; }

    [Header("Score Texts")]
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI itemCraftedText;
    [SerializeField] private TextMeshProUGUI foodConsumedText;
    [SerializeField] private TextMeshProUGUI beverageConsumedText;
    [SerializeField] private TextMeshProUGUI medicineConsumedText;
    [SerializeField] private TextMeshProUGUI hungerRecoverText;
    [SerializeField] private TextMeshProUGUI SPRecoverText;
    [SerializeField] private TextMeshProUGUI HPRecoverText;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    [Header("Win-lose cases")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI gameEndMessage;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup loadingScreenCG;

    [Header("Buttons")]
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button exitGameBtn;

    [Header("Array of score objects")]
    [SerializeField] private float displayRate = 0.5f;
    [SerializeField] private GameObject[] scoreObjArray;

    public bool GameEnd { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // mainMenu button
        mainMenuBtn.onClick.AddListener(() =>
        {
            DisableButtons();

            loadingScreenCG.gameObject.SetActive(true);
            loadingScreenCG.transform.DOScale(1, 1).SetEase(Ease.OutQuad).SetUpdate(true);
            loadingScreenCG.DOFade(1, 1).SetUpdate(true).OnComplete(() =>
            {
                Time.timeScale = 1;
                Loader.Load(Loader.Scene.MainMenuScene);
            });
        });

        // exitGame button
        exitGameBtn.onClick.AddListener(() =>
        {
            DisableButtons();
            Time.timeScale = 1;
            Application.Quit();
        });

        Hide();
    }

    public void Show(bool win)
    {
        GameEnd = true;
        if (win)
        {
            gameEndMessage.text = "You Escaped!";
            background.color = winColor;
        }
        else
        {
            gameEndMessage.text = "You Died!";
            background.color = loseColor;
        }

        // delete savefile when game ends
        DataPersistenceManager.Instance.DeleteSaveFile = true;
        DataPersistenceManager.Instance.FileDataHandler.DeleteSaveFile();

        gameObject.SetActive(true);
        StartCoroutine(DisplayScore());
    }
    public void Hide()
    {
        for (int i = 0; i < scoreObjArray.Length; i++)
        {
            scoreObjArray[i].SetActive(false);
        }
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private IEnumerator DisplayScore()
    {
        Time.timeScale = 0;
        canvasGroup.DOFade(0, 0).SetUpdate(true);

        float appearTime = 2;
        canvasGroup.DOFade(1, appearTime).SetUpdate(true);
        yield return new WaitForSecondsRealtime(appearTime);

        ScoreTracker st = Player.Instance.ScoreTracker;

        killCountText.text = st.KillCount.ToString();
        itemCraftedText.text = st.ItemCrafted.ToString();
        foodConsumedText.text = st.FoodConsumed.ToString();
        beverageConsumedText.text = st.BeverageConsumed.ToString();
        medicineConsumedText.text = st.MedicineConsumed.ToString();
        hungerRecoverText.text = st.HungerRecoverAmount.ToString();
        SPRecoverText.text = st.StaminaRecoverAmount.ToString();
        HPRecoverText.text = st.HPRecoverAmount.ToString();
        totalScoreText.text = st.GetScore().ToString();

        for (int i = 0; i < scoreObjArray.Length; i++)
        {
            yield return new WaitForSecondsRealtime(displayRate);
            scoreObjArray[i].SetActive(true);
        }
    }

    private void DisableButtons()
    {
        mainMenuBtn.interactable = false;
        exitGameBtn.interactable = false;
    }

    private void OnDestroy()
    {
        DOTween.Kill(canvasGroup);
        DOTween.Kill(loadingScreenCG);
    }
}
