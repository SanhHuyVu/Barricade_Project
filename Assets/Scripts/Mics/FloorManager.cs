using UnityEngine;
public class FloorManager : MonoBehaviour
{
    [SerializeField] private GameObject roofs;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && roofs != null)
        {
            if (roofs.activeSelf) roofs.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && roofs != null)
        {
            roofs.SetActive(true);
        }
    }

}
