using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    private static List<string> keys = new List<string>();

    public static void AddKey(string keyName)
    {
        keys.Add(keyName);
        Debug.Log($"Picked up key: {keyName}");
    }

    public static bool HasKey(string keyName)
    {
        return keys.Contains(keyName);
    }

    public static void UseKey(string keyName)
    {
        if (keys.Contains(keyName))
        {
            keys.Remove(keyName);
            Debug.Log($"Used key: {keyName}");
        }
    }

    public static void ClearKeys()
    {
        keys.Clear();
    }
}
