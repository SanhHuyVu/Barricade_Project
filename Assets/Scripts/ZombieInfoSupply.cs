public class ZombieInfoSupply : PoolingSupply<ZombieInfo>
{
    // public ZombieInfo GetZombieInfoSupply()
    // {
    //     ZombieInfo zombieInfo = GetSupply();
    //     zombieInfo.Agent.enabled = false;
    //     return zombieInfo;
    // }

    public override ZombieInfo GetSupply()
    {
        ZombieInfo supply = null;

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy && pool[i].Enemy.CurrentHP <= 0) supply = pool[i];
        }

        if (supply == null)
        {
            supply = Instantiate(prefab);
            supply.transform.SetParent(parent);
            pool.Add(supply);
            supply.gameObject.SetActive(false);
            return supply;
        }
        return supply;
    }
}
