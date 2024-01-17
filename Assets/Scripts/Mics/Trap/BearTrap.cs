using UnityEngine;

public class BearTrap : Trap
{
    private Animation trapAnimation;
    private void Awake()
    {
        trapTF = transform;
        trapAnimation = GetComponent<Animation>();
        damage = GetDamage();
    }

    public override void AltarnateInteract()
    {
        SetTrapTrigger(false);
    }

    public override void ShowSelectedVisual()
    {
        if (triggered)
        {
            // Debug.Log("fixable");
            UIManager.Instance.AltInteractText.text = "<color=green>'F'</color> Fix";
        }
        UIManager.Instance.InteractText.text = "<color=yellow>'E'</color> Retrieve";
        selectedInteractableVisual?.Show();
    }
    public override void HideSelectedVisual()
    {
        UIManager.Instance.AltInteractText.text = "";
        UIManager.Instance.InteractText.text = "";
        selectedInteractableVisual?.Hide();
    }

    public override void SetTrapTrigger(bool triggered)
    {
        if (triggered) // set the trigger to TRUE and update visual
        {
            this.triggered = triggered;
            UIManager.Instance.AltInteractText.text = "";
            trapAnimation.Play(ConstVariables.TRAP_CLOSE);
        }
        else // set the trigger to FALSE and update visual
        {
            this.triggered = triggered;
            UIManager.Instance.AltInteractText.text = "";
            trapAnimation.Play(ConstVariables.TRAP_OPEN);
        }
    }

    protected override void TriggerTrap(Enemy enemy)
    {
        triggered = true;
        enemy.TakeDamage(damage);
        trapAnimation.Play(ConstVariables.TRAP_CLOSE);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConstVariables.ENEMY_TAG) && !triggered)
        {
            // Debug.Log("Stepped on trap");
            // trapAnimation.Play(ConstVariables.TRAP_CLOSE);
            // triggered = true;
            TriggerTrap(other.GetComponent<Enemy>());
        }
    }
}
