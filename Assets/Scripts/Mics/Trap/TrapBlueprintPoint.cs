using UnityEngine;

public class TrapBlueprintPoint : MonoBehaviour
{
    private TrapBlueprint trapBlueprint;

    public GameObject trapGO { get; private set; }

    public void SetTrapBlueprint(TrapBlueprint _trapBlueprint, int id)
    {
        if (trapGO != null || _trapBlueprint == null)
        {
            Destroy(trapGO);
            trapBlueprint = null;
        }

        if (_trapBlueprint != null)
        {
            trapGO = Instantiate(_trapBlueprint.gameObject);
            Transform tf = trapGO.transform;
            tf.SetParent(transform);
            tf.localPosition = Vector3.zero;
            trapBlueprint = trapGO.GetComponent<TrapBlueprint>();
            trapBlueprint.SetUpTrap(id);
        }
    }

    public bool PlaceTrap()
    {
        return trapBlueprint.PlaceTrap(transform.position, transform.forward);
    }
}
