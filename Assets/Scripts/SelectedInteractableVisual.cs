using System.Collections.Generic;
using UnityEngine;

public class SelectedInteractableVisual : MonoBehaviour
{
    [SerializeField] protected List<GameObject> visualGameObjects;

    public List<GameObject> VisualGameObjects => visualGameObjects;

    public void Show()
    {
        foreach (GameObject gameObject in visualGameObjects)
        {
            gameObject.SetActive(true);
        }
    }
    public void Hide()
    {
        foreach (GameObject gameObject in visualGameObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
