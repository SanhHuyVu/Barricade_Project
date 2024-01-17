using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform[] cameraMovePoints;
    [SerializeField] private Transform[] cameraLookAtPoints;

    [Header("UI Overplay")]
    [SerializeField] private CanvasGroup mainMenuUI;
    [SerializeField] private CanvasGroup characterSelectUI;


    [Header("MainMenuUI Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    [Header("CharacterSelectUI Buttons")]
    [SerializeField] private Button letsGoButton;
    [SerializeField] private Button backButton;

    [Header("LoadingScreen")]
    [SerializeField] private CanvasGroup loadingScreenCG;
    [SerializeField] private float animTIme = 1;

    [Header("Character Selection")]
    [SerializeField] private CharacterSelect[] characterSelects;

    private bool isSelectingCharacter = false;
    private Transform cameraTF;
    private RaycastHit raycastHit;
    private Transform highlight;
    private Transform selection;

    private void Awake()
    {
        cameraTF = _camera.transform;

        ToggleButtonInteractions(true);

        AddEventToButtons();
    }

    private void Start()
    {
        cameraTF.DOMove(cameraMovePoints[0].position, 0);
        cameraTF.DOLookAt(cameraLookAtPoints[0].position, 0);
        mainMenuUI.transform.DOScale(1, 0);
        characterSelectUI.transform.DOScale(0, 0);

        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueButton.interactable = false;
        }
        if (DataPersistenceManager.Instance.DeleteSaveFile)
        {
            loadingScreenCG.gameObject.SetActive(true);

            loadingScreenCG.transform.DOScale(1, 0).SetEase(Ease.OutQuad);
            loadingScreenCG.DOFade(1, 0);

            loadingScreenCG.transform.DOScale(0, 1).SetEase(Ease.OutQuad);
            loadingScreenCG.DOFade(0, 1);
        }
    }

    private void Update()
    {
        CharacterMouseSelect();
    }

    private void AddEventToButtons()
    {
        /* mainMenuUi Buttons*/
        newGameButton.onClick.AddListener(() =>
        {
            // playButton clicked
            ToggleCharacterSelectionUI();
        });
        continueButton.onClick.AddListener(() =>
        {
            // continueButton clicked
            ToggleButtonInteractions(false);
            loadingScreenCG.transform.DOScale(1, animTIme).SetEase(Ease.OutQuad);
            loadingScreenCG.DOFade(1, animTIme).OnComplete(() =>
            {
                Loader.Load(Loader.Scene.GamePlay);
            });
        });
        quitButton.onClick.AddListener(() =>
        {
            // quitButton clicked
            ToggleButtonInteractions(false);
            Application.Quit();
        });


        /* characterSelectUI Buttons*/
        letsGoButton.onClick.AddListener(() =>
        {
            // letsGoButton clicked
            ToggleButtonInteractions(false);
            loadingScreenCG.transform.DOScale(1, animTIme).SetEase(Ease.OutQuad);
            loadingScreenCG.DOFade(1, animTIme).OnComplete(() =>
            {
                DataPersistenceManager.Instance.NewGame();
                CharacterSpawner.Instance.SetCharacterToLoad();
                Loader.Load(Loader.Scene.GamePlay);
            });
        });
        backButton.onClick.AddListener(() =>
        {
            // backButton clicked
            ToggleButtonInteractions(false);
            ToggleCharacterSelectionUI();
        });
    }

    private void CharacterMouseSelect()
    {
        if (!isSelectingCharacter) return;

        if (highlight != null)
        {
            CharacterSelect characterSelect = highlight.gameObject.GetComponent<CharacterSelect>();
            if (characterSelect != null && characterSelect.CharacterEnum != CharacterSpawner.Instance.NewCharacter) characterSelect.HideOutline();
            highlight = null;
        }

        // highlight the character the player hovers over
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            CharacterSelect characterSelect = highlight.gameObject.GetComponent<CharacterSelect>();
            if (highlight.CompareTag("CharacterSelect") && highlight != selection && characterSelect != null)
            {
                characterSelect.ShowOutline();
            }
            else
            {
                highlight = null;
            }
        }

        if (Input.GetMouseButtonDown(0)) // select and highlight the character the player clicks on
        {
            if (highlight)
            {
                CharacterSelect characterSelect = null;

                for (int i = 0; i < characterSelects.Length; i++)
                    characterSelects[i].HideOutline();

                if (selection != null)
                {
                    characterSelect = selection.gameObject.GetComponent<CharacterSelect>();
                    if (characterSelect != null && characterSelect.CharacterEnum != CharacterSpawner.Instance.NewCharacter)
                        characterSelect.HideOutline();
                }
                selection = raycastHit.transform;
                characterSelect = selection.gameObject.GetComponent<CharacterSelect>();
                if (characterSelect != null)
                {
                    characterSelect.ShowOutline();
                    CharacterSpawner.Instance.SetNewCharacter(characterSelect.CharacterEnum);
                }
                highlight = null;
            }
            // else
            // {
            //     if (selection)
            //     {
            //         CharacterSelect characterSelect = selection.gameObject.GetComponent<CharacterSelect>();
            //         if (characterSelect != null && characterSelect.CharacterEnum != CharacterSpawner.Instance.SelectedCharacter)
            //             characterSelect.HideOutline();
            //         selection = null;
            //     }
            // }
        }
    }

    private void ToggleCharacterSelectionUI()
    {
        // moving the camera
        MoveCamera();
        if (isSelectingCharacter) isSelectingCharacter = false;
        else isSelectingCharacter = true;

        ToggleButtonInteractions(true);
    }

    private void MoveCamera()
    {
        if (isSelectingCharacter) // currently looking at characterSelectUI
        {
            characterSelectUI.transform.DOScale(0, 0.25f);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(cameraTF.DOMove(cameraMovePoints[2].position, 0.5f));
            sequence.Append(cameraTF.DOMove(cameraMovePoints[1].position, 0.2f));
            sequence.Append(cameraTF.DOMove(cameraMovePoints[0].position, 0.5f).OnComplete(() =>
            {
                cameraTF.DOLookAt(cameraLookAtPoints[0].position, 0.5f);
            }));
            cameraTF.DOLookAt(cameraLookAtPoints[0].position, 1.2f).OnComplete(() =>
            {
                mainMenuUI.transform.DOScale(1, 0.25f);
                // cameraTF.DOLookAt(cameraLookAtPoints[0].position, 0);
            });
        }
        else // currently looking at mainMenuUI
        {
            mainMenuUI.transform.DOScale(0, 0.25f);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(cameraTF.DOMove(cameraMovePoints[1].position, 0.2f));
            sequence.Append(cameraTF.DOMove(cameraMovePoints[2].position, 0.5f).OnComplete(() =>
            {
                cameraTF.DOLookAt(cameraLookAtPoints[1].position, 0.5f);
            }));
            sequence.Append(cameraTF.DOMove(cameraMovePoints[3].position, 0.5f));
            cameraTF.DOLookAt(cameraLookAtPoints[1].position, 1.2f).OnComplete(() =>
            {
                characterSelectUI.transform.DOScale(1, 0.25f);
            });
        }
    }

    private void ToggleButtonInteractions(bool intarcatable)
    {
        newGameButton.interactable = intarcatable;
        continueButton.interactable = intarcatable;
        quitButton.interactable = intarcatable;
        letsGoButton.interactable = intarcatable;
        backButton.interactable = intarcatable;
    }

    private void OnDestroy()
    {
        DOTween.Kill(mainMenuUI);
        DOTween.Kill(characterSelectUI);
    }
}
