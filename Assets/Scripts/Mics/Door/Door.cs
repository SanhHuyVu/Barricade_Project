using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] private GameObject[] planksOfLvl1 = new GameObject[2];
    [SerializeField] private GameObject[] planksOfLvl2 = new GameObject[2];
    [SerializeField] private GameObject[] planksOfLvl3 = new GameObject[2];
    [SerializeField] private int hp;

    [SerializeField] private int barricadeHP;
    public int BarricadeLevel { get; private set; }

    // has to reference the DoorAnim and DoorCollider like this because transform.parent.getcomponent behave wierdly
    public Animation DoorAnim { get; private set; }
    public BoxCollider DoorCollider { get; private set; }

    public int HP => hp;
    public int BarricadeHP => barricadeHP;
    public bool IsBarricaded { get; private set; }
    public string ID => id;

    private void Awake()
    {
        DoorAnim = GetComponent<Animation>();
        DoorCollider = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        hp = 300;
        BarricadeLevel = 0;
        barricadeHP = 0;
        IsBarricaded = false;
        UpdateBarricadeVisual();
    }

    public void TakeDamage(int dmg)
    {
        if (BarricadeLevel <= 0)
        {
            hp -= dmg;
            return;
        }

        barricadeHP -= dmg;
        if (barricadeHP <= 0)
        {
            BarricadeLevel--;
            BarricadeLevel = Mathf.Clamp(BarricadeLevel, 0, 3);

            if (BarricadeLevel > 0) barricadeHP = 100;
            else
            {
                barricadeHP = 0;
                IsBarricaded = false;
            }
            UpdateBarricadeVisual();
        }
    }

    public bool Barricade()
    {
        if (BarricadeLevel < 3)
        {
            BarricadeLevel++;
            IsBarricaded = true;
            BarricadeLevel = Mathf.Clamp(BarricadeLevel, 0, 3);
            barricadeHP = 100;
            UpdateBarricadeVisual();
            return true;
        }
        return false;
    }

    public void RemoveBarricade(int lvlToRemove)
    {
        BarricadeLevel -= lvlToRemove;
        BarricadeLevel = Mathf.Clamp(BarricadeLevel, 0, 3);
        Debug.Log("Removed Barricade");
        UpdateBarricadeVisual();
        if (BarricadeLevel == 0)
        {
            barricadeHP = 0;
            IsBarricaded = false;
        }
    }

    public bool FixBarricade(int fixHP)
    {
        if (!IsBarricaded) return false;
        barricadeHP += fixHP;
        return true;
    }

    public void UpdateBarricadeVisual()
    {
        switch (BarricadeLevel)
        {
            case 0:
                planksOfLvl1[0].SetActive(false); planksOfLvl1[1].SetActive(false);
                planksOfLvl2[0].SetActive(false); planksOfLvl2[1].SetActive(false);
                planksOfLvl3[0].SetActive(false); planksOfLvl3[1].SetActive(false);
                break;
            case 1:
                planksOfLvl1[0].SetActive(true); planksOfLvl1[1].SetActive(true);
                planksOfLvl2[0].SetActive(false); planksOfLvl2[1].SetActive(false);
                planksOfLvl3[0].SetActive(false); planksOfLvl3[1].SetActive(false);
                break;
            case 2:
                planksOfLvl1[0].SetActive(true); planksOfLvl1[1].SetActive(true);
                planksOfLvl2[0].SetActive(true); planksOfLvl2[1].SetActive(true);
                planksOfLvl3[0].SetActive(false); planksOfLvl3[1].SetActive(false);
                break;
            case 3:
                planksOfLvl1[0].SetActive(true); planksOfLvl1[1].SetActive(true);
                planksOfLvl2[0].SetActive(true); planksOfLvl2[1].SetActive(true);
                planksOfLvl3[0].SetActive(true); planksOfLvl3[1].SetActive(true);
                break;
        }
    }

    public void SetHP(int newHP)
    {
        hp = newHP;
    }
    public void SetBarricadeHP(int newHP)
    {
        barricadeHP = newHP;
    }
    public void SetBarricadeLevel(int newLvl)
    {
        BarricadeLevel = newLvl;
    }
    public void SetIsBarricaded(bool isBarricaded)
    {
        IsBarricaded = isBarricaded;
    }
}
