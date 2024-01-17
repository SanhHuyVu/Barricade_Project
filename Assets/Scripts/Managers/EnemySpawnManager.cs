using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance { get; private set; }
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float minDisanceToSpawn = 50;
    [SerializeField] private ZombieInfo enemyPrefab; /*old*/
    [SerializeField] private ZombieInfoSupply zombieSupply; /*pooling*/
    [SerializeField] private List<ZombieAreaManager> zombieAreas;
    [SerializeField] private int finalDay = 7;

    public int FinalDay => finalDay;

    public List<ZombieAreaManager> ZombieAreas => zombieAreas;

    private int[,] waves = {
        /*{minEnemy, maxEnemy}*/
        {1,4},/*day 1*/
        {2,5},/*day 2*/
        {3,6},/*day 3*/
        {6,8},/*day 4*/
        {7,10},/*day 5*/
        {10,12},/*day 6*/
    };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator SpawnWaveZombies(int day)
    {
        SortSpawnPointsBaseOnPlayerDistance(spawnPoints, Player.Instance.PlayerTransform);

        List<Transform> possibleSpawnPoints = new List<Transform>();
        for (int i = 0; i < 3; i++)
        {
            // pick out at most 3 closest spawnpoints to player
            possibleSpawnPoints.Add(spawnPoints[i]);
        }

        // day between 0 and 5 (6 days) day 7th is special wave
        if (day == finalDay)
        {
            // this is day 7 -> final day
            yield break;
        }


        int enemyNumber = UnityEngine.Random.Range(waves[day - 1, 0], waves[day - 1, 1]);
        // Debug.Log($"{enemyNumber} enemies!");

        if (day > waves.Length && day > finalDay)
        {
            // these are days after final day (7th day)
            enemyNumber = 50;
        }

        Debug.Log($"Spawned {enemyNumber} wave zombies");
        for (int i = 0; i < enemyNumber; i++)
        {
            int random = UnityEngine.Random.Range(0, possibleSpawnPoints.Count);

            if (GetDistanceToPlayerSquared(possibleSpawnPoints[random], Player.Instance.PlayerTransform) >= minDisanceToSpawn * minDisanceToSpawn)
            {
                // var zombieJustSpawned = zombieSupply.GetZombieInfoSupply();
                var zombieJustSpawned = zombieSupply.GetSupply();
                zombieJustSpawned.transform.position = possibleSpawnPoints[random].position;
                zombieJustSpawned.ZombieOccludee.ToggleHideMeshOnly(true);
                zombieJustSpawned.gameObject.SetActive(true);
                // zombieJustSpawned.Agent.enabled = true;
                zombieJustSpawned.Agent.ResetPath();
                zombieJustSpawned.EnemySenses.Reset();
                zombieJustSpawned.Enemy.SetWaypoints(null);
                zombieJustSpawned.Enemy.UpdateStats(true);
            }
            else i--;
        }
    }

    public void SpawnZombieInAreas(bool reInitialize = false)
    {
        for (int i = 0; i < zombieAreas.Count; i++)
        {
            zombieAreas[i].SpawnZombieInArea(reInitialize);
        }
    }

    // private float GetDistanceToPlayer(Transform spawnPoint, Transform player)
    // {
    //     return Vector3.Distance(spawnPoint.position, player.position);
    // }

    private float GetDistanceToPlayerSquared(Transform spawnPoint, Transform player)
    {
        return Vector3.SqrMagnitude(player.position - spawnPoint.position);
    }

    private void SortSpawnPointsBaseOnPlayerDistance(Transform[] _spawnPoints, Transform player)
    {
        Array.Sort(_spawnPoints, delegate (Transform spawnPoint1, Transform spawnPoint2)
        {
            return GetDistanceToPlayerSquared(spawnPoint1, player).CompareTo(GetDistanceToPlayerSquared(spawnPoint2, player));
        });
    }
}
