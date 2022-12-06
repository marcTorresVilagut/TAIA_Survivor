using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class CannonController : MonoBehaviour {
    // Instances models
    public GameObject cannonBall;
    public GameObject cannonMarker;
    public GameObject collideArea;
    public GameObject explosion;
    [HideInInspector]
    public CannonSpawner mCannonSpawner;

    // Cannon Firing variables
    public Transform shotPos;
    public float firePower;

    // Start is called before the first frame update
    void Start() {
        mCannonSpawner = GetComponentInParent<CannonSpawner>();
        // Spawn marker
        Vector3 markPos = new Vector3(shotPos.position.x, -0.40f, shotPos.position.z);
        var i_cannonBallMarker = Instantiate(cannonMarker, markPos, shotPos.rotation);
        i_cannonBallMarker.transform.parent = gameObject.transform;

        // Fire cannon on 1 second
        Invoke(nameof(FireCannon), 1.0f);
    }

    public void FireCannon() {
        // Create cannon ball instance
        var i_cannonBall = Instantiate(cannonBall, shotPos.position, shotPos.rotation);
        i_cannonBall.transform.parent = gameObject.transform;
        Rigidbody cannonBallRB = i_cannonBall.GetComponent<Rigidbody>();
        
        // Add movement force to the cannon ball
        cannonBallRB.AddForce(shotPos.forward * firePower);

        // Create firing explosion particle system
        var i_cannonExplosion = Instantiate(explosion, shotPos.position, shotPos.rotation);
        i_cannonExplosion.transform.parent = gameObject.transform;
    }

    public void CannonBallHit() {
        // Create cannon ball explosion particle syste,
        Vector3 explosion_pos = new Vector3(shotPos.position.x, -0.45f, shotPos.position.z);

        var i_cannonballExplosion = Instantiate(explosion, explosion_pos, shotPos.rotation);
        i_cannonballExplosion.transform.parent = gameObject.transform;
    }
    private void OnDestroy() {
        mCannonSpawner.CannonDestroyed();
    }

}
