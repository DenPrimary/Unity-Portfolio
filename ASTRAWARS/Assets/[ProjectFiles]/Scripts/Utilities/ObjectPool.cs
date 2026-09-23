using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> pool = new Queue<T>();

    public ObjectPool(T prefab, int initialSize, Transform parent = null) {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++) { 
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get() {
        T obj;

        if (pool.Count > 0)
            obj = pool.Dequeue();
        else
            obj = Object.Instantiate(prefab, parent);

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj) {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        pool.Enqueue(obj);
    }

    public int CountPool => pool.Count;
}
