using UnityEngine;
using UnityEngine.AI;

public class PatrolState : StateMachineBehaviour
{
    [SerializeField] private float patrolTime = 30;
    [SerializeField] private ZombieInfo zombieInfo;
    private float timer;
    private Transform[] waypoints;
    private NavMeshAgent agent;
    private EnemySenses enemySenses;
    private Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;

        zombieInfo = animator.GetComponent<ZombieInfo>();
        enemySenses = zombieInfo.EnemySenses;
        agent = zombieInfo.Agent;
        enemy = zombieInfo.Enemy;
        waypoints = enemy.Waypoints;

        if (TimeController.Instance.IsNightTime() || TimeController.Instance.CurrentDay >= 7)
        {
            animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, true);
            agent.speed = (float)enemy.RunSpeed + Random.Range(0, 2f);
        }
        else
        {
            animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, false);
            agent.speed = enemy.MoveSpeed;
        }

        agent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        // check day or day or night to go into frenzy 
        if (TimeController.Instance.IsNightTime() && !animator.GetBool(ConstVariables.ZOMBIE_ISFRENZY)) animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, true);
        if (!TimeController.Instance.IsNightTime() && animator.GetBool(ConstVariables.ZOMBIE_ISFRENZY)) animator.SetBool(ConstVariables.ZOMBIE_ISFRENZY, false);

        // get random waypoint to patrol if the current waypoint is too close (reach this waypoint)
        if (agent.remainingDistance <= agent.stoppingDistance)
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);

        timer += Time.deltaTime;
        if (timer >= patrolTime || agent.remainingDistance <= agent.stoppingDistance)
            animator.SetBool(ConstVariables.ZOMBIE_ISPATROLLING, false);

        // start chasing
        if (enemySenses.SpotedPlayer)
        {
            animator.SetBool(ConstVariables.ZOMBIE_ISPATROLLING, false);
            animator.SetBool(ConstVariables.ZOMBIE_ISCHASING, true);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
    }
}
