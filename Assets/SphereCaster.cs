using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class SphereCaster : MonoBehaviour {

    public GameObject currentHitObject;

    public float sphereRadius;
    public float maxDistance;
    public LayerMask layerMask;

    private Vector3 origin;
    private Vector3 direction;

    private float currentHitDistance;
    
    void Update() {
        origin = transform.position;
        direction = transform.up;
        RaycastHit hit;
        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal)) {
            currentHitObject = hit.transform.gameObject;
            currentHitDistance = hit.distance;
        } else {
            currentHitObject = null;
            currentHitDistance = maxDistance;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 end = origin + direction * currentHitDistance;
        Debug.DrawLine(origin, end);
        Gizmos.DrawWireSphere(end, sphereRadius);
    }

    public bool CheckIfInDanger() {
        return (currentHitObject != null && currentHitObject.transform.CompareTag("cannon_ball"));
    }

    public float GetCurrentHitDistance() {
        return currentHitDistance;
    }


}
