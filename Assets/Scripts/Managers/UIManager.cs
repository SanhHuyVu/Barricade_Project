using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public enum ActionType { Barricade, RemoveBarricade, Searching }

    public static UIManager Instance { get; private set; }

    // player's stats
    [SerializeField] private Image visualHP;
    [SerializeField] private Image visualHunger;
    [SerializeField] private Image visualSP;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private TextMeshProUGUI spText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;

    [SerializeField] private GameObject inventoryMenu;
    [Header("Close button to close inventory")]
    [SerializeField] private Button closeBtn;

    // action options and progress
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI altInteractText;
    [Header("Barricade")]
    [SerializeField] private GameObject barricadeProgressBar;
    [SerializeField] private Image barricadeProgress;
    [Header("Remove Barricade")]
    [SerializeField] private GameObject removeBarricadeProgressBar;
    [SerializeField] private Image removeBarricadeProgress;
    [Header("search")]
    [SerializeField] private GameObject searchingProgressBar;
    [SerializeField] private Image searchingProgress;
    [Header("Use Item")]
    [SerializeField] private TextMeshProUGUI useItemText;
    [Header("Time and day")]
    [Header("ItemUseageCD")]
    [SerializeField] private GameObject foodUseCD;
    [SerializeField] private Image foodUseCDProgress;
    [SerializeField] private GameObject beverageUseCD;
    [SerializeField] private Image beverageUseCDProgress;
    [SerializeField] private GameObject pillUseCD;
    [SerializeField] private Image pillUseCDProgress;

    [Header("Inputs For Android")]
    [SerializeField] private GameObject androidInput;
    [SerializeField] private CanvasGroup actionInputs;

    private bool isInventoryMenusOpen = true;
    public bool IsInventoryMenusOpen => isInventoryMenusOpen;

    private bool doingAction = false;

    private Player playerInstance;

    private bool inventoryOpenAble = true;

    public TextMeshProUGUI InteractText => interactText;
    public TextMeshProUGUI AltInteractText => altInteractText;
    public bool DoingAction => doingAction;

    public TextMeshProUGUI UseItemText => useItemText;

    public GameObject FoodUseCD => foodUseCD;
    public GameObject BeverageUseCD => beverageUseCD;
    public GameObject PillUseCD => pillUseCD;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ToggleInventoryMenu(0);

        barricadeProgressBar.SetActive(false);
        removeBarricadeProgressBar.SetActive(false);
        searchingProgressBar.SetActive(false);

        interactText.text = "";
        altInteractText.text = "";
        useItemText.text = "";

        androidInput.SetActive(false);

        closeBtn.onClick.AddListener(delegate { ToggleInventoryMenu(0.5f); });

#if UNITY_ANDROID
        androidInput.SetActive(true);
