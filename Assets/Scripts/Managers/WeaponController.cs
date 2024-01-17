using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get; private set; }

    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackCooldown = .82f;
    private float currentAttackCooldown = 0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DestroyChildrenObject();
    }

    private void Start()
    {
        GameInput.Instance.OnAttackAction += GameInput_OnAttackAction;
    }

    private void GameInput_OnAttackAction(object sender, EventArgs e)
    {
        if (canAttack && !Player.Instance.IsMoving() && Player.Instance.IsHoldingWeapon() && !MouseData.IsMouseBusy)
        {
            PerformAttack();
        }
    }

    private void Update()
    {
        if (Player.Instance.IsMoving())
        {
            currentAttackCooldown = 0f;
            canAttack = true;
            PlayerAnimator.Instance.GetAnimator.SetBool(ConstVariables.PLAYER_ISATTACKING, false);
        }

        if (!canAttack)
        {
            currentAttackCooldown += Time.deltaTime;
            if (currentAttackCooldown > attackCooldown)
            {
                currentAttackCooldown = 0;
                canAttack = true;
                PlayerAnimator.Instance.GetAnimator.SetBool(ConstVariables.PLAYER_ISATTACKING, false);
            }
        }
    }

    public void PutOnWeapon(ItemObject itemObj, ItemType type)
    {
        GameObject item = Instantiate(itemObj.Model, Vector3.zero, quaternion.identity, transform);
        item.transform.localPosition = new Vector3(itemObj.ModelPosition.x, itemObj.ModelPosition.y, itemObj.ModelPosition.z);
        item.transform.localEulerAngles = new Vector3(itemObj.ModelRotation.x, itemObj.ModelRotation.y, itemObj.ModelRotation.z);
    }
    public void RemoveWeapon()
    {
        // weaponType = ItemType.Default; // temporary
        DestroyChildrenObject();
    }

    private void DestroyChildrenObject()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    private void PerformAttack()
    {
        canAttack = false;
        PlayerAnimator.Instance.PerformAttackAnimation();
        PlayerAnimator.Instance.GetAnimator.SetBool(ConstVariables.PLAYER_ISATTACKING, true);
        Player.Instance.ActionCost(0.15f, 0.1f);
    }
}
