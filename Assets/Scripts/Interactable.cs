using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected SelectedInteractableVisual selectedInteractableVisual;

    private void Start()
    {

    }

    public virtual void Interact()
    {

    }

    public virtual void AltarnateInteract()
    {

    }

    public virtual void ShowSelectedVisual()
    {
        selectedInteractableVisual?.Show();
    }

    public virtual void HideSelectedVisual()
    {
        selectedInteractableVisual?.Hide();
    }
}
