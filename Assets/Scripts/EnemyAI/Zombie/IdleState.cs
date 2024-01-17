using UnityEngine;
using UnityEngine.AI;

public class IdleState : StateMachineBehaviour
{
    [SerializeField] private float normalIdleTime = 40;
    [SerializeField] private float waveZombieIdleTime = 20;
    [SerializeField] private bool isWaveZombie = false;
    private float timer;
    private ZombieInfo zombieInfo;
    private EnemySenses enemySenses;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        zombieInfo = animator.GetComponent<ZombieInfo>();
        enemySenses = zombieInfo.EnemySenses;

        timer = 0;

        if (zombieInfo.Enemy.Waypoints == null || zombieInfo.Enemy.Waypoints.Length <= 1) isWaveZombie = true;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        // check day or day or night to go into frenzy 
        if (TimeController.Instance.IsNightTime() && !animator.GetBool(ConstVariables.ZOMBIE_ISFRENZY)) animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, true);
        if (!TimeController.Instance.IsNightTime() && animator.GetBool(ConstVariables.ZOMBIE_ISFRENZY)) animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, false);

        // check if door is infront
        if (enemySenses.blockedByDoor != null && !enemySenses.blockedByDoor.DoorOpened) // door is infront and is closed
        {
            // if (Vector3.Distance(animator.transform.position, enemySenses.blockedByDoor.transform.position) <= 1f && enemySenses.blockedByDoor.Door.HP > 0)
            if (Vector3.SqrMagnitude(enemySenses.blockedByDoor.transform.position - animator.transform.position) <= 1)
            {
                animator.transform.LookAt(enemySenses.blockedByDoor.transform); // look toward door
                animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, true);
                return;
            }

            if (enemySenses.blockedByDoor.Door.HP <= 0) enemySenses.blockedByDoor = null;
        }
        

        // switch to patrol or chase the player 
        timer += Time.deltaTime + Random.Range(0, 0.3f);
        if (isWaveZombie && timer >= waveZombieIdleTime) enemySenses.SetLastSeenPosiotion(Player.Instance.PlayerTransform.position);
        if (!isWaveZombie && timer >= normalIdleTime) animator.SetBool(ConstVariables.ZOMBIE_ISPATROLLING, true);

        // float distance = Vector3.Distance(Player.Instance.PlayerTransform.position, animator.transform.position);
        float distanceSquared = Vector3.SqrMagnitude(Player.Instance.PlayerTransform.position - animator.transform.position);

        // keeps on attacking if player is still infront
        // if (distance <= 1.5f && enemySenses.SpotedPlayer) // keeps on attacking
        if (distanceSquared <= 2.25f && enemySenses.SpotedPlayer) // keeps on attacking
        {
            // animator.transform.LookAt(Player.Instance.PlayerTransform); // look toward player
            Vector3 dirToTarget = Player.Instance.PlayerTransform.position - animator.transform.position;
            animator.transform.forward = dirToTarget;
            animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, true);
            return;
        }

        // start chasing
        if (enemySenses.SpotedPlayer || enemySenses.LastSeenAtPos != Vector3.zero) // start chasing
            animator.SetBool(ConstVariables.ZOMBIE_ISCHASING, true);
    }
    // override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    // {

    // }
}
