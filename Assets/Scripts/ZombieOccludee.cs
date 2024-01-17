using UnityEngine;

public class ZombieOccludee : Occludee
{
    [SerializeField] private SkinnedMeshRenderer torsoMesh;
    [SerializeField] private MeshRenderer headMesh;
    [SerializeField] private Enemy enemy;

    private void Start()
    {
        if (!spawnCorrectly) return;
        MapManager.Instance.Occludees.Add(this);
    }

    private void OnDestroy()
    {
        if (!spawnCorrectly) return;
        MapManager.Instance.Occludees.Remove(this);
    }

    public void ToggleHideMeshOnly(bool hideMeshOnly)
    {
        onlyHideMesh = hideMeshOnly;
    }

    public override void DoOcclusion()
    {
        if (torsoMesh == null || headMesh == null || enemy.CurrentHP <= 0) return;
        bool rendered = Vector3.SqrMagnitude(Player.Instance.PlayerTransform.position - transform.position) <= renderDistance * renderDistance ? true : false;

        if (onlyHideMesh)
        {
            if (torsoMesh.enabled != rendered) torsoMesh.enabled = rendered;
            if (headMesh.enabled != rendered) headMesh.enabled = rendered;
        }
        else
        {
            if (gameObject.activeSelf != rendered) gameObject.SetActive(rendered);
        }
    }
}
