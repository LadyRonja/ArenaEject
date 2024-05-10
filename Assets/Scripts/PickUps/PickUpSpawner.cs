using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PickUp pickUpPrefab;
    [SerializeField] private List<PickUp> pickUpPrefabs = new();
    [SerializeField] private float spawnDelay = 0f;
    [SerializeField] private float spawnInterval = 10f;
    private PickUp myCurrentPickUp;

    private void Start()
    {
        InvokeRepeating(nameof(AttemptSpawn), spawnDelay, spawnInterval);
    }


    private void AttemptSpawn()
    {
        if (myCurrentPickUp != null) return;

        if(pickUpPrefabs.Count == 0)
        {
            myCurrentPickUp = Instantiate(pickUpPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            int rand = Random.Range(0, pickUpPrefabs.Count);
            myCurrentPickUp = Instantiate(pickUpPrefabs[rand], spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
