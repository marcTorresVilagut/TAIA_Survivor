using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : MonoBehaviour {
    [HideInInspector]
    public FanSpawner mFanSpawner;
    // Start is called before the first frame update
    void Start() {
        mFanSpawner = GetComponentInParent<FanSpawner>();
    }

    private void OnDestroy() {
        mFanSpawner.FanDestroyed();
    }
}
