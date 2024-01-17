using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private List<Transform> hittedEnemies;

    private void Awake()
    {
        hittedEnemies = new List<Transform>();
    }

    private void Update()
    {
        if (!PlayerAnimator.Instance.GetAnimator.GetBool(ConstVariables.PLAYER_ISATTACKING))
        {
            hittedEnemies.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ConstVariables.ENEMY_TAG && PlayerAnimator.Instance.GetAnimator.GetBool(ConstVariables.PLAYER_ISATTACKING))
        {
            // Debug.Log($"Collided");
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null && (hittedEnemies.Count == 0 || !hittedEnemies.Contains(other.transform)))
            {
                hittedEnemies.Add(other.transform);
                enemy.TakeDamage(Player.Instance.Attack);
                // Debug.Log($"Attacking this enemy: {other.gameObject.name}");
            }
        }
    }
}
