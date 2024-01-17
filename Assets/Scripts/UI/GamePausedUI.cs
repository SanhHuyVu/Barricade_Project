using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GamePausedUI : MonoBehaviour
{
    public static GamePausedUI Instance { get; private set; }
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private CanvasGroup loadingScreenCG;

    public bool IsGamePaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);
        AddEventToButtons();
    }

    public void ToggleGamePause()
    {
        if (GameEndUI.Instance.GameEnd) return;
        if (IsGamePaused) // game was pause -> resume the game
        {
            IsGamePaused = false;
            Time.timeScale = 1;
            gameObject.SetActive(false);
            ToggleButtons(false);
        }
        else // game was not pause -> pause the game
        {
            IsGamePaused = true;
            Time.timeScale = 0;
            gameObject.SetActive(true);
            ToggleButtons(true);
        }
    }

    private void AddEventToButtons()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            if (!TimeController.Instance.IsNightTime()) // only save the game in day time
                DataPersistenceManager.Instance.SaveGame();

            ToggleButtons(false);

            loadingScreenCG.gameObject.SetActive(true);
            loadingScreenCG.transform.DOScale(1, 1).SetEase(Ease.OutQuad).SetUpdate(true);
            loadingScreenCG.DOFade(1, 1).SetUpdate(true).OnComplete(() =>
            {
                Time.timeScale = 1;
                Loader.Load(Loader.Scene.MainMenuScene);
            });
        });
        resumeButton.onClick.AddListener(() =>
        {
            ToggleButtons(false);
            ToggleGamePause();
        });
    }

    private void ToggleButtons(bool on)
    {
        mainMenuButton.interactable = on;
        resumeButton.interactable = on;
    }
    private void OnDestroy()
    {
        DOTween.Kill(loadingScreenCG);
    }
}
