using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {
    public float strength;
    [HideInInspector]
    public Vector3 direction;
    private GameObject start;
    private GameObject end;

    private void Start() {
        Transform[] ts = gameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) {
            if (t.gameObject.name == "Start") start = t.gameObject;
            if (t.gameObject.name == "End") end = t.gameObject;
        }

        direction = (end.transform.position - start.transform.position).normalized;
    }
}
