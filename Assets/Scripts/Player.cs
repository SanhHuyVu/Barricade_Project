using System;
using UnityEngine;

public partial class Player : StatsManager, IDataPersistence
{
    public EventHandler OnTakeDamage;

    public static Player Instance { get; private set; }

    [SerializeField] private float rotateSpeed = 6f;

    [SerializeField] private float hungerConsumeRate = 60f;
    [SerializeField] private float spConsumeRate = 40f;

    [SerializeField] private LayerMask cannotPassThroughLayer;
    [SerializeField] private LayerMask interactableLayerMasks;

    // inventory system
    [SerializeField] private InventoryObject inventory;
    [SerializeField] private InventoryObject equipment;
    [SerializeField] private InventoryObject hotbar;

    [SerializeField] private TrapBlueprintPoint trapBlueprintPoint;

    public InventoryObject Inventory => inventory;
    public InventoryObject Hotbar => hotbar;

    // last Interactable "hover"
    private Interactable hoveredInteractable;

    private bool isMoving, isWalking, isRuning, isHoldingWeapon;
    private float playerRadius = 0.3f, playerHeight = 1.75f;
    private float moveDistance;

    private Vector3 lastInteractDirection;

    private float hungerTimer = 0;
    private float spTimer = 0;

    public InventoryObject Equipment => equipment;
    public TrapBlueprintPoint TrapBlueprintPoint => trapBlueprintPoint;

    public bool IsHoveringAnInteractable => hoveredInteractable != null ? true : false;
    public Transform PlayerTransform { get; private set; }

    public PlayerStatus PlayerStatus { get; private set; }
    public ScoreTracker ScoreTracker { get; private set; }
    public AudioSource AudioSource { get; private set; }


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        PlayerTransform = transform;
        PlayerStatus = new PlayerStatus();
        ScoreTracker = new ScoreTracker();
        AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // press the interact key (the last time checked it was "E")

        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnAlternateInteractAction += GameInput_OnAlternateInteractAction;
        GameInput.Instance.OnBackpackAction += GameInput_OnBackpackAction;