#endif
    }

    private void Start()
    {
        playerInstance = Player.Instance;
        playerInstance.OnTakeDamage += CancleAction;
    }

    public void UpdateVisualHP(int hp, int maxHp)
    {
        float newHPNormalized = (float)hp / maxHp;
        Mathf.Clamp(newHPNormalized, 0, 1);
        visualHP.rectTransform.localScale = new Vector3(newHPNormalized, 1, 1);
        hpText.text = $"{hp} / {maxHp}";
    }

    public void UpdateVisualHunger(int hunger)
    {
        float newHungerNormalized = (float)hunger / 100;
        Mathf.Clamp(newHungerNormalized, 0, 1);
        visualHunger.rectTransform.localScale = new Vector3(newHungerNormalized, 1, 1);
        hungerText.text = $"{hunger} / 100";
    }

    public void UpdateVisualStamina(int sp)
    {
        float newSPNormalized = (float)sp / 100;
        Mathf.Clamp(newSPNormalized, 0, 1);
        visualSP.rectTransform.localScale = new Vector3(newSPNormalized, 1, 1);
        spText.text = $"{sp} / 100";
    }

    public void UpdateItemUseCDProgress(ItemType itemType, float currentCd, float maxCd)
    {
        switch (itemType)
        {
            case ItemType.Food:
                foodUseCDProgress.fillAmount = currentCd / maxCd;
                break;
            case ItemType.medicine:
                pillUseCDProgress.fillAmount = currentCd / maxCd;
                break;
            case ItemType.Beverage:
                beverageUseCDProgress.fillAmount = currentCd / maxCd;
                break;
        }
    }

    public void UpdateAttack(int attack)
    {
        attackText.text = $"att: {attack}";
    }
    public void UpdateDefensse(int defense)
    {
        defenseText.text = $"def: {defense}";
    }

    public void ToggleInventoryMenu(float toggleSpeed = 0.5f)
    {
        if (!inventoryOpenAble) return;
        inventoryOpenAble = false;
        var inventoryTranform = inventoryMenu.transform;
        if (isInventoryMenusOpen)
        {
            isInventoryMenusOpen = false;
            closeBtn.interactable = false;
            inventoryTranform.DOMoveY(inventoryTranform.position.y - Screen.height, toggleSpeed).SetUpdate(true).OnComplete(() =>
            {
                closeBtn.interactable = true;
                inventoryOpenAble = true;
            });
        }
        else
        {
            isInventoryMenusOpen = true;
            closeBtn.interactable = false;
            inventoryMenu.transform.DOMoveY(inventoryTranform.position.y + Screen.height, toggleSpeed).SetUpdate(true).OnComplete(() =>
            {
                closeBtn.interactable = true;
                inventoryOpenAble = true;
            });
        }

#if UNITY_ANDROID
        // actionInputs.SetActive(!isInventoryMenusOpen);
        if (isInventoryMenusOpen) actionInputs.transform.DOScale(0, 0);
        else actionInputs.transform.DOScale(1, 0);
#endif
    }

    public IEnumerator DoAction<T>(T sender, ActionType actionType, int levelToRemove = 0)
    {
        // Debug.Log("Start couroutine");
        doingAction = true;
        float currentActionProgress = 0;

        DoActionInit(actionType);

        bool isDoingAction = true;

        while (isDoingAction)
        {
            if (playerInstance.IsMoving() || !doingAction)
            {
                doingAction = false;
                DoActionCancel(actionType, sender);
                yield break;
            }

            currentActionProgress += Time.deltaTime;
            DoActionUpdateProgress(actionType, sender, currentActionProgress);
            yield return null;

            isDoingAction = !DoActionCheckComplete(actionType, sender, currentActionProgress, levelToRemove);
        }
    }

    private void DoActionInit(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Barricade:
                barricadeProgress.fillAmount = 0;
                barricadeProgressBar.SetActive(true);
                altInteractText.text = "Barricading";
                if (playerInstance.AudioSource.isPlaying) playerInstance.AudioSource.Stop();
                playerInstance.AudioSource.PlayOneShot(SoundAssets.Instance.Barricading);
                break;
            case ActionType.RemoveBarricade:
                barricadeProgress.fillAmount = 0;
                removeBarricadeProgressBar.SetActive(true);
                interactText.text = "Removing Barricade";
                break;
            case ActionType.Searching:
                searchingProgress.fillAmount = 0;
                searchingProgressBar.SetActive(true);
                interactText.text = "Searching";
                break;
        }
    }

    private void DoActionCancel<T>(ActionType actionType, T sender)
    {
        switch (actionType)
        {
            case ActionType.Barricade:
                barricadeProgressBar.SetActive(false);
                altInteractText.text = "<color=yellow>'F'</color> Barricade";
                playerInstance.AudioSource.Stop();
                break;
            case ActionType.RemoveBarricade:
                removeBarricadeProgressBar.SetActive(false);
                // interactText.text = sender.ConvertTo<DoorController>().Door.IsBarricaded ? "<color=green>'E'</color> Open" : "<color=red>'E'</color> Remove Barricade";
                DoorController doorController = sender as DoorController;
                interactText.text = doorController.Door.IsBarricaded ? "<color=green>'E'</color> Open" : "<color=red>'E'</color> Remove Barricade";
                break;
            case ActionType.Searching:
                searchingProgressBar.SetActive(false);
                interactText.text = "";
                break;
        }
    }

    private void DoActionUpdateProgress<T>(ActionType actionType, T sender, float currentProgress)
    {
        switch (actionType)
        {
            case ActionType.Barricade:
                // barricadeProgress.fillAmount = currentProgress / sender.ConvertTo<DoorController>().BarricadeTime;
                DoorController doorController = sender as DoorController;
                barricadeProgress.fillAmount = currentProgress / doorController.BarricadeTime;
                break;
            case ActionType.RemoveBarricade:
                doorController = sender as DoorController;
                // removeBarricadeProgress.fillAmount = currentProgress / sender.ConvertTo<DoorController>().RemoveBarricadeTime;
                removeBarricadeProgress.fillAmount = currentProgress / doorController.RemoveBarricadeTime;
                break;
            case ActionType.Searching:
                // searchingProgress.fillAmount = currentProgress / sender.ConvertTo<Lootable>().SearchTime;
                Lootable lootable = sender as Lootable;
                searchingProgress.fillAmount = currentProgress / lootable.SearchTime;
                break;
        }
    }

    private bool DoActionCheckComplete<T>(ActionType actionType, T sender, float currentProgress, int levelToRemove = 0)
    {
        switch (actionType)
        {
            case ActionType.Barricade:
                // DoorController doorController = sender.ConvertTo<DoorController>();
                DoorController doorController = sender as DoorController;
                if (currentProgress >= doorController.BarricadeTime)
                {
                    doingAction = false;
                    barricadeProgressBar.SetActive(false);
                    interactText.text = !doorController.Door.IsBarricaded ? "<color=green>'E'</color> Open" : "<color=red>'E'</color> Remove Barricade";
                    altInteractText.text = doorController.Door.BarricadeLevel < 3 ? "<color=yellow>'F'</color> Barricade" : "";
                    doorController.Door.Barricade();
                    if (playerInstance.AudioSource.isPlaying) playerInstance.AudioSource.Stop();
                    return true;
                }
                break;
            case ActionType.RemoveBarricade:
                // doorController = sender.ConvertTo<DoorController>();
                doorController = sender as DoorController;
                if (currentProgress >= doorController.RemoveBarricadeTime)
                {
                    doingAction = false;
                    removeBarricadeProgressBar.SetActive(false);
                    interactText.text = !doorController.Door.IsBarricaded ? "<color=green>'E'</color> Open" : "<color=red>'E'</color> Remove Barricade";
                    altInteractText.text = doorController.Door.BarricadeLevel < 3 ? "<color=yellow>'F'</color> Barricade" : "";
                    doorController.Door.RemoveBarricade(levelToRemove);
                    return true;
                }
                break;
            case ActionType.Searching:
                // Lootable lootable = sender.ConvertTo<Lootable>();
                Lootable lootable = sender as Lootable;
                if (currentProgress >= lootable.SearchTime)
                {
                    doingAction = false;
                    searchingProgressBar.SetActive(false);
                    interactText.text = "";
                    lootable.DoSetInventory();
                    return true;
                }
                break;
        }
        return false;
    }

    public void CancleAction(object sender, EventArgs e)
    {
        playerInstance.AudioSource.Stop();
        doingAction = false;
    }

    private void OnDestroy()
    {
        DOTween.Kill(actionInputs);
    }
}
