using System.Collections.Generic;
using UnityEngine;

public abstract class PoolingSupply<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T prefab;
    [SerializeField] protected Transform parent;

    protected List<T> pool = new List<T>();

    public virtual T GetSupply()
    {
        T supply = null;

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy) supply = pool[i];
        }

        if (supply == null)
        {
            supply = Instantiate(prefab);
            supply.transform.SetParent(parent);
            pool.Add(supply);
            return supply;
        }
        supply.gameObject.SetActive(true);
        return supply;
    }
}