        inventory.Clear();
        equipment.Clear();
        hotbar.Clear();

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].value.Init(this);
        }
        SetUpStats();
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnEquipItem;
        }

        UIManager.Instance.UpdateVisualHunger(Hunger);
        UIManager.Instance.UpdateVisualStamina(SP);
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            HotbarInterface.Instance.UpdateSelectedSlot(HotbarInterface.Instance.SelectedSlotIndex + 1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            HotbarInterface.Instance.UpdateSelectedSlot(HotbarInterface.Instance.SelectedSlotIndex - 1);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) GamePausedUI.Instance.ToggleGamePause();

        if (Input.GetKeyDown(KeyCode.M)) // test TakeDamage()
        {
            TakeDamage(10);
        }
        if (Input.GetKey(KeyCode.L)) // test Loading
        {
            // LOAD GAME
            DataPersistenceManager.Instance.LoadGame();
        }
        if (Input.GetKey(KeyCode.O)) // test Saving
        {
            // SAVE GAME
            DataPersistenceManager.Instance.SaveGame();
        }

        HandleMovement();
        HandleInteractions();
        HandleHungerAndThirst();
    }

    public void OnRemoveItem(InventorySlot slot)
    {
        // unequiting
        if (slot.itemObject == null)
            return;
        switch (slot.Parent.Inventory.Type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:

                if (slot.itemObject.IsWeapon() && slot.IsEquipmentSlot == IsEquipmentSlot.MainHand)
                {
                    WeaponController.Instance.RemoveWeapon();
                    isHoldingWeapon = false;
                }

                //Debug.Log($"Removed {slot.itemObject} on {slot.Parent.Inventory.Type}, Allowed Items: {string.Join(",", slot.AllowedItems)}");
                for (int i = 0; i < slot.Item.Buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == slot.Item.Buffs[i].Attribute)
                            attributes[j].value.RemoveModifier(slot.Item.Buffs[i]);
                    }
                }
                UpdateStats();
                break;
            case InterfaceType.Hotbar:
                break;
            case InterfaceType.Lootable:
                break;
        }
    }
    public void OnEquipItem(InventorySlot slot)
    {
        // equiping
        if (slot.itemObject == null)
            return;
        switch (slot.Parent.Inventory.Type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                //Debug.Log($"Placed {slot.itemObject} on {slot.Parent.Inventory.Type}, Allowed Items: {string.Join(",", slot.AllowedItems)}");

                if (slot.itemObject.IsWeapon() && slot.IsEquipmentSlot == IsEquipmentSlot.MainHand)
                {
                    WeaponController.Instance.PutOnWeapon(slot.itemObject, slot.itemObject.Type);
                    isHoldingWeapon = true;
                }

                for (int i = 0; i < slot.Item.Buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == slot.Item.Buffs[i].Attribute)
                            attributes[j].value.AddModifier(slot.Item.Buffs[i]);
                    }
                }
                UpdateStats();
                break;
            case InterfaceType.Hotbar:
                break;
            case InterfaceType.Lootable:
                break;
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (hoveredInteractable != null)
        {
            if (hoveredInteractable is GroundItem groundItem)
            {
                Item item = new Item(groundItem.Item);
                int amount = groundItem.Amount;
                TryPickUpItem(item, groundItem, amount);
            }
            else if (hoveredInteractable is Trap trap)
            {
                Item item = new Item(trap.GetItemObject());
                TryPickUpItem(item, trap, 1);
            }
            else
            {
                hoveredInteractable.Interact();
            }
        }
    }
    private void GameInput_OnAlternateInteractAction(object sender, EventArgs e)
    {
        if (hoveredInteractable != null)
        {
            hoveredInteractable.AltarnateInteract();
        }
    }
    private void GameInput_OnBackpackAction(object sender, EventArgs e)
    {
        UIManager.Instance.ToggleInventoryMenu();
    }

    private void TryPickUpItem<T>(Item item, T orginItem, int amount)
    {
        if (hotbar.AddItem(item, amount))
        {
            HotbarInterface.Instance.UpdateSelectedSlot();
            SuccessPickUpItem(orginItem);
        }
        else if (inventory.AddItem(item, 1))
        {
            SuccessPickUpItem(orginItem);
        }
    }

    private void SuccessPickUpItem<T>(T item)
    {
        if (item is GroundItem groundItem)
        {
            groundItem?.HideSelectedVisual();
            GroundItemManager.Instance.RemoveGroundItemFromList(groundItem);
        }
        if (item is Trap trap)
        {
            trap?.HideSelectedVisual();
            GroundItemManager.Instance.RemoveTrapFromList(trap);
        }

        Destroy(hoveredInteractable.gameObject);

        hoveredInteractable = null;
    }

    private void HandleHungerAndThirst()
    {
        if (hungerTimer < hungerConsumeRate) hungerTimer += Time.deltaTime;
        if (spTimer < spConsumeRate) spTimer += Time.deltaTime;

        if (isMoving)
        {
            hungerTimer += 0.05f;
            spTimer += 0.05f;
        }
        if (isRuning)
        {
            hungerTimer += 0.1f;
            spTimer += 0.1f;
        }

        if (hungerTimer >= hungerConsumeRate)
        {
            hungerTimer -= hungerConsumeRate;
            int hungerDmg = UpdateHunger(HungerCost);
            UIManager.Instance.UpdateVisualHunger(Hunger);
            if (hungerDmg > 0) TakeDamage(hungerDmg, true);
        }
        if (spTimer >= spConsumeRate)
        {
            spTimer -= spConsumeRate;
            int spDmg = UpdateSP(SPCost);
            UIManager.Instance.UpdateVisualStamina(SP);
            if (spDmg > 0) TakeDamage(spDmg, true);
        }
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }

        float interactDistance = 1.5f;

        if (Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius,
         lastInteractDirection, out RaycastHit raycastHit, interactDistance, interactableLayerMasks))
        {
            if (raycastHit.transform.TryGetComponent(out Interactable interactable))
            {
                // has Interactable component
                if (interactable != hoveredInteractable)
                {
                    if (hoveredInteractable != null)
                        hoveredInteractable?.HideSelectedVisual();

                    hoveredInteractable = interactable;
                    hoveredInteractable?.ShowSelectedVisual();
                }
            }
            else
            {
                OnHoveredInteractableExitHover();
            }
        }
        else
        {
            // if raycast does not hit anything
            OnHoveredInteractableExitHover();
        }
    }

    private void OnHoveredInteractableExitHover()
    {
        if (hoveredInteractable != null)
        {
            hoveredInteractable?.HideSelectedVisual();
            hoveredInteractable = null;
            InteractableInventoryInterface.Instance.UnSetInventory();
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (PlayerAnimator.Instance.GetAnimator.GetBool(ConstVariables.PLAYER_ISATTACKING)) return;

        if (moveDir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        // if (Input.GetKey(KeyCode.Space))
        if (GameInput.Instance.IsSprintButtonPressed())
        {
            moveDistance = RunSpeed * Time.deltaTime;

            moveDir = GetVectorOfMovement(moveDir);

            if (CanMove(moveDir))
            {
                transform.position += moveDir * moveDistance;

                isMoving = moveDir != Vector3.zero;

                isWalking = false;
                isRuning = true;
            }
        }
        else
        {
            moveDistance = MoveSpeed * Time.deltaTime;

            moveDir = GetVectorOfMovement(moveDir);

            if (CanMove(moveDir))
            {
                transform.position += moveDir * moveDistance;

                isMoving = moveDir != Vector3.zero;

                isWalking = true;
                isRuning = false;
            }
        }

        if (!(isMoving = moveDir != Vector3.zero))
        {
            isWalking = false;
            isRuning = false;
        }

    }

    private bool CanMove(Vector3 moveDir)
    {
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
            playerRadius, moveDir, out RaycastHit hit, moveDistance, cannotPassThroughLayer);

        if (!canMove)
        {
            // Debug.Log(hit.collider);
        }

        return canMove;
    }

    private Vector3 GetVectorOfMovement(Vector3 moveDir)
    {
        if (!CanMove(moveDir))
        {
            // cannot move torward moveDir -> attempt to move diagonally
            // attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            bool canMove = CanMove(moveDirX);

            if (canMove)
            {
                // can move only in the X
                return moveDirX;
            }
            else
            {
                // cannot move only in the X
                // attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = CanMove(moveDirZ);

                if (canMove)
                {
                    // can move only in the Z
                    return moveDirZ;
                }
                else
                {
                    // cannot move in any direction
                    return Vector3.zero;
                }
            }
        }
        else
        {
            // cannot move torward moveDir
            return moveDir;
        }
    }

    public bool IsMoving() { return isMoving; }
    public bool IsRuning() { return isRuning; }
    public bool IsWalking() { return isWalking; }
    public bool IsHoldingWeapon() { return isHoldingWeapon; }

    public void ActionCost(float hungerCost, float spCost, bool directDeduction = false)
    {
        if (directDeduction)
        {
            UpdateHunger(HungerCost);
            UIManager.Instance.UpdateVisualHunger(Hunger);
            UpdateSP(SPCost);
            UIManager.Instance.UpdateVisualStamina(SP);
        }
        else
        {
            hungerTimer += hungerCost;
            spTimer += spCost;
        }
    }

    public override bool TakeDamage(int damage, bool hpLost = false)
    {
        if (CurrentHP > 0)
        {
            OnTakeDamage?.Invoke(this, EventArgs.Empty);

            if (!hpLost) PlayerAnimator.Instance.GetAnimator.SetTrigger(ConstVariables.PLAYER_GETHIT);
            bool die = base.TakeDamage(damage);
            UIManager.Instance.UpdateVisualHP(CurrentHP, MaxHP);

            if (die)
            {
                PlayerAnimator.Instance.GetAnimator.SetTrigger(ConstVariables.PLAYER_DIE);
                GameEndUI.Instance.Show(false);
            }

            return die;
        }

        return true;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
        GameInput.Instance.OnAlternateInteractAction -= GameInput_OnAlternateInteractAction;

    }

    public void LoadData(GameData data)
    {
        inventory.Clear();
        equipment.Clear();
        hotbar.Clear();

        transform.position = data.PlayerPosition;
        inventory.LoadContainer(data.PlayerInventory);
        hotbar.LoadContainer(data.PlayerHotbar);
        equipment.LoadContainer(data.PlayerEquipment);

        SetHP(data.CurrentHP);
        SetSP(data.SP);
        SetHunger(data.Hunger);
        UIManager.Instance.UpdateVisualHP(CurrentHP, MaxHP);
        UIManager.Instance.UpdateVisualHunger(Hunger);
        UIManager.Instance.UpdateVisualStamina(SP);
    }

    public void SaveData(ref GameData data)
    {
        if (Player.Instance != null)
        {
            data.PlayerPosition = transform.position;
            data.PlayerInventory = inventory.Container;
            data.PlayerHotbar = hotbar.Container;
            data.PlayerEquipment = equipment.Container;

            data.CurrentHP = CurrentHP;
            data.SP = SP;
            data.Hunger = Hunger;
        }
    }
}
