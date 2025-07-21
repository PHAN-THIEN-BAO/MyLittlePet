using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GemSpawner : MonoBehaviour
{
    [Header("Cài d?t Gem")]
    [SerializeField] private GameObject gemPrefab;
    [SerializeField] private int minGemAmount = 30;
    [SerializeField] private int maxGemAmount = 50;
    [Header("Khu v?c Spawn")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;
    [Header("Tùy ch?n nâng cao")]
    [SerializeField] private float minDistanceFromPlayer = 1f;
    [SerializeField] private float minDistanceBetweenGems = 0.8f;
    [SerializeField] private bool spawnOnAwake = true;
    [SerializeField] private bool respawnWhenCollected = true;
    [SerializeField] private float respawnDelay = 15f;
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private int currentGemCount;
    private int targetGemCount;
    private void Awake()
    {
        if (spawnOnAwake)
        {
            SpawnRandomGems();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Ðang spawn l?i gem...");
            SpawnRandomGems();
        }
    }
    public void SpawnRandomGems()
    {
        if (gemPrefab == null)
        {
            Debug.LogError("Gem Prefab chua du?c gán! Vui lòng gán prefab vào GemSpawner.");
            return;
        }
        Debug.Log("B?t d?u spawn gem...");
        ClearAllGems();
        targetGemCount = Random.Range(minGemAmount, maxGemAmount + 1);
        Debug.Log($"Ðang spawn {targetGemCount} gem trên b?n d?");
        for (int i = 0; i < targetGemCount; i++)
        {
            SpawnSingleGem();
        }
        currentGemCount = targetGemCount;
    }
    private void SpawnSingleGem()
    {
        Vector3 spawnPos;
        int maxAttempts = 50;
        int attempts = 0;
        do
        {
            spawnPos = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                0f
            );
            attempts++;
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Không th? tìm v? trí phù h?p d? spawn gem sau " + maxAttempts + " l?n th?");
                return;
            }
        }
        while (!IsValidSpawnPosition(spawnPos));
        GameObject newGem = Instantiate(gemPrefab, spawnPos, Quaternion.identity);
        newGem.transform.parent = this.transform;
        Debug.Log($"Ðã spawn gem t?i v? trí: {spawnPos}");
        spawnedPositions.Add(spawnPos);
        if (respawnWhenCollected)
        {
            CollectableItems collectableComponent = newGem.GetComponent<CollectableItems>();
            if (collectableComponent != null)
            {
                StartCoroutine(WaitForGemDestroy(newGem));
            }
        }
    }
    private bool IsValidSpawnPosition(Vector3 position)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            if (Vector3.Distance(position, player.transform.position) < minDistanceFromPlayer)
            {
                return false;
            }
        }
        foreach (Vector3 existingPos in spawnedPositions)
        {
            if (Vector3.Distance(position, existingPos) < minDistanceBetweenGems)
            {
                return false;
            }
        }
        return true;
    }
    private IEnumerator WaitForGemDestroy(GameObject gem)
    {
        while (gem != null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        currentGemCount--;
        yield return new WaitForSeconds(respawnDelay);
        if (currentGemCount < targetGemCount)
        {
            SpawnSingleGem();
            currentGemCount++;
        }
    }
    public void ClearAllGems()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        spawnedPositions.Clear();
        currentGemCount = 0;
    }
    public void RespawnAllGems()
    {
        SpawnRandomGems();
    }
    public void SpawnSpecificAmount(int amount)
    {
        minGemAmount = amount;
        maxGemAmount = amount;
        SpawnRandomGems();
    }
}