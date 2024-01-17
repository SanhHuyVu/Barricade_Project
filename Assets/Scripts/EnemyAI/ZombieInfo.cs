using UnityEngine;
using UnityEngine.AI;

public class ZombieInfo : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemySenses enemySenses;
    [SerializeField] private Enemy enemy;
    [SerializeField] private ZombieCollisionDetection zombieCD;
    [SerializeField] private ZombieOccludee occludee;
    [SerializeField] private AudioSource audioSource;

    public NavMeshAgent Agent => agent;
    public EnemySenses EnemySenses => enemySenses;
    public Enemy Enemy => enemy;
    public ZombieCollisionDetection ZombieCD => zombieCD;
    public ZombieOccludee ZombieOccludee => occludee;
    public AudioSource AudioSource => audioSource;
}
