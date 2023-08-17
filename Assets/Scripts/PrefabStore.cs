using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabStore : MonoBehaviour
{
    public static PrefabStore Instance;

    [SerializeField] private GameObject[] prefabs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static bool GetPrefabOfType<T>(out T value)
    {
        for (int i = 0; i < Instance.prefabs.Length; i++)
        {
            T component = Instance.prefabs[i].GetComponent<T>();
            if (component != null)
            {
                value = component;
                return true;
            }
        }

        Debug.LogWarning($"No prefab exists of type {typeof(T)}");
        value = default;
        return false;
    }
}
