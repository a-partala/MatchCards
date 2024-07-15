using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T GetRandom<T>(this List<T> list) where T : class
    {
        if(list == null || list.Count == 0)
        {
            return null;
        }
        int id = Random.Range(0, list.Count);
        return list[id];
    }
}
