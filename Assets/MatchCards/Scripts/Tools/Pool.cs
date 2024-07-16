using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private static Dictionary<GameObject, List<GameObject>> objects = new();

    public static GameObject Get(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Pool>> GameObject that you are trying to get from is null");
            return null;
        }
        GameObject obj;
        if (objects.ContainsKey(prefab))
        {
            if (objects[prefab].Count > 0)
            {
                obj = objects[prefab][0];
                objects[prefab].RemoveAt(0);
            }
            else
            {
                obj = Instantiate(prefab);
            }
            return obj;
        }
        else
        {
            objects.Add(prefab, new());
            obj = Get(prefab);
        }
        return obj;
    }

    public static T Get<T>(T prefab) where T : MonoBehaviour
    {
        if (prefab == null)
        {
            Debug.LogError("Pool>> GameObject that you are trying to get from is null");
            return null;
        }
        return Get(prefab.gameObject).GetComponent<T>();
    }

    public static void Return(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("Pool>> GameObject that you're trying to return is null");
            return;
        }
        TouchController.RemovePauseReason(obj);
        if (objects.ContainsKey(obj))
        {
            objects[obj].Add(obj);
        }
        else
        {
            Destroy(obj);
            Debug.LogWarning("Pool>> GameObject that you're trying to return is not in the pool");
        }
    }
}
