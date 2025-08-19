using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Vector3 position;
        public GameObject itemPrefab;
        public string itemName;
    }

    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    private const string ITEM_SPAWNED_KEY = "ItemSpawned_";

    private void Start()
    {
        SpawnItems();
    }

    private void SpawnItems()
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (!HasItemBeenSpawned(spawnPoint.itemName))
            {
                SpawnItem(spawnPoint);
                MarkItemAsSpawned(spawnPoint.itemName);
            }
        }
    }

    private void SpawnItem(SpawnPoint spawnPoint)
    {
        Instantiate(spawnPoint.itemPrefab, spawnPoint.position, Quaternion.identity);
    }

    private void MarkItemAsSpawned(string itemName)
    {
        PlayerPrefs.SetInt(ITEM_SPAWNED_KEY + itemName, 1);
        PlayerPrefs.Save();
    }

    private bool HasItemBeenSpawned(string itemName)
    {
        return PlayerPrefs.GetInt(ITEM_SPAWNED_KEY + itemName, 0) == 1;
    }

    public void ResetSpawnedItems()
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            PlayerPrefs.DeleteKey(ITEM_SPAWNED_KEY + spawnPoint.itemName);
        }
        PlayerPrefs.Save();
    }
}