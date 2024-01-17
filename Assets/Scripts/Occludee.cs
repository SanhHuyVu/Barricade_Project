using UnityEngine;

public class Occludee : MonoBehaviour
{
    [SerializeField] protected float renderDistance = 30f;
    [SerializeField] protected bool onlyHideMesh = false;

    private MeshRenderer meshRenderer;
    protected bool spawnCorrectly => MapManager.Instance != null;


    private void Awake()
    {
        Awake_();
    }

    protected virtual void Awake_()
    {
        if (onlyHideMesh) meshRenderer = GetComponent<MeshRenderer>();
    }

    // currently not doing dynamic occ on other objs
    // private void Start()
    // {
    //     if (!spawnCorrectly) return;
    //     MapManager.Instance.Occludees.Add(this);
    // }

    // private void OnDestroy()
    // {
    //     if (!spawnCorrectly) return;
    //     MapManager.Instance.Occludees.Remove(this);
    // }

    public virtual void DoOcclusion()
    {
        // bool rendered = Vector3.Distance(transform.position, Player.Instance.PlayerTransform.position) <= renderDistance ? true : false;
        bool rendered = Vector3.SqrMagnitude(Player.Instance.PlayerTransform.position - transform.position) <= renderDistance * renderDistance ? true : false;
        if (onlyHideMesh && meshRenderer != null)
        {
            if (meshRenderer.enabled != rendered) meshRenderer.enabled = rendered;
        }
        else
        {
            if (gameObject.activeSelf != rendered) gameObject.SetActive(rendered);
        }
    }
}
