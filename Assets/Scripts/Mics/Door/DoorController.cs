using System;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour, IDataPersistence
{
    private TextMeshProUGUI interactText;             //Display the information about how to close/open the door
    private TextMeshProUGUI altInteractText;             //Display the information about how to barricade the door

    private bool playerInZone;                  //Check if the player is in the zone
    public bool DoorOpened { get; private set; }                    //Check if door is currently opened or not
    // public bool DoorOpened => doorOpened;

    private Animation doorAnim;
    private BoxCollider doorCollider;           //To enable the player to go through the door if door is opened else block him

    [SerializeField] private Door door;
    [SerializeField] private float barricadeTime = 3f;
    [SerializeField] private float removeBarricadeTime = 3f;
    public Door Door => door;
    public float BarricadeTime => barricadeTime;
    public float RemoveBarricadeTime => removeBarricadeTime;

    /// <summary>
    /// Initial State of every variables
    /// </summary>
    /// 
    private void Start()
    {
        GameInput.Instance.OnAlternateInteractAction += GameInput_OnAlternateInterction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;

        DoorOpened = false; //Is the door currently opened
        playerInZone = false; //Player not in zone

        interactText = UIManager.Instance.InteractText;
        altInteractText = UIManager.Instance.AltInteractText;

        doorAnim = door.DoorAnim;
        doorCollider = door.DoorCollider;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConstVariables.PLAYER_TAG))
            playerInZone = true;
        if (other.CompareTag(ConstVariables.ENEMY_TAG) && door.HP > 0 && !DoorOpened)
        {
            var enemySenses = other.GetComponent<EnemySenses>();
            enemySenses.blockedByDoor = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ConstVariables.PLAYER_TAG))
        {
            playerInZone = false;
            interactText.text = "";
            altInteractText.text = "";
        }
        if (other.CompareTag(ConstVariables.ENEMY_TAG))
        {
            var enemySenses = other.GetComponent<EnemySenses>();
            enemySenses.blockedByDoor = null;
        }
    }

    private void GameInput_OnAlternateInterction(object sender, EventArgs e)
    {
        // barricade door with "F" by default
        if (playerInZone && !DoorOpened && door.HP > 0 && door.BarricadeLevel < 3 && !UIManager.Instance.DoingAction && !Player.Instance.IsHoveringAnInteractable)
        {
            StartCoroutine(UIManager.Instance.DoAction(this, UIManager.ActionType.Barricade));
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        // remove barricade with "E" by default
        if (door.IsBarricaded && playerInZone && !UIManager.Instance.DoingAction && !Player.Instance.IsHoveringAnInteractable)
        {
            // StartCoroutine(UIManager.Instance.DoRemoveBarricade(this, 1));
            StartCoroutine(UIManager.Instance.DoAction(this, UIManager.ActionType.RemoveBarricade, 1));
            return;
        }

        // open/close door with "E" by default
        if (playerInZone && !UIManager.Instance.DoingAction && !Player.Instance.IsHoveringAnInteractable)
        {
            if (door.IsBarricaded) return;

            if (!DoorOpened && !doorAnim.isPlaying)
            {
                DoorOpened = true;
                doorAnim.Play("Door_Open_spedup");
                altInteractText.text = "";

            }

            if (DoorOpened && !doorAnim.isPlaying)
            {
                DoorOpened = false;
                doorAnim.Play("Door_Close");
                if (door.HP < 300) door.SetHP(300); // Temp: restore hp when closing
            }
        }
    }

    private void Update()
    {
        if (door.HP <= 0 && !DoorOpened)
        {
            doorCollider.enabled = false;
            doorAnim.Play("Door_Open_spedup");
            DoorOpened = true;
        }

        //To Check if the player is in the zone
        if (playerInZone /*&& door.HP > 0*/)
        {
            if (DoorOpened)
            {
                interactText.text = "<color=green>'E'</color> Close";
                doorCollider.enabled = false;
            }
            else
            {
                if (!UIManager.Instance.DoingAction && !Player.Instance.IsHoveringAnInteractable)
                    interactText.text = !door.IsBarricaded ? "<color=green>'E'</color> Open" : "<color=red>'E'</color> Remove Barricade";

                if (!UIManager.Instance.DoingAction && !Player.Instance.IsHoveringAnInteractable)
                    altInteractText.text = door.BarricadeLevel < 3 ? "<color=orange>'F'</color> Barricade" : "";

                doorCollider.enabled = true;
            }
        }
        // if (playerInZone && door.HP <= 0)
        // {
        //     interactText.text = "<color=green>'E'</color> Fix";
        // }
    }

    public void LoadData(GameData data)
    {
        data.DoorDatas.TryGetValue(door.ID, out DoorData doorData);

        door.SetHP(doorData.HP);
        door.SetBarricadeHP(doorData.BarricadeHP);
        door.SetBarricadeLevel(doorData.BarricadeLevel);
        door.UpdateBarricadeVisual();
        DoorOpened = doorData.DoorOpened;
        door.SetIsBarricaded(doorData.IsBarricaded);

        if (DoorOpened) doorAnim.Play("Door_Open_spedup");
        else doorAnim.Play("Door_Close");

    }

    public void SaveData(ref GameData data)
    {
        if (data.DoorDatas.ContainsKey(door.ID)) data.DoorDatas.Remove(door.ID);

        data.DoorDatas.Add(door.ID, new DoorData(door.HP, door.BarricadeHP, door.BarricadeLevel, DoorOpened, door.IsBarricaded));
    }
}
