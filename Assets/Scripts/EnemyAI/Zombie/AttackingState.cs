using UnityEngine;
using UnityEngine.AI;

public class AttackingState : StateMachineBehaviour
{
    [SerializeField] private float attackCD = 1.1f;

    private ZombieInfo zombieInfo;
    private EnemySenses enemySenses;
    private Enemy enemy;
    private NavMeshAgent agent;
    private float attackCDTimer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        zombieInfo = animator.GetComponent<ZombieInfo>();
        enemySenses = zombieInfo.EnemySenses;
        enemy = zombieInfo.Enemy;
        agent = zombieInfo.Agent;

        attackCDTimer = 0;

        agent.avoidancePriority = 5;

        if (enemySenses.blockedByDoor == null)
            zombieInfo.ZombieCD.EnableBoxCollider(enemy.Attack);

        if (zombieInfo.AudioSource.isPlaying) zombieInfo.AudioSource.Stop();
        zombieInfo.AudioSource.PlayOneShot(SoundAssets.Instance.ZombieAttackSounds[Random.Range(0, SoundAssets.Instance.ZombieAttackSounds.Length)]);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackCDTimer += Time.deltaTime;
        if (attackCDTimer >= attackCD)
            animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, false);

        if (enemySenses.blockedByDoor == null || (enemySenses.blockedByDoor != null && enemySenses.blockedByDoor.DoorOpened))
        {
            // animator.transform.LookAt(playerTF);  // look toward player
            Vector3 dirToTarget = Player.Instance.PlayerTransform.position - animator.transform.position;
            animator.transform.forward = dirToTarget;
            // float distance = Vector3.Distance(Player.Instance.PlayerTransform.position, animator.transform.position);
            float distanceSquared = Vector3.SqrMagnitude(Player.Instance.PlayerTransform.position - animator.transform.position);

            // out of attacking range
            // if (distanceSquared > 3.5)
            if (distanceSquared > 12.25f)
                animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, false);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.avoidancePriority = 50;
        zombieInfo.ZombieCD.DisableBoxCollider();

        // if (enemySenses.blockedByDoor != null
        // && Vector3.Distance(animator.transform.position, enemySenses.blockedByDoor.transform.position) <= 1.5f)
        if (enemySenses.blockedByDoor != null
            && !enemySenses.blockedByDoor.DoorOpened
            && Vector3.SqrMagnitude(enemySenses.blockedByDoor.transform.position - animator.transform.position) <= 2.25f)
            enemySenses.blockedByDoor.Door.TakeDamage(enemy.Attack);
    }
}
