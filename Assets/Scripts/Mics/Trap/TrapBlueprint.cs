using UnityEngine;

public class TrapBlueprint : MonoBehaviour
{
    [SerializeField] private Trap trap;
    [SerializeField] LayerMask notBuildableLayerMask;

    [SerializeField] private GameObject valid;
    [SerializeField] private GameObject unValid;

    private int objCollide = 0;
    private int trapID;
    private bool triggered;

    private void Awake()
    {
        valid.SetActive(true);
        unValid.SetActive(false);
    }

    public bool PlaceTrap(Vector3 position, Vector3 forwardVector)
    {
        if (objCollide > 0) return false;

        GameObject trapObj = Instantiate(trap.gameObject);
        trapObj.transform.forward = forwardVector;
        trapObj.transform.position = position;
        var _trap = trapObj.GetComponent<Trap>();
        _trap.SetUpTrap(trapID, triggered);
        _trap.ResetDmg();
        GroundItemManager.Instance.AddTrapToList(_trap);
        return true;
    }

    public void SetUpTrap(int id, bool triggered = false)
    {
        trapID = id;
        this.triggered = triggered;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & notBuildableLayerMask) != 0)
        {
            //It matched one
            objCollide++;
            if (objCollide > 0)
            {
                // Debug.Log("not buildable");
                if (valid.activeSelf) valid.SetActive(false);
                if (!unValid.activeSelf) unValid.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & notBuildableLayerMask) != 0)
        {
            //It matched one
            objCollide--;
            objCollide = Mathf.Clamp(objCollide, 0, int.MaxValue);
            if (objCollide == 0)
            {
                // Debug.Log("buildable");
                if (!valid.activeSelf) valid.SetActive(true);
                if (unValid.activeSelf) unValid.SetActive(false);
            }
        }
    }
}
