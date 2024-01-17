using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public static PlayerAnimator Instance { get; private set; }

    private Animator animator;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.SetBool(ConstVariables.PLAYER_ISMOVING, Player.Instance.IsMoving());
        animator.SetBool(ConstVariables.PLAYER_ISWALKING, Player.Instance.IsWalking());
        animator.SetBool(ConstVariables.PLAYER_ISRUNING, Player.Instance.IsRuning());
        animator.SetBool(ConstVariables.PLAYER_ISHOLDINGWEAPON, Player.Instance.IsHoldingWeapon());
    }

    private void Update()
    {
        animator.SetBool(ConstVariables.PLAYER_ISMOVING, Player.Instance.IsMoving());
        animator.SetBool(ConstVariables.PLAYER_ISWALKING, Player.Instance.IsWalking());
        animator.SetBool(ConstVariables.PLAYER_ISRUNING, Player.Instance.IsRuning());
        animator.SetBool(ConstVariables.PLAYER_ISHOLDINGWEAPON, Player.Instance.IsHoldingWeapon());
    }

    public void PerformAttackAnimation()
    {
        animator.SetBool(ConstVariables.PLAYER_ISMOVING, false);
        animator.SetBool(ConstVariables.PLAYER_ISWALKING, false);
        animator.SetBool(ConstVariables.PLAYER_ISRUNING, false);

        animator.SetTrigger(ConstVariables.PLAYER_TRIGGERATTACKANIM);
    }

    public Animator GetAnimator { get { return animator; } }
}
