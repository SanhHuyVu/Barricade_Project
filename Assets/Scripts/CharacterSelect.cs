using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private CharacterEnum characterEnum;
    private Outline outline;

    private float r;
    private float g;
    private float b;

    public CharacterEnum CharacterEnum => characterEnum;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        r = outline.OutlineColor.r;
        g = outline.OutlineColor.g;
        b = outline.OutlineColor.b;

        outline.OutlineColor = new Color(r, g, b, 0);
    }

    private void Start()
    {
        if (CharacterSpawner.Instance.NewCharacter == characterEnum)
            ShowOutline();
    }

    public void ShowOutline()
    {
        outline.OutlineColor = new Color(r, g, b, 1);
    }

    public void HideOutline()
    {
        outline.OutlineColor = new Color(r, g, b, 0);
    }
}
