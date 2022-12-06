using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LifetimeController : MonoBehaviour {
    public float lifetime;

    // Update is called once per frame
    void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) Destroy(this.gameObject);
    }
}
