using UnityEngine;

public class ZombieCollisionDetection : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;

    private bool alreadyHitPlayer = false;
    private int damage;

    public void EnableBoxCollider(int dmg)
    {
        boxCollider.enabled = true;
        damage = dmg;
        alreadyHitPlayer = false;
    }
    public void DisableBoxCollider()
    {
        damage = 0;
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !alreadyHitPlayer)
        {
            Player.Instance.TakeDamage(damage);
            alreadyHitPlayer = true;
        }
    }
}