using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pipePairPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.25f;
    [SerializeField] private float minY = -1.5f;
    [SerializeField] private float maxY = 2.5f;

    private readonly List<GameObject> spawnedPipes = new List<GameObject>();
    private float timer;
    private bool isSpawning;

    private void Update()
    {
        if (!isSpawning)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnPipePair();
        }
    }

    public void BeginSpawning()
    {
        timer = 0f;
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void ResetSpawner()
    {
        isSpawning = false;
        timer = 0f;

        for (int i = spawnedPipes.Count - 1; i >= 0; i--)
        {
            if (spawnedPipes[i] != null)
            {
                Destroy(spawnedPipes[i]);
            }
        }

        spawnedPipes.Clear();
    }

    private void SpawnPipePair()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

        GameObject instance = Instantiate(pipePairPrefab, spawnPosition, Quaternion.identity);
        spawnedPipes.Add(instance);
    }
}