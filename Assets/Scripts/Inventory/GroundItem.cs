using UnityEditor;
using UnityEngine;

public class GroundItem : Interactable, ISerializationCallbackReceiver
{
    [SerializeField] private ItemObject item;
    [SerializeField] private int amount = 1;
    [SerializeField] private LookAtCamera billBoard;

    public ItemObject Item { get { return item; } set { item = value; } }
    public int Amount { get { return amount; } set { amount = value; } }


    public override void ShowSelectedVisual()
    {
        base.ShowSelectedVisual();
        UIManager.Instance.InteractText.text = "<color=green>'E'</color> Pick up";
    }

    public override void HideSelectedVisual()
    {
        base.HideSelectedVisual();
        UIManager.Instance.InteractText.text = "";
    }

    public void UpdateSprite()
    {
        billBoard.GetComponent<SpriteRenderer>().sprite = item.UiDisplay;
        GetComponentInChildren<SpriteRenderer>().sprite = item.UiDisplay;
        selectedInteractableVisual.VisualGameObjects[0].GetComponent<SpriteRenderer>().sprite = item.UiDisplay;
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        GetComponentInChildren<SpriteRenderer>().sprite = item.UiDisplay;
        selectedInteractableVisual.VisualGameObjects[0].GetComponent<SpriteRenderer>().sprite = item.UiDisplay;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }
}
