using System.Collections.Generic;
using UnityEngine;

public class ZombieAreaManager : MonoBehaviour
{
    [SerializeField] private int maxZombieInArea = 3;
    [SerializeField] private ZombieInfoSupply zombieSupply;
    [SerializeField] private Transform[] patrolPoints;

    private List<Enemy> currentZombieInArea = new List<Enemy>();

    public void SpawnZombieInArea(bool reInitialize = false)
    {
        RemoveDeadZombieFromList(reInitialize);

        int numToSpawn = Random.Range(0, 5);
        // int numToSpawn = 3;
        for (int i = 0; i < numToSpawn; i++)
        {
            if (currentZombieInArea.Count >= maxZombieInArea) break;

            // var zombieJustSpawned = zombieSupply.GetZombieInfoSupply();
            var zombieJustSpawned = zombieSupply.GetSupply();
            zombieJustSpawned.transform.position = patrolPoints[Random.Range(0, patrolPoints.Length)].position;
            zombieJustSpawned.ZombieOccludee.ToggleHideMeshOnly(false);
            zombieJustSpawned.gameObject.SetActive(true);
            // zombieJustSpawned.Agent.enabled = true;
            zombieJustSpawned.Agent.ResetPath();
            zombieJustSpawned.EnemySenses.Reset();
            zombieJustSpawned.Enemy.SetWaypoints(patrolPoints);
            zombieJustSpawned.Enemy.UpdateStats(true);
            currentZombieInArea.Add(zombieJustSpawned.Enemy);
        }
    }

    private void RemoveDeadZombieFromList(bool reInitialize = false)
    {
        // remove dead zombie from list
        var tempList = currentZombieInArea;
        for (int i = 0; i < currentZombieInArea.Count; i++)
        {
            if (!reInitialize && currentZombieInArea[i].CurrentHP <= 0) tempList.Remove(currentZombieInArea[i]);
            if (reInitialize)
            {
                currentZombieInArea[i].gameObject.SetActive(false);
                tempList.Remove(currentZombieInArea[i]);
            }
        }
        currentZombieInArea = tempList;
    }
}
