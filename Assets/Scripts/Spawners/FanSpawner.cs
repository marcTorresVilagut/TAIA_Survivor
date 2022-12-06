using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSpawner : MonoBehaviour {
    public GameObject fanPrefab;
    public float spawnRate;
    private SurvivorEnvController m_GameController;
    public List<GameObject> SpawnerAreas = new List<GameObject>();

    [HideInInspector]
    public int nFanInstances = 0;

    private void Start() {
        m_GameController = GetComponentInParent<SurvivorEnvController>();
    }

    private void Update() {
        if(nFanInstances< spawnRate) {
            GameObject spawn_area = SpawnerAreas[Random.Range(0, SpawnerAreas.Count)];
            Bounds spawn_area_bounds = spawn_area.GetComponent<BoxCollider>().bounds;
            float x_range = spawn_area_bounds.extents.x;
            var delta_x = Random.Range(-x_range, x_range);

            //spawn_area.transform.rotation
            GameObject i_fan = Instantiate(fanPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            i_fan.transform.parent = gameObject.transform;
            i_fan.transform.localRotation = spawn_area.transform.localRotation;
            i_fan.transform.localPosition = new Vector3(spawn_area.transform.localPosition.x + delta_x, 2.5f, spawn_area.transform.localPosition.z);
            
            nFanInstances += 1;
        }
    }

    public void FanDestroyed() {
        nFanInstances -= 1;
    }
}
