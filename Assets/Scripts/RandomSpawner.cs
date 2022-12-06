using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour {
    //public GameObject platform;
    public GameObject objectPrefab;
    public float spawnRate;

    private SurvivorEnvController m_GameController;
    
    private void Start(){
        m_GameController = GetComponentInParent<SurvivorEnvController>();

        float startTime = 2f / spawnRate;
        float intervalTime = 5f / spawnRate;
        InvokeRepeating(nameof(SpawnObject), startTime, intervalTime);
    }

    private void SpawnObject() {
        Vector3 rand_pos = m_GameController.GetRandomSpawnPos();
        Vector3 spawn_pos = new(rand_pos.x, 9f ,rand_pos.z);
        var i_cannon = Instantiate(objectPrefab, spawn_pos, Quaternion.identity);
        i_cannon.transform.parent = gameObject.transform;
    }
}
