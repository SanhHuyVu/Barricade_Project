using Unity.Mathematics;
using UnityEngine;

public class Landmine : Trap
{
    [SerializeField] private int maxHits = 25;
    [SerializeField] private float radius = 6f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private LayerMask blockExplosiontLayer;
    [SerializeField] private float maxDmgPercent = 1;
    [SerializeField] private float minDmgPercent = 0.05f;
    [SerializeField] private ParticleSystem explosion;

    private Collider[] hits;

    private void Awake()
    {
        hits = new Collider[maxHits];
        trapTF = transform;
    }

    public override void ShowSelectedVisual()
    {
        UIManager.Instance.InteractText.text = "<color=yellow>'E'</color> Retrieve";
        selectedInteractableVisual?.Show();
    }

    public override void HideSelectedVisual()
    {
        UIManager.Instance.InteractText.text = "";
        selectedInteractableVisual?.Hide();
    }

    protected override void TriggerTrap(Enemy enemy)
    {
        Exlpode();
    }

    private void Exlpode()
    {
        Instantiate(explosion, trapTF.position, quaternion.identity);
        int _hits = Physics.OverlapSphereNonAlloc(trapTF.position, radius, hits, hitLayer);
        for (int i = 0; i < _hits; i++)
        {
            if (hits[i].CompareTag(ConstVariables.ENEMY_TAG))
            {
                hits[i].TryGetComponent(out Enemy enemy);
                float distance = Vector3.Distance(trapTF.position, enemy.transform.position);
                if (!Physics.Raycast(trapTF.position, (enemy.transform.position - trapTF.position).normalized, distance, blockExplosiontLayer))
                {
                    int dmg = Mathf.FloorToInt(Mathf.Lerp(damage * maxDmgPercent, damage * minDmgPercent, distance / radius));
                    enemy.TakeDamage(dmg);
                }
            }
            if (hits[i].CompareTag(ConstVariables.PLAYER_TAG))
            {
                hits[i].TryGetComponent(out Player player);
                float distance = Vector3.Distance(trapTF.position, player.transform.position);
                if (!Physics.Raycast(trapTF.position, (player.transform.position - trapTF.position).normalized, distance, blockExplosiontLayer))
                {
                    int dmg = Mathf.FloorToInt(Mathf.Lerp(damage * maxDmgPercent, damage * minDmgPercent, distance / radius * 2));
                    player.TakeDamage(dmg);
                }
            }
        }

        GroundItemManager.Instance.RemoveTrapFromList(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConstVariables.ENEMY_TAG) && !triggered && damage > 0)
        {
            TriggerTrap(other.GetComponent<Enemy>());
        }
    }
}
