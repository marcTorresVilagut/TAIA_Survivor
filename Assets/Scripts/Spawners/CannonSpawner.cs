using System.Collections.Generic;
using UnityEngine;

public class CannonSpawner : MonoBehaviour {
    public GameObject objectPrefab;
    public float spawnRate;
    public GameObject target;
    public bool followTarget;
    private SurvivorEnvController m_GameController;
    private float spawnHeight;

    [HideInInspector]
    public int nCannonInstances = 0;

    private void Start(){
        m_GameController = GetComponentInParent<SurvivorEnvController>();
        spawnHeight = 15f;
    }

    private void Update() {
        if (nCannonInstances < spawnRate) {
            if(spawnRate != 1) {
                Vector3 rand_pos = m_GameController.GetRandomSpawnPos();
                Vector3 spawn_pos = new Vector3(rand_pos.x, spawnHeight, rand_pos.z);
                var new_cannon = Instantiate(objectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                new_cannon.transform.parent = gameObject.transform;
                new_cannon.transform.localPosition = spawn_pos;
            } else {
                if (!followTarget) {
                    Vector3 rand_pos = m_GameController.GetRandomSpawnPos();
                    Vector3 spawn_pos = new Vector3(rand_pos.x, spawnHeight, rand_pos.z);
                    var new_cannon = Instantiate(objectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    new_cannon.transform.parent = gameObject.transform;
                    new_cannon.transform.localPosition = spawn_pos;
                } else {
                    Vector3 target_pos = target.transform.localPosition;
                    Vector3 spawn_pos = new Vector3(target_pos.x, spawnHeight, target_pos.z);
                    var new_cannon = Instantiate(objectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    new_cannon.transform.parent = gameObject.transform;
                    new_cannon.transform.localPosition = spawn_pos;
                }
            }
            nCannonInstances += 1;
        }
    }

    private void SpawnObject() {
        if (!followTarget) {
            Vector3 rand_pos = m_GameController.GetRandomSpawnPos();
            Vector3 spawn_pos = new Vector3(rand_pos.x, 12f, rand_pos.z);
            var new_cannon = Instantiate(objectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            new_cannon.transform.parent = gameObject.transform;
            new_cannon.transform.localPosition = spawn_pos;
        } else {
            Vector3 target_pos = target.transform.localPosition;
            Vector3 spawn_pos = new Vector3(target_pos.x, 12f, target_pos.z);
            var new_cannon = Instantiate(objectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            new_cannon.transform.parent = gameObject.transform;
            new_cannon.transform.localPosition = spawn_pos;
        }
    }
    public void CannonDestroyed() {
        nCannonInstances -= 1;
    }
}
