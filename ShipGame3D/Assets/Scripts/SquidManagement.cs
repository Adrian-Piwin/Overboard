using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SquidManagement : MonoBehaviour
{
    public float animationLength;

    [SerializeField] private List<Transform> spawnPosList;
    [SerializeField] private GameObject squidPrefab;
    [SerializeField] private CargoManager cargoManager;

    [NonSerialized] public float spawnRate;
    [NonSerialized] public bool isSpawning = true;
    private List<bool> activeSpawners;

    // Start is called before the first frame update
    void Start()
    {
        activeSpawners = new List<bool>(new bool[spawnPosList.Count]);
        StartCoroutine(SpawnSquid());
    }

    IEnumerator SpawnSquid()  
    {
        while (isSpawning) 
        {
            yield return new WaitForSeconds(spawnRate);

            // Check if there is any cargo to throw
            if (!cargoManager.IsMissingCargo())
            {
                continue;
            }

            // Get spawn location
            Transform spawnPos = GetSpawnLocation();
            if (spawnPos == null)
            {
                continue;
            }

            // Check if still need to spawn
            if (!isSpawning) break;

            // Spawn squid
            GameObject squid = Instantiate(squidPrefab, spawnPos.position, spawnPos.rotation, transform);
            squid.GetComponent<Squid>().cargoManager = cargoManager.transform;
            squid.GetComponent<Squid>().AssignCargo(cargoManager.GetMissingCargo());
        }

    }

    private Transform GetSpawnLocation() 
    {
        // Return a random available spawner location

        List<int> indexList = new List<int>();
        for (int i = 0; i < activeSpawners.Count; i++)
        {
            if (!activeSpawners[i]) indexList.Add(i);
        }

        // Return null if no spawners available
        if (indexList.Count == 0) return null;

        int ind = indexList[UnityEngine.Random.Range(0, indexList.Count)];
        activeSpawners[ind] = true;
        StartCoroutine(MakeSpawnerAvailable(ind));
        return spawnPosList[ind];
    }

    IEnumerator MakeSpawnerAvailable(int ind) 
    {
        // Make spawner available after certain time
        yield return new WaitForSeconds(animationLength);
        activeSpawners[ind] = false;
    }
}
