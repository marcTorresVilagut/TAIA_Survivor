using UnityEngine;

public class CannonBall : MonoBehaviour {
    public GameObject explosion;
    public CannonController m_CannonController;

    private void Start() {
        m_CannonController = GetComponentInParent<CannonController>();
    }
    private void OnTriggerEnter(Collider collider) {
        //print($"collider.transform.tag {collider.transform.tag}");
        if(collider.transform.CompareTag("agent") ||
           collider.transform.CompareTag("platform") ||
           collider.transform.CompareTag("collision_area")) {
        
            Explode();
        }
    }

    public void Explode() {

        m_CannonController.CannonBallHit();
        // Destroy this object
        Destroy(gameObject);
    }
}
