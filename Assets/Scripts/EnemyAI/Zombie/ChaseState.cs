using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    private ZombieInfo zombieInfo;
    private NavMeshAgent agent;
    private EnemySenses enemySenses;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        zombieInfo = animator.GetComponent<ZombieInfo>();
        agent = zombieInfo.Agent;
        enemySenses = zombieInfo.EnemySenses;

        if (TimeController.Instance.IsNightTime() || TimeController.Instance.CurrentDay >= 7)
        {
            animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, true);
            agent.speed = (float)zombieInfo.Enemy.RunSpeed + Random.Range(0, 1f);
        }
        else
        {
            animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, false);
            agent.speed = zombieInfo.Enemy.MoveSpeed;
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // check day or day or night to go into frenzy 
        if (TimeController.Instance.IsNightTime() && !animator.GetBool(ConstVariables.ZOMBIE_ISFRENZY)) animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, true);
        if (!TimeController.Instance.IsNightTime() && animator.GetBool(ConstVariables.ZOMBIE_ISFRENZY)) animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, false);

        // check if door is infront
        if (enemySenses.blockedByDoor != null && !enemySenses.blockedByDoor.DoorOpened) // door is infront and is closed
        {
            // if (Vector3.Distance(animator.transform.position, enemySenses.blockedByDoor.transform.position) <= 1f)
            if (Vector3.SqrMagnitude(enemySenses.blockedByDoor.transform.position - animator.transform.position) <= 1)
            {
                animator.transform.LookAt(enemySenses.blockedByDoor.transform);
                animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, true);
                animator.SetBool(ConstVariables.ZOMBIE_ISCHASING, false);
                return;
            }

            if (enemySenses.blockedByDoor.Door.HP <= 0) enemySenses.blockedByDoor = null;
        }


        // float distance = Vector3.Distance(Player.Instance.PlayerTransform.position, animator.transform.position);
        float distanceSquared = Vector3.SqrMagnitude(Player.Instance.PlayerTransform.position - animator.transform.position);

        // get close enough then start attacking
        // if (distanceSquared < 1.5f)
        if (distanceSquared < 2.25f)
        {
            animator.transform.LookAt(Player.Instance.PlayerTransform);
            animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, true);
            animator.SetBool(ConstVariables.ZOMBIE_ISCHASING, false);
            return;
        }

        // completly lost sigh of target, stop chasing
        if (!enemySenses.SpotedPlayer && enemySenses.LastSeenAtPos == Vector3.zero)
            animator.SetBool(ConstVariables.ZOMBIE_ISCHASING, false);

        // move to last seen position of target
        if (enemySenses.LastSeenAtPos != Vector3.zero)
        {
            // if (Vector3.Distance(enemySenses.LastSeenAtPos, animator.transform.position) <= 1.5f)
            if (Vector3.SqrMagnitude(enemySenses.LastSeenAtPos - animator.transform.position) <= 2.25f)
                enemySenses.ResetLastSeenPosiotion();
            agent.SetDestination(enemySenses.LastSeenAtPos);
        }

        // stop chasing if reaches the last seen position
        if (!enemySenses.SpotedPlayer && enemySenses.LastSeenAtPos == Vector3.zero)
            animator.SetBool(ConstVariables.ZOMBIE_ISCHASING, false);

        // chasing when spotted player
        if (enemySenses.SpotedPlayer)
            agent.SetDestination(Player.Instance.PlayerTransform.position);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
    }
}
